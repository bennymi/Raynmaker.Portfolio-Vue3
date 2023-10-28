
- in table sorting by percentage does not work

- computation of market profit does not fit to computation 
  of buying price if sell and rebuy transactions happened
  - add tests
  - we could separately show "current profit" based on "remaining investment"
    additionally to already "cashed out" money and "total profit"
  - OR we could keep "total profit" calculation but then change way of
    "buying price" computation


- in VM
  - rename "shares" -> Count
  - rename "isin" -> assetid
  - how can we diff in UI whether it is isin or coin? on controller request we have to diff

- support for multiple brokers (diba + binance)
  - or should we just ignore the broker when simulating sell?
  - we also ignore taxes ...
  - we could later added as optional feature but then we would also have to ask for taxes

- separate events for
  - "wertpapiereinang"
  - "Kapitalmassnahme"

- how to handle tax corrections in a proper way?, e.g.:

  // in case of tax corrections dividends could be zero temporarily
  // TODO: we should probably have dedicated event for that
  //Contract.ensures (fun () -> newP.Dividends > 0.0M<Currency>) "new Dividends > 0"

=====================

- find a way to automatically get the prices we need
  - add a service which collects in background and saves in store
    - e.g. udpate csv file and send message to PricesRepository to refresh
  - e.g.: http://www.google.com/finance/historical?q=MSFT&output=csv
  
=====================

- Effective Yield
  - charts to visualize change over time - a good "average" can be found later
  - absolute win/loss per month based on capital at start and of month
  - percentage win/loss per month
  - on "performance" view
  - how to handle "disbursements"?
  - precond:
    - prices of bought stocks at month start and end in correct currency
    - currency service if prices not available in wanted currency
  
=====================


- improve TestAPI and test to be based on Controller/Presenter
  - TestAPI only bridges between test and SUT
  - TDK provides convenience functions

- Specs: restructure features more towards end user features
  - remove "given-when-then" from scenario description

- fix red tests

- add missing tests

- buying price is not correctly calculated when some parts of the position got sold

=====================

- restructure backlog
  - road map?
  - issues?

- migrate "shell" to electron

- total return per year = (total return + 1)^(1/years)-1

- move "formatValue" into controller/presenter
  - but then sorting is difficult ...

- for router link highlighting see vue cookbook page 292

- benchmark
  - annual fee not considered
    // TODO: annual fee
    // -> what about just inventing an event because it could then be calculated when walking the positions
  - how do we hande cash in general in the system?

- over all performance
  - current implementation is too naive (we cannot just substract disbursement - we could have invested it in between ...)
  - TWR
  - MWR

- keep the position details
  - at least by and sell activities
  - would also be easier to "get a last price"

# References 

- http://tidyjava.com/clean-architecture-screaming/
- https://github.com/cleancoders/CleanCodeCaseStudy/tree/master/src/cleancoderscom

