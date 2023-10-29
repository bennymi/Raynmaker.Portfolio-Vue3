<script setup lang="ts">
import { computed } from 'vue'
import { Chart as ChartJS, ArcElement, Tooltip, Legend, Colors } from 'chart.js'
import { Pie } from 'vue-chartjs'
// @ts-ignore:next-line
import * as pl from '../utils/palette.js'

import type { PieChartProps } from './types'

const props = defineProps<PieChartProps>()

ChartJS.register(Colors, ArcElement, Tooltip, Legend)

const pieData = computed(() => {
    const { samples, labels } = props

    const backgroundColor = pl.palette('tol', Math.min(samples ? samples.length : 0, 12)).map((hex: string) => `#${hex}`)

    const datasets = [
        {
            data: samples,
            backgroundColor
        }
    ]

    return {
        labels,
        datasets
    }
})
</script>

<template>
    <div class="pie-chart">
        <Pie :data="pieData" :options="chartOptions" />
    </div>
</template>

<style scoped>
.pie-chart {
    width: 400px;
    height: 400px;
}
</style>
