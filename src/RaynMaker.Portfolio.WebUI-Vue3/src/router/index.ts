import { createRouter, createWebHistory } from 'vue-router'
import PositionsOpen from '../views/positions/open/PositionsOpen.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: PositionsOpen
    },
    {
      path: '/position-details/:isin/:name',
      name: 'PositionDetails',
      component: () => import('@/views/positions/details/PositionDetails.vue')
    },
    {
      path: '/cashflow',
      name: 'Cashflow',
      component: () => import('@/views/cashflow/Cashflow.vue')
    },
    {
      path: '/performance',
      name: 'Performance',
      component: () => import('@/views/performance/Performance.vue')
    },
    {
      path: '/history',
      name: 'History',
      //@ts-ignore:next-line
      component: () => import('@/views/positions/closed/ClosedPositions.vue')
    }
  ]
})

export default router
