
export type Performance = {
    investingTime: string;
    cashLimit: string;
    totalDeposit: string;
    totalDisbursement: string;
    totalInvestment: string;
    totalCash: string;
    totalDividends: string;
    currentPortfolioValue: string;
    totalValue: string;
    totalProfit: string;
}

type BuyInstead = {
    totalProfit: string;
    totalRoi: string;
    totalRoiAnnual: string;
}

type BuyPlan = {
    rate: string;
    totalProfit: string;
    totalRoi: string;
    totalRoiAnnual: string;
}

export type Benchmark = {
    name: string;
    isin: string;
    buyInstead: BuyInstead;
    buyPlan: BuyPlan;
}