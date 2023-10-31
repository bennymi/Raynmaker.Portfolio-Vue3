
export type Entry = Record<string, string | number> & {
    name: string;
    marketProfit: number;
    dividentProfit: number;
    totalProfit: number;
    marketRoi: number;
    dividendRoi: number;
    totalRoi: number;
    marketProfitAnnual: number;
    dividendProfitAnnual: number;
    totalProfitAnnual: number;
    marketRoiAnnual: number;
    dividendRoiAnnual: number;
    totalRoiAnnual: number;
}