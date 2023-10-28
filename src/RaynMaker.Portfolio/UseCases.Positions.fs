﻿namespace RaynMaker.Portfolio.UseCases

module Depot =
    open RaynMaker.Portfolio
    open RaynMaker.Portfolio.Entities

    type private Msg = 
        | Init of (unit -> DomainEvent list)
        | Get of AsyncReplyChannel<Position list>
        | Stop 

    type Api = {
        Get: unit -> Position list
        Stop: unit -> unit
    }

    let create handleLastChanceException init =
        let agent = Agent<Msg>.Start(fun inbox ->
            let rec loop store =
                async {
                    let! msg = inbox.Receive()

                    match msg with
                    | Init f -> return! loop (f() |> Positions.create)
                    | Get replyChannel -> 
                        replyChannel.Reply store
                        return! loop store
                    | Stop -> return ()
                }
            loop [] ) 

        agent.Error.Add(fun ex -> handleLastChanceException "Building up positions failed"  ex)
        
        agent.Post(init |> Init)

        { Get = fun () -> agent.PostAndReply( fun replyChannel -> replyChannel |> Get)
          Stop = fun () -> agent.Post Stop }

module PositionsInteractor =
    open RaynMaker.Portfolio
    open RaynMaker.Portfolio.Entities
    open System

    type OpenPositionEvaluation = {
        Position : Position
        PricedAt : DateTime
        // TODO: why is there an option? if count is zero then the position is closed
        BuyingPrice : decimal<Currency> option
        BuyingValue : decimal<Currency>
        CurrentPrice : decimal<Currency>
        CurrentValue : decimal<Currency>
        MarketProfit : decimal<Currency>
        DividendProfit : decimal<Currency>
        MarketRoi : decimal<Percentage>
        DividendRoi : decimal<Percentage>
        MarketProfitAnnual : decimal<Currency>
        DividendProfitAnnual : decimal<Currency>
        MarketRoiAnnual : decimal<Percentage>
        DividendRoiAnnual : decimal<Percentage> }
    
    let evaluate broker getLastPrice (p:Position) =
        let getFee = p.AssetId |> function
            | Isin _ -> Broker.getFee broker
            | Commodity _ -> Broker.getFee broker // TODO: to be analyzed what fees would be realistic
            | Coin _ -> fun _ -> 1.0M<Currency> // TODO: workaround for missing support of mutltiple brokers

        let value,pricedAt = 
            let price = p.AssetId |> getLastPrice |> Option.get // there has to be a price otherwise there would be no position
            p.Payouts + p.Count * price.Value - (getFee price.Value), price.Day

        let investedYears = (pricedAt - p.OpenedAt).TotalDays / 365.0 |> decimal
        let marketRoi = (value - p.Invested) / p.Invested * 100.0M<Percentage>
        // TODO: this is not 100% correct: we would have to compare the dividends to the 
        // invested capital in that point in time when we got the dividend
        let dividendRoi = p.Dividends / p.Invested * 100.0M<Percentage>
            
        { 
            Position = p
            PricedAt = pricedAt
            BuyingPrice = if p.Count > 0.0M then (p.Invested - p.Payouts) / p.Count |> Some else None
            BuyingValue = p.Invested - p.Payouts
            CurrentPrice = (p.AssetId |> getLastPrice |> Option.get).Value
            CurrentValue = (p.AssetId |> getLastPrice |> Option.get).Value * p.Count
            MarketProfit = value - p.Invested
            DividendProfit = p.Dividends
            MarketRoi = marketRoi
            DividendRoi = dividendRoi
            MarketProfitAnnual = if investedYears = 0.0M then 0.0M<Currency> else (value - p.Invested) / investedYears
            DividendProfitAnnual = if investedYears = 0.0M then 0.0M<Currency> else p.Dividends / investedYears
            MarketRoiAnnual = if investedYears = 0.0M then 0.0M<Percentage> else marketRoi / investedYears 
            DividendRoiAnnual = if investedYears = 0.0M then 0.0M<Percentage> else dividendRoi / investedYears
        }

    // TODO: core calcs like "ROI" or "DividendROI" should be part of entities - this logic is independent from the API
    // and is partially part of the UL of DDD
    // TODO: we should consider ignoring the broker here - it will anyhow be a guess. we do not consider it in BDD tests. 
    // we just need to make clear in the app / help that it is not considered
    let evaluateOpenPositions broker getLastPrice positions =
        positions
        |> Seq.filter(fun p -> p.ClosedAt |> Option.isNone)
        |> Seq.map (evaluate broker getLastPrice)
        |> List.ofSeq

    type ClosedPositionEvaluation = {
        Position : Position
        Duration : TimeSpan
        MarketProfit : decimal<Currency>
        DividendProfit : decimal<Currency>
        TotalProfit : decimal<Currency>
        TotalRoi : decimal<Percentage>
        MarketProfitAnnual : decimal<Currency>
        DividendProfitAnnual : decimal<Currency>
        MarketRoiAnnual : decimal<Percentage>
        DividendRoiAnnual : decimal<Percentage> }

    let evaluateClosedPositions positions =
        let evaluate (p:Position) =
            let value,pricedAt = p.Payouts,(p.ClosedAt |> Option.get)

            let investedYears = (pricedAt - p.OpenedAt).TotalDays / 365.0 |> decimal
            let marketRoi = (value - p.Invested) / p.Invested * 100.0M<Percentage>
            // TODO: this is not 100% correct: we would have to compare the dividends to the 
            // invested capital in that point in time when we got the dividend
            let dividendRoi = p.Dividends / p.Invested * 100.0M<Percentage>
            
            { 
                ClosedPositionEvaluation.Position = p
                Duration = pricedAt - p.OpenedAt
                MarketProfit = value - p.Invested
                DividendProfit = p.Dividends
                TotalProfit = value - p.Invested + p.Dividends
                TotalRoi = marketRoi + dividendRoi
                MarketProfitAnnual = (value - p.Invested) / investedYears
                DividendProfitAnnual = p.Dividends / investedYears
                MarketRoiAnnual = marketRoi / investedYears 
                DividendRoiAnnual = dividendRoi / investedYears
            }

        positions
        |> Seq.filter(fun p -> p.ClosedAt |> Option.isSome)
        |> Seq.map evaluate
        |> List.ofSeq

    type ProfitEvaluation = {
        Profit : decimal<Currency>
        Roi : decimal<Percentage>
        RoiAnnual : decimal<Percentage> }

    let evaluateProfit broker getLastPrice positions =
        let evaluate (p:Position) =
            let value,pricedAt = 
                match p.ClosedAt with
                | Some c -> p.Payouts,c
                | None -> let price = p.AssetId |> getLastPrice |> Option.get // there has to be a price otherwise there would be no position
                          p.Payouts + p.Count * price.Value - (Broker.getFee broker price.Value),price.Day

            let investedYears = (pricedAt - p.OpenedAt).TotalDays / 365.0 |> decimal
            let marketRoi = (value - p.Invested) / p.Invested * 100.0M<Percentage>
            // TODO: this is not 100% correct: we would have to compare the dividends to the 
            // invested capital in that point in time when we got the dividend
            let dividendRoi = p.Dividends / p.Invested * 100.0M<Percentage>
            
            { 
                Profit = value - p.Invested + p.Dividends
                Roi = marketRoi + dividendRoi
                RoiAnnual = (marketRoi / investedYears + dividendRoi / investedYears)
            }

        positions
        |> Seq.map evaluate
        |> List.ofSeq

module StatisticsInteractor =
    open PositionsInteractor
    open RaynMaker.Portfolio.Entities

    type DiversificationReport = {
        Positions : (string*decimal<Currency>) list
        }

    /// gets diversification report according to current value of the position 
    /// based on last price and share count
    let getDiversification getLastPrice (positions:Position list) =
        let investmentPerPositions =
            positions
            |> Seq.filter(fun p -> p.ClosedAt |> Option.isNone)        
            |> Seq.map(fun p -> 
                let price = p.AssetId |> getLastPrice |> Option.get // there has to be a price otherwise there would be no position
                p.Name, p.Count * price.Value)  
            |> List.ofSeq
        
        { Positions = investmentPerPositions } 
