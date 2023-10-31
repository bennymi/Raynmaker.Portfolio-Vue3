<script setup lang="ts">
import { computed, ref } from 'vue'
import type { Entry } from './types'

const props = defineProps<{ entries: Entry[]; filterKey: string }>()

const sortKey = ref<string>('name')

const sortOrders = ref<Record<string, number>>({
    name: 1,
    marketProfit: 1,
    dividentProfit: 1,
    totalProfit: 1,
    marketRoi: 1,
    dividendRoi: 1,
    totalRoi: 1,
    marketProfitAnnual: 1,
    dividendProfitAnnual: 1,
    totalProfitAnnual: 1,
    marketRoiAnnual: 1,
    dividendRoiAnnual: 1,
    totalRoiAnnual: 1
})

const filteredData = computed(() => {
    const filterString = props.filterKey.toLowerCase()
    const order: number = sortOrders.value[sortKey.value] || 1

    let data = props.entries

    if (filterString) {
        data = data?.filter((row) => {
            return Object.keys(row).some((key) => {
                return String(row[key]).toLowerCase().indexOf(filterString) > -1
            })
        })
    }

    if (sortKey.value) {
        data = data?.slice().sort(function (a, b) {
            const a2 = a[sortKey.value]
            const b2 = b[sortKey.value]
            return (a2 === b2 ? 0 : a2 > b2 ? 1 : -1) * order
        })
    }

    return data
})

function sortBy(key: string) {
	sortKey.value = key
	sortOrders.value[key] = sortOrders.value[key] * -1
}
</script>

<template>
    <table class="table table-bordered table-sm table-hover">
        <thead>
            <tr>
                <th @click="sortBy('name')" :class="{ sortedBy: sortKey == 'name' }">
                    Name
                    <span class="arrow" :class="sortOrders['name'] > 0 ? 'asc' : 'dsc'"></span>
                </th>
                <th colspan="3"></th>
                <th>Buying Price</th>
                <th>Price</th>
                <th
                    @click="sortBy('marketProfit')"
                    :class="{ sortedBy: sortKey == 'marketProfit' }"
                >
                    Profit
                    <span
                        class="arrow"
                        :class="sortOrders['marketProfit'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
                <th
                    @click="sortBy('dividendProfit')"
                    :class="{ sortedBy: sortKey == 'dividendProfit' }"
                >
                    Dividend
                    <span
                        class="arrow"
                        :class="sortOrders['dividendProfit'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
                <th @click="sortBy('totalProfit')" :class="{ sortedBy: sortKey == 'totalProfit' }">
                    Total
                    <span
                        class="arrow"
                        :class="sortOrders['totalProfit'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
                <th
                    @click="sortBy('marketProfitAnnual')"
                    :class="{ sortedBy: sortKey == 'marketProfitAnnual' }"
                >
                    Profit Annual
                    <span
                        class="arrow"
                        :class="sortOrders['marketProfitAnnual'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
                <th
                    @click="sortBy('dividendProfitAnnual')"
                    :class="{ sortedBy: sortKey == 'dividendProfitAnnual' }"
                >
                    Dividend Annual
                    <span
                        class="arrow"
                        :class="sortOrders['dividendProfitAnnual'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
                <th
                    @click="sortBy('totalProfitAnnual')"
                    :class="{ sortedBy: sortKey == 'totalProfitAnnual' }"
                >
                    Total Annual
                    <span
                        class="arrow"
                        :class="sortOrders['totalProfitAnnual'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
            </tr>
            <tr>
                <th>Isin</th>
                <th>Shares</th>
                <th>Duration</th>
                <th>Priced At</th>
                <th>Buying Value</th>
                <th>Value</th>
                <th @click="sortBy('marketRoi')" :class="{ sortedBy: sortKey == 'marketRoi' }">
                    %
                    <span class="arrow" :class="sortOrders['marketRoi'] > 0 ? 'asc' : 'dsc'"></span>
                </th>
                <th @click="sortBy('dividendRoi')" :class="{ sortedBy: sortKey == 'dividendRoi' }">
                    %
                    <span
                        class="arrow"
                        :class="sortOrders['dividendRoi'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
                <th @click="sortBy('totalRoi')" :class="{ sortedBy: sortKey == 'totalRoi' }">
                    %
                    <span class="arrow" :class="sortOrders['totalRoi'] > 0 ? 'asc' : 'dsc'"></span>
                </th>
                <th
                    @click="sortBy('marketRoiAnnual')"
                    :class="{ sortedBy: sortKey == 'marketRoiAnnual' }"
                >
                    %
                    <span
                        class="arrow"
                        :class="sortOrders['marketRoiAnnual'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
                <th
                    @click="sortBy('dividendRoiAnnual')"
                    :class="{ sortedBy: sortKey == 'dividendRoiAnnual' }"
                >
                    %
                    <span
                        class="arrow"
                        :class="sortOrders['dividendRoiAnnual'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
                <th
                    @click="sortBy('totalRoiAnnual')"
                    :class="{ sortedBy: sortKey == 'totalRoiAnnual' }"
                >
                    %
                    <span
                        class="arrow"
                        :class="sortOrders['totalRoiAnnual'] > 0 ? 'asc' : 'dsc'"
                    ></span>
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="entry in filteredData" v-bind:key="entry.isin">
                <td>
                    <router-link
                        :to="{
                            name: 'PositionDetails',
                            params: { isin: entry.isin, name: entry.name }
                        }"
                        >{{ entry.name }}</router-link
                    >
                    <br />
                    {{ entry.isin }}
                </td>
                <td>{{ entry.shares }}</td>
                <td>{{ entry.duration }}</td>
                <td>{{ entry.pricedAt }}</td>
                <td>
                    {{ entry.buyingPrice }} <br />
                    {{ entry.buyingValue }}
                </td>
                <td>
                    {{ entry.currentPrice }} <br />
                    {{ entry.currentValue }}
                </td>
                <td>
                    {{ entry.marketProfit }} <br />
                    {{ entry.marketRoi }}
                </td>
                <td>
                    {{ entry.dividendProfit }} <br />
                    {{ entry.dividendRoi }}
                </td>
                <td :class="entry.totalProfit > 0 ? 'win' : 'loss'">
                    <b>
                        {{ entry.totalProfit }} <br />
                        {{ entry.totalRoi }}
                    </b>
                </td>
                <td>
                    {{ entry.marketProfitAnnual }} <br />
                    {{ entry.marketRoiAnnual }}
                </td>
                <td>
                    {{ entry.dividendProfitAnnual }} <br />
                    {{ entry.dividendRoiAnnual }}
                </td>
                <td :class="entry.totalProfitAnnual > 0 ? 'win' : 'loss'">
                    <b>
                        {{ entry.totalProfitAnnual }} <br />
                        {{ entry.totalRoiAnnual }}
                    </b>
                </td>
            </tr>
        </tbody>
    </table>
</template>
