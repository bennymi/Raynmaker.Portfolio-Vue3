<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { CCardHeader, CCard, CCardBody, CCardTitle } from '@coreui/vue'
import API from '@/api'
import PieChart from '@/components/PieChart.vue'
// import PositionsGrid from './PositionsGrid.vue'

const capital = ref([])
const labels = ref([])
const positions = ref()
const summary = ref()
const filter = ref()

onMounted(async () => {
    let response = await API.get('/positions')

    console.log('response:', response.data);
    
    positions.value = response.data.positions
    summary.value = {
        totalInvestment: response.data.totalInvestment,
        currentValue: response.data.currentValue,
        totalProfit: response.data.totalProfit
    }

    response = await API.get('/diversification')
    capital.value = response.data.capital
    labels.value = response.data.labels
    console.log('diversification response:', response.data)
})

// export default {
//   data() {
//     return {
//       positions: [],
//       summary: {},
//       filter: '',
//       diversification: {
//         capital: null,
//         labels: null
//       }
//     }
//   },
//   components: {
//     'positions-grid': PositionsGrid,
//     'pie-chart': PieChart
//   },
//   async created() {
//     let response = await API.get('/positions')
//     this.positions = response.data.positions
//     this.summary = {
//       totalInvestment: response.data.totalInvestment,
//       currentValue: response.data.currentValue,
//       totalProfit: response.data.totalProfit
//     }

//     response = await API.get('/diversification')
//     this.diversification.capital = response.data.capital
//     this.diversification.labels = response.data.labels
//     console.log('diversification response:', response.data)
//   }
// }
</script>

<!-- <template>
    <h1>test</h1>
</template> -->

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

        <!-- <PositionsGrid
          :data="positions"
          :filter-key="filter"
          style="margin-top: 10px"
        ></PositionsGrid> -->
      </CCardBody>
    </CCard>

    <CCard>
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
