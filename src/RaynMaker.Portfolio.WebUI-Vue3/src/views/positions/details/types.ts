
export type Details = {
    currency: string;
    shares: string;
    buyingPrice: string;
    buyingValue: string;
    currentPrice: string;
    currentValue: string;
    totalProfit: string;
    totalRoi: string;
    totalDividends: string;
    dividendsRoi: string;
    transactions: Record<string, string>[];
    dividends: Record<string, string>[];
}