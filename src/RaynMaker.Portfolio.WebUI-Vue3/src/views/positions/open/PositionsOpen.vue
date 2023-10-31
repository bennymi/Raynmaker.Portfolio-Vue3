<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { CCardHeader, CCard, CCardBody, CCardTitle } from '@coreui/vue'
import API from '@/api'
import PieChart from '@/components/PieChart.vue'
import PositionsGrid from './PositionsGrid.vue'

import type { Entry } from './types'

const capital = ref([])
const labels = ref<string[]>([])
const positions = ref<Entry[]>([])
const summary = ref<Record<string, number>>({})
const filter = ref('')

onMounted(async () => {
    let response = await API.get('/positions')
    
    positions.value = response.data.positions
    summary.value = {
        totalInvestment: response.data.totalInvestment,
        currentValue: response.data.currentValue,
        totalProfit: response.data.totalProfit
    }

    console.log('response:',  response.data.positions)

    response = await API.get('/diversification')
    capital.value = response.data.capital
    labels.value = response.data.labels
})
</script>

<template>
  <div>
    <CCard>
      <CCardHeader>
        <CCardTitle>Positions</CCardTitle>
      </CCardHeader>
      <CCardBody>
        <form id="filter">
          <label style="margin-right: 10px">Filter: </label>
          <input name="query" v-model="filter" />
          <span style="float: right">
            <label>Total investment: </label> {{ summary?.totalInvestment }}
            <span style="margin-left: 20px"></span>
            <label>Current Value: </label> {{ summary?.currentValue }}
            <span style="margin-left: 20px"></span>
            <label>Total profit: </label> {{ summary?.totalProfit }}
          </span>
        </form>

        <PositionsGrid
          :entries="positions"
          :filterKey="filter"
        />
      </CCardBody>
    </CCard>

    <CCard :style="{ 'margin-top': '2rem' }">
      <CCardHeader>
        <CCardTitle>Diversification</CCardTitle>
      </CCardHeader>
      <CCardBody>
        <p>Current value</p>
        <PieChart
          :labels="labels"
          :samples="capital"
          :chartOptions="{ responsive: true }"
        ></PieChart>
      </CCardBody>
    </CCard>
  </div>
</template>

<style>
label {
    font-weight: bold;
}
</style>
