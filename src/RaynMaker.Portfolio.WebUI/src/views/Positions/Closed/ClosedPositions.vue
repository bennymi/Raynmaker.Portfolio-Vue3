<template>
  <div>
    <CCard>
      <CCardHeader>
        <CCardTitle>Sumamry</CCardTitle>
      </CCardHeader>
      <CCardBody>
        <table>
          <tr>
            <th>Profit</th>
            <td>{{ data.totalProfit }} {{data.currency}}</td>
          </tr>
          <tr>
            <th style="padding-right:20px">Dividends</th>
            <td>{{ data.totalDividends}} {{data.currency}}</td>
          </tr>
        </table>
      </CCardBody>
    </CCard>

    <CCard>
      <CCardHeader>
        <CCardTitle>Closed Positions</CCardTitle>
      </CCardHeader>
      <CCardBody>
        <form id="filter">
          <label style="margin-right: 10px">Filter: </label>
          <input name="query" v-model="filter">
        </form>

        <closed-positions-grid v-if="data.positions" :data="data.positions" :filter-key="filter" style="margin-top:10px"></closed-positions-grid>
      </CCardBody>
    </CCard>
  </div>
</template>

<script>
  import API from '@/api'
  import ClosedPositionsGrid from './ClosedPositionsGrid'

  export default {
    name: 'ClosedPositions',
    data () {
      return {
        data: {},
        filter: ''
      }
    },
    components: {
      'closed-positions-grid': ClosedPositionsGrid
    },
    async created () {
      const response = await API.get('/closedPositions')
      this.data = response.data
    }
  }
</script>
