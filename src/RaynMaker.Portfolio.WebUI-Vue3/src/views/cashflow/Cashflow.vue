<script setup lang="ts">
import API from '@/api'
import { ref, onMounted, watch } from 'vue'
import { CCard, CCardHeader, CCardTitle, CCardBody } from '@coreui/vue'
import type { Transaction } from './types'

const transactions = ref<Transaction[]>([])
const limit = ref<number>(25)

async function onLimitChanged() {
  if (limit.value > 0) {
    const response = await API.get(`/cashflow?limit=${limit.value}`)
    transactions.value = response.data
  }
}

onMounted(() => {
  onLimitChanged()
})

watch(limit, () => {
  onLimitChanged()
})
</script>

<template>
  <div>
    <CCard>
      <CCardHeader>
        <CCardTitle>Cashflow</CCardTitle>
      </CCardHeader>
      <CCardBody>
        <label>Limit: </label>
        <input label="Limit" v-model="limit" @keyup.enter="onLimitChanged" />

        <table class="table-hover">
          <thead>
            <tr>
              <th class="date">Date</th>
              <th class="comment">Type/Comment</th>
              <th class="value">Value</th>
              <th class="value">Balance</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="t in transactions"
              style="border-top-style: solid; border-top-width: 1px"
              v-bind:key="t.date + Math.random()"
            >
              <td class="date">
                {{ t.date }}
              </td>
              <td class="comment" style="padding-left: 100px; padding-right: 100px">
                <b>{{ t.type }}</b
                ><br />
                {{ t.comment }}
              </td>
              <td class="value">
                {{ t.value }}
              </td>
              <td class="value">
                {{ t.balance }}
              </td>
            </tr>
          </tbody>
        </table>
      </CCardBody>
    </CCard>
  </div>
</template>

<style scoped>
.date {
  text-align: left;
}

.value {
  text-align: right;
}

.comment {
  text-align: center;
}

th,
td {
  padding: 15px;
}

td {
  vertical-align: top;
}
</style>
