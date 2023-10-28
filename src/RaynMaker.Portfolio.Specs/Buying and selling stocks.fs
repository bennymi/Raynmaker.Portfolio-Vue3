﻿namespace RaynMaker.Portfolio.Specs

open NUnit.Framework
open FsUnit
open RaynMaker.Portfolio.Entities
open RaynMaker.Portfolio

[<TestFixture>]
module ``Feature: Buying and selling stocks`` =
    [<Test>]
    let ``<Given> no existing position of a stock <When> buying 10 such stocks <Then> a new position is opened holding 10 stocks``() =
        [
            at 2014 01 01 |> buy "Joe Inc" (count 10) (price 50.0) (fee 1.0)
        ]
        |> TestAPI.getOwningStockCount "Joe Inc"
        |> should equal 10

    [<Test>]
    let ``<Given> no existing position of a stock <When> buying 10 such stocks for 50$ each <And> paying 7$ fee <Then> the investment in the position is 507$``() =
        [
            at 2014 01 01 |> buy "Joe Inc" (count 10) (price 50.0) (fee 7.0)
        ]
        |> TestAPI.getActiveInvestment "Joe Inc"
        |> should equal 507.0M<Currency>

    [<Test>]
    let ``<Given> an open position of 25 stocks <When> buying 10 more stocks <Then> the total owning stock count is 35``() =
        [
            at 2014 01 01 |> buy "Joe Inc" (count 20) (price 40.0) (fee 1.0)
            at 2014 03 01 |> sell "Joe Inc" (count 15) (price 45.0) (fee 1.0)
            at 2014 08 01 |> buy "Joe Inc" (count 20) (price 35.0) (fee 1.0)
            at 2015 01 01 |> buy "Joe Inc" (count 10) (price 50.0) (fee 1.0)
        ]
        |> TestAPI.getOwningStockCount "Joe Inc"
        |> should equal 35

    [<Test>]
    let ``<Given> a position with an investment of 1000$ <When> buying 10 more stocks for 50$ each <And> paying 7$ fee <Then> the position is an investment of 1507$``() =
        [
            at 2014 01 01 |> buy "Joe Inc" (count 20) (price 50.0) (fee 0.0)
            at 2015 01 01 |> buy "Joe Inc" (count 10) (price 50.0) (fee 7.0)
        ]
        |> TestAPI.getActiveInvestment "Joe Inc"
        |> should equal 1507.0M<Currency>

    [<Test>]
    let ``<Given> a position was once closed <When> buying again 10 stocks for 50$ each <And> paying 7$ fee <Then> the active investment is 507$``() =
        [
            at 2014 01 01 |> buy "Joe Inc" (count 20) (price 50.0) (fee 10.0)
            at 2014 01 01 |> sell "Joe Inc" (count 10) (price 60.0) (fee 1.0)
            at 2015 01 01 |> buy "Joe Inc" (count 10) (price 50.0) (fee 7.0) 
        ]
        |> TestAPI.getActiveInvestment "Joe Inc"
        |> should equal 507.0M<Currency>

    [<Test>]
    let ``<Given> 1000$ cash <When> buying 10 stocks for 50$ each <And> paying 7$ fee <Then> remaining cash is 493$``() =
        [
            at 2014 01 01 |> deposit (price 1000.0)
            at 2015 01 01 |> buy "Joe Inc" (count 10) (price 50.0) (fee 7.0)  
        ]
        |> TestAPI.getBalance
        |> should equal 493.0M<Currency>

    [<Test>]
    let ``<Given> an open position of 25 stocks <When> selling 10 stocks <Then> the total owning stock count is 15``() =
        [
            at 2014 01 01 |> buy "Joe Inc" (count 25) (price 40.0) (fee 1.0)
            at 2014 03 01 |> sell "Joe Inc" (count 5) (price 45.0) (fee 1.0)
            at 2014 06 01 |> sell "Joe Inc" (count 5) (price 50.0) (fee 1.0)
        ]
        |> TestAPI.getOwningStockCount "Joe Inc"
        |> should equal 15

    [<Test>]
    let ``<Given> a position with an investment of 1000$ <When> selling for 500$ <And> a fee of 9$ <Then> the position is an investment of 509$``() =
        [
            at 2014 01 01 |> buy "Joe Inc" (count 20) (price 50.0) (fee 0.0)
            at 2015 01 01 |> sell "Joe Inc" (count 10) (price 50.0) (fee 9.0)
        ]
        |> TestAPI.getActiveInvestment "Joe Inc"
        |> should equal 509.0M<Currency>

    [<Test>]
    let ``<Given> an active position <When> selling all stocks <Then> the position is closed``() =
        [
            at 2014 01 01 |> buy "Joe Inc" (count 20) (price 50.0) (fee 10.0)
            at 2014 01 01 |> sell "Joe Inc" (count 20) (price 60.0) (fee 11.0)
        ]
        |> TestAPI.getActiveInvestment "Joe Inc"
        |> should equal 0.0M<Currency>

    [<Test>]
    let ``<Given> 1000$ cash <When> selling 10 stocks for 50$ each <And> paying 7$ fee <Then> cash is 1493$``() =
        [
            at 2014 01 01 |> deposit (price 1500.0)
            at 2015 01 01 |> buy "Joe Inc" (count 10) (price 50.0) (fee 0.0)  
            at 2015 01 01 |> sell "Joe Inc" (count 10) (price 50.0) (fee 7.0)  
        ]
        |> TestAPI.getBalance
        |> should equal 1493.0M<Currency>

