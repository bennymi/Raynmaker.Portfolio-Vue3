

export type PieChartProps = {
    labels: string[];
    samples: number[];
    chartOptions?: any;
}

export type Dataset = {
    data: number[];
    backgroundColor: string[];
}

export type PieData = {
    chartData: {
        labels: string[];
        datasets: Dataset[];
    };
    chartOptions?: any;
}