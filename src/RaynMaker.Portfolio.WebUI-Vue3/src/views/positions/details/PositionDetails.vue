<script setup lang="ts">
import API from '@/api'
import { useRoute } from 'vue-router'
import { CCardBody, CCard, CCardHeader, CCardTitle, CTable as CDataTable } from '@coreui/vue';
import { computed, ref, onMounted } from 'vue'
import type { Details } from './types'

const route = useRoute()

const data = ref<Details>()

onMounted(async () => {
	const response = await API.get(`/positionDetails?isin=${route.params.isin}`)
	data.value = response.data
})

const name = computed(() => route.params.name)
const isin = computed(() => route.params.isin)
const currency = computed(() => data?.value?.currency)
</script>

<template>
    <div>
        <CCard>
            <CCardHeader>
                <CCardTitle>{{ name }}</CCardTitle>
            </CCardHeader>
            <CCardBody>
                <table>
                    <tr>
                        <th>Isin:</th>
                        <td>{{ isin }}</td>
                    </tr>
                    <tr>
                        <th>Shares</th>
                        <td>{{ data?.shares }}</td>
                    </tr>
                    <tr>
                        <th style="padding-right: 20px">Buying Price / Buying Value</th>
                        <td>
                            {{ data?.buyingPrice }} {{ currency }} / {{ data?.buyingValue }}
                            {{ currency }}
                        </td>
                    </tr>
                    <tr>
                        <th>Price / Value</th>
                        <td>
                            {{ data?.currentPrice }} {{ currency }} / {{ data?.currentValue }}
                            {{ currency }}
                        </td>
                    </tr>
                    <tr>
                        <th>Profit</th>
                        <td>{{ data?.totalProfit }} {{ currency }} ({{ data?.totalRoi }} %)</td>
                    </tr>
                    <tr>
                        <th>Dividends</th>
                        <td>
                            {{ data?.totalDividends }} {{ currency }} ({{ data?.dividendsRoi }} %)
                        </td>
                    </tr>
                </table>
            </CCardBody>
        </CCard>

        <CCard :style="{ 'margin-top': '2rem' }">
            <CCardHeader>
                <CCardTitle>Transactions</CCardTitle>
            </CCardHeader>
            <CCardBody>
                <CDataTable
                    :items="data?.transactions"
                    column-filter
                    :responsive="false"
                    :items-per-page="250"
                    hover
                >
                    <template #price="{ item }">
                        <td>{{ item.price }} {{ currency }}</td>
                    </template>
                    <template #value="{ item }">
                        <td>{{ item.value }} {{ currency }}</td>
                    </template>
                </CDataTable>
            </CCardBody>
        </CCard>

        <CCard :style="{ 'margin-top': '2rem' }">
            <CCardHeader>
                <CCardTitle>Dividends</CCardTitle>
            </CCardHeader>
            <CCardBody>
                <CDataTable
                    :items="data?.dividends"
                    column-filter
                    :responsive="false"
                    :items-per-page="250"
                    hover
                >
                    <template #value="{ item }">
                        <td>{{ item.value }} {{ currency }}</td>
                    </template>
                </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>
