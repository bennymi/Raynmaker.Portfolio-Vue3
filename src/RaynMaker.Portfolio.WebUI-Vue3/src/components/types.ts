

export type PieChartProps = {
    labels: string[];
    samples: number[];
    chartOptions?: any;
}

export type Dataset = {
    data: number[];
    backgroundColor: string[];
}

export type ChartData = {
    labels: string[];
    datasets: Dataset[];
}

export type PieData = {
    chartData: ChartData;
    chartOptions?: any;
}