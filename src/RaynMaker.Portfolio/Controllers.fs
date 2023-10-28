﻿module RaynMaker.Portfolio.Controllers

open System
open RaynMaker.Portfolio.Entities
open RaynMaker.Portfolio.UseCases
open RaynMaker.Portfolio.UseCases.BenchmarkInteractor

module private Format =
    let date (date:DateTime) = date.ToString("yyyy-MM-dd")
    let timespan (span:TimeSpan) = 
        match span.TotalDays with
        | x when x > 365.0 -> sprintf "%.2f years" (span.TotalDays / 365.0)
        | x when x > 90.0 -> sprintf "%.2f months" (span.TotalDays / 30.0)
        | _ -> sprintf "%.0f days" span.TotalDays
    let count = function
        | Isin _ -> sprintf "%.2f"
        | Commodity _ -> sprintf "%.2f"
        | Coin _ -> sprintf "%.6f"
    let currency = sprintf "%.2f"
    let currencyOpt = Option.map currency >> Option.defaultValue "n.a"
    let percentage = sprintf "%.2f"

type OpenPositionVM = {
    Name : string
    Isin : string
    Shares : string
    Duration : string
    BuyingPrice : string
    BuyingValue : string
    PricedAt : string
    CurrentPrice : string
    CurrentValue : string
    MarketProfit : string
    DividendProfit : string
    TotalProfit : string
    MarketRoi : string
    DividendRoi : string
    TotalRoi : string
    MarketProfitAnnual : string
    DividendProfitAnnual : string
    TotalProfitAnnual : string
    MarketRoiAnnual : string
    DividendRoiAnnual : string
    TotalRoiAnnual : string
}

type OpenPositionsVM = {
    TotalInvestment : string
    CurrentValue : string
    TotalProfit : string
    Positions : OpenPositionVM list
}

let listOpenPositions (depot:Depot.Api) broker lastPriceOf = 
    let positions = 
        depot.Get() 
        |> PositionsInteractor.evaluateOpenPositions broker lastPriceOf
    {
        TotalInvestment = positions |> List.sumBy(fun x -> x.BuyingValue) |> Format.currency
        CurrentValue = positions |> List.sumBy(fun x -> x.CurrentValue) |> Format.currency
        TotalProfit = positions |> List.sumBy(fun x -> x.MarketProfit + x.DividendProfit) |> Format.currency
        Positions =
            positions
            |> List.map(fun p -> 
                {            
                    Name = p.Position.Name
                    Isin = p.Position.AssetId |> Str.ofAssetId
                    Shares = p.Position.Count |> Format.count p.Position.AssetId
                    Duration = p.PricedAt - p.Position.OpenedAt |> Format.timespan
                    BuyingPrice = p.BuyingPrice |> Format.currencyOpt
                    BuyingValue = p.BuyingValue |> Format.currency
                    PricedAt = p.PricedAt |> Format.date
                    CurrentPrice = p.CurrentPrice |> Format.currency
                    CurrentValue = p.CurrentValue |> Format.currency
                    MarketProfit = p.MarketProfit |> Format.currency
                    DividendProfit = p.DividendProfit |> Format.currency
                    TotalProfit = p.MarketProfit + p.DividendProfit |> Format.currency
                    MarketRoi = p.MarketRoi |> Format.percentage
                    DividendRoi = p.DividendRoi |> Format.percentage
                    TotalRoi = p.MarketRoi + p.DividendRoi |> Format.percentage
                    MarketProfitAnnual = p.MarketProfitAnnual |> Format.currency
                    DividendProfitAnnual = p.DividendProfitAnnual |> Format.currency
                    TotalProfitAnnual = p.MarketProfitAnnual + p.DividendProfitAnnual |> Format.currency
                    MarketRoiAnnual = p.MarketRoiAnnual |> Format.percentage
                    DividendRoiAnnual = p.DividendRoiAnnual |> Format.percentage
                    TotalRoiAnnual = p.MarketRoiAnnual + p.DividendRoiAnnual |> Format.percentage
                })
        }

type ClosedPositionVM = {
    Name : string
    Isin : string
    Duration : string
    TotalProfit : string
    TotalRoi : string
    MarketProfitAnnual : string
    DividendProfitAnnual : string
    TotalProfitAnnual : string
    MarketRoiAnnual : string
    DividendRoiAnnual : string
    TotalRoiAnnual : string
}

type ClosedPositionsVM = {
    Currency : string

    TotalProfit : string
    TotalDividends : string

    Positions : ClosedPositionVM list
}

let listClosedPositions (depot:Depot.Api) = 
    let positions = depot.Get() |> PositionsInteractor.evaluateClosedPositions

    {
        Currency = "€"

        TotalProfit = positions |> Seq.sumBy(fun x -> x.MarketProfit) |> Format.currency
        TotalDividends = positions |> Seq.sumBy(fun x -> x.DividendProfit) |> Format.currency

        Positions = positions
            |> List.map(fun p -> 
                {            
                    Name = p.Position.Name
                    Isin = p.Position.AssetId |> Str.ofAssetId
                    Duration = p.Duration |> Format.timespan
                    TotalProfit = p.TotalProfit |> Format.currency
                    TotalRoi = p.TotalRoi |> Format.percentage
                    MarketProfitAnnual = p.MarketProfitAnnual |> Format.currency
                    DividendProfitAnnual = p.DividendProfitAnnual |> Format.currency
                    TotalProfitAnnual = p.MarketProfitAnnual + p.DividendProfitAnnual |> Format.currency
                    MarketRoiAnnual = p.MarketRoiAnnual |> Format.percentage
                    DividendRoiAnnual = p.DividendRoiAnnual |> Format.percentage
                    TotalRoiAnnual = p.MarketRoiAnnual + p.DividendRoiAnnual |> Format.percentage
                })
    }

type PositionTransactionVM = {
    Date : string
    Action : string
    Shares : string
    Price : string
    Value : string
}

type DividendsVM = {
    Date : string
    Value : string
}

type PositionDetailsVM = {
    Name : string
    Isin : string
    Shares : string
    Currency : string

    BuyingPrice : string
    BuyingValue : string
    CurrentPrice : string
    CurrentValue : string
    TotalProfit : string
    TotalRoi : string
    Transactions : PositionTransactionVM list

    TotalDividends : string
    DividendsRoi : string
    Dividends : DividendsVM list
}

let positionDetails (store:EventStore.Api) (depot:Depot.Api) broker lastPriceOf (assetId:string) = 
    // TODO: workaround - we need to send AssetId type to FE
    let position = 
        depot.Get() 
        |> Seq.tryFind(fun x -> x.AssetId = (AssetId.Isin assetId))
        |> Option.defaultWith(fun () -> 
            depot.Get() 
            |> Seq.tryFind(fun x -> x.AssetId = (AssetId.Coin assetId))
            |> Option.defaultWith(fun () -> depot.Get() |> Seq.find(fun x -> x.AssetId = (AssetId.Commodity assetId))))
    let evaluation = position |> PositionsInteractor.evaluate broker lastPriceOf

    let transactions = 
        store.Get()
        |> Seq.filter(fun x -> x |> DomainEvent.AssetId = Some position.AssetId)
        |> Seq.sortByDescending DomainEvent.Date
        |> List.ofSeq

    {
        Name = position.Name
        Isin = position.AssetId |> Str.ofAssetId
        Shares = position.Count |> Format.count position.AssetId
        Currency = "€"

        BuyingPrice = evaluation.BuyingPrice |> Format.currencyOpt
        BuyingValue = 
            if position.ClosedAt |> Option.isSome then
                // simplification: substract last sell to get remaining investment which was there before the sell
                let closeValue = 
                    transactions 
                    |> Seq.choose(function | AssetSold e -> e.Price * e.Count - e.Fee |> Some | _ -> None ) 
                    |> Seq.head
                evaluation.BuyingValue + closeValue
            else
                evaluation.BuyingValue
            |> Format.currency
        CurrentPrice = evaluation.CurrentPrice |> Format.currency
        CurrentValue = evaluation.CurrentValue |> Format.currency
        TotalProfit = evaluation.MarketProfit |> Format.currency
        TotalRoi = evaluation.MarketRoi |> Format.percentage
        Transactions = 
            transactions
            |> Seq.choose(function
                | AssetBought e -> 
                    {
                        Date = e.Date |> Format.date
                        Action = "Buy"
                        Shares = e.Count |> Format.count position.AssetId
                        Price = e.Price |> Format.currency
                        // effective value including fees
                        Value = e.Price * e.Count + e.Fee |> Format.currency
                    } |> Some
                | AssetSold e -> 
                    {
                        Date = e.Date |> Format.date
                        Action = "Sell"
                        Shares = e.Count |> Format.count position.AssetId
                        Price = e.Price |> Format.currency
                        // effective value including fees & taxes
                        Value = e.Price * e.Count - e.Fee |> Format.currency
                    } |> Some
                | DividendReceived _ | DepositAccounted _ | DisbursementAccounted _ | InterestReceived _ | AssetPriced _ -> None)
            |> List.ofSeq

        TotalDividends = evaluation.DividendProfit |> Format.currency
        DividendsRoi = evaluation.DividendRoi |> Format.percentage
        Dividends = 
            store.Get()
            |> Seq.filter(fun x -> x |> DomainEvent.AssetId = Some position.AssetId)
            |> Seq.sortByDescending DomainEvent.Date
            |> Seq.choose(function
                | DividendReceived e -> 
                    {
                        Date = e.Date |> Format.date
                        Value = e.Value - e.Fee |> Format.currency
                    } |> Some
                | AssetBought _ | AssetSold _  | DepositAccounted _  | DisbursementAccounted _ | InterestReceived _ | AssetPriced _ -> None)
            |> List.ofSeq
    }



type PortfolioPerformanceVM = {
    TotalDeposit : string
    TotalDisbursement : string
    TotalInvestment : string
    TotalCash : string
    TotalDividends : string
    CurrentPortfolioValue : string
    TotalValue : string
    TotalProfit : string
    CashLimit : string
    InvestingTime : string
}

let getPerformanceIndicators (store:EventStore.Api) (depot:Depot.Api) broker getCashLimit lastPriceOf = 
    depot.Get()
    |> PerformanceInteractor.getPerformance (store.Get()) getCashLimit lastPriceOf
    |> fun p -> 
        {
            TotalDeposit = p.TotalDeposit |> Format.currency
            TotalDisbursement = p.TotalDisbursement |> Format.currency
            TotalInvestment = p.TotalInvestment |> Format.currency
            TotalCash = p.TotalCash |> Format.currency
            TotalDividends = p.TotalDividends |> Format.currency
            CurrentPortfolioValue = p.CurrentPortfolioValue |> Format.currency
            TotalValue = p.TotalValue |> Format.currency
            TotalProfit = p.TotalProfit |> Format.currency
            CashLimit = p.CashLimit |> Format.currency
            InvestingTime = p.InvestingTime |> Format.timespan
        }

type AssetPerformanceVM = {
    TotalProfit : string
    TotalRoi : string
    TotalRoiAnnual : string
}

type AssetPerformanceRateVM = {
    Rate : string
    TotalProfit : string
    TotalRoi : string
    TotalRoiAnnual : string
}

type BenchmarkVM = {
    Name : string
    Isin : string
    BuyInstead : AssetPerformanceVM
    BuyPlan : AssetPerformanceRateVM
}

let getBenchmarkPerformance (store:EventStore.Api) broker savingsPlan (historicalPrices:PricesRepository.Api) (benchmark:Benchmark) = 
    benchmark
    |> BenchmarkInteractor.getBenchmarkPerformance store broker savingsPlan historicalPrices
    |> fun r -> 
        {
            Name = benchmark.Name
            Isin = benchmark.Isin |> Str.ofAssetId
            BuyInstead = {
                TotalProfit = r.BuyInstead.Profit |> Format.currency
                TotalRoi = r.BuyInstead.Roi |> Format.percentage
                TotalRoiAnnual = r.BuyInstead.RoiAnnual |> Format.percentage
            }
            BuyPlan = {
                Rate = savingsPlan.Rate |> Format.currency
                TotalProfit = r.BuyPlan.Profit |> Format.currency
                TotalRoi = r.BuyPlan.Roi |> Format.percentage
                TotalRoiAnnual = r.BuyPlan.RoiAnnual |> Format.percentage
            }
        }

type DiversificationVM = {
    Labels : string list
    Capital : decimal<Currency> list
}

let getDiversification (depot:Depot.Api) lastPriceOf = 
    depot.Get() 
    |> StatisticsInteractor.getDiversification lastPriceOf
    |> fun r ->
        let positions = r.Positions |> List.sortByDescending snd
        {
            Labels = positions |> List.map fst
            Capital = positions |> List.map snd
        }

type CashflowTransactionVM = {
    Date : string
    Type : string
    Comment : string
    Value : string
    Balance : string
}

let listCashflow (store:EventStore.Api) limit = 
    store.Get() 
    |> CashflowInteractor.getTransactions limit
    |> List.map(fun t ->
        {
            Date = t.Date |> Format.date
            Type = t.Type
            Comment = t.Comment
            Value = t.Value |> Format.currency
            Balance = t.Balance |> Format.currency
        })
        
