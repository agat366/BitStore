<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1 class="h2">Welcome {{ authStore.userName }}!</h1>
      <button
        @click="logout"
        class="btn btn-danger">
        Logout
      </button>
    </div>

    <div v-if="loading" class="text-secondary">
      Loading data...
    </div>
    <div v-else-if="error" class="alert alert-danger">
      {{ error }}
    </div>
    <div v-else class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <span>Order Book</span>
        <div class="d-flex align-items-center">
          <div class="countdown-spinner me-3" v-if="!loading">
            <svg width="20" height="20" viewBox="0 0 20 20">
              <circle
                cx="10"
                cy="10"
                r="8"
                fill="none"
                stroke="#dee2e6"
                stroke-width="2"
              />
              <circle
                cx="10"
                cy="10"
                r="8"
                fill="none"
                stroke="#6c757d"
                stroke-width="2"
                stroke-dasharray="50.26548245743669"
                :stroke-dashoffset="spinnerOffset"
                transform="rotate(-90 10 10)"
                class="spinner-circle"
              />
            </svg>
          </div>
          <div v-else class="spinner-border spinner-border-sm text-secondary me-3" role="status">
            <span class="visually-hidden">Loading...</span>
          </div>
          <span class="text-muted">{{ formatTimestamp(data?.timestamp) }}</span>
        </div>
      </div>
      <div class="card-body">
        <OrderBook
          :bids="data?.bids || []"
          :asks="data?.asks || []"
        />
      </div>
      <div class="card-footer">
        <div class="row align-items-center">
          <!-- Trade Type Selection -->
          <div class="col-auto">
            <div class="btn-group-vertical" role="group">
              <input type="radio" class="btn-check" name="tradeType" id="buyOption" value="buy"
                     v-model="tradeType" checked>
              <label class="btn btn-outline-success" for="buyOption">Buy</label>

              <input type="radio" class="btn-check" name="tradeType" id="sellOption" value="sell"
                     v-model="tradeType">
              <label class="btn btn-outline-danger" for="sellOption">Sell</label>
            </div>
          </div>

          <!-- Amount Input -->
          <div class="col-4">
            <div class="input-group">
              <input
                type="number"
                class="form-control"
                :class="{
                  'border-success text-success': tradeType === 'buy',
                  'border-danger text-danger': tradeType === 'sell'
                }"
                v-model="amount"
                placeholder="Enter BTC amount"
                step="0.1"
                min="0"
                @input="calculateTotal"
              >
              <span class="input-group-text">BTC</span>
            </div>
          </div>

          <!-- Total Cost Display -->
          <div class="col-auto ms-3">
            <div class="h5 mb-0">
              <span class="text-muted">Total:</span>
              <span :class="{
                'text-success': tradeType === 'buy',
                'text-danger': tradeType === 'sell'
              }">
                {{ formatPrice(totalCost) }} USD
                <span class="text-muted ms-2" v-if="totalCost > 0">
                  ({{ formatPrice(avgPrice) }} per BTC)
                </span>
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { API_BASE_URL } from '@/config'
import OrderBook from '@/components/OrderBook.vue'

type Timer = ReturnType<typeof setInterval>

interface Order {
  price: number
  amount: number
}

const router = useRouter()
const authStore = useAuthStore()

const data = ref(null)
const loading = ref(false)
const error = ref('')
const timeToRefresh = ref(10)
const spinnerOffset = ref(0)
let refreshTimer: Timer | null = null
let spinnerInterval: Timer | null = null
const tradeType = ref('buy')
const amount = ref('')
const totalCost = ref(0)
const avgPrice = ref(0)

// Calculate total cost based on order book
function calculateTotal() {
  const amountNum = parseFloat(amount.value)
  if (isNaN(amountNum) || amountNum <= 0) {
    totalCost.value = 0
    avgPrice.value = 0
    return
  }

  const orders = tradeType.value === 'buy' ? data.value?.asks : data.value?.bids
  if (!orders?.length) {
    totalCost.value = 0
    return
  }

  let remainingAmount = amountNum
  let total = 0

  for (const order of orders) {
    const orderAmount = Math.min(remainingAmount, order.amount)
    total += orderAmount * order.price
    remainingAmount -= orderAmount

    if (remainingAmount <= 0) break
  }

  if (remainingAmount > 0) {
    // Not enough liquidity
    totalCost.value = 0
    avgPrice.value = 0
    return
  }

  totalCost.value = total
  avgPrice.value = total / amountNum
}

function formatPrice(price: number): string {
  return !price ? '0' : Math.round(price).toLocaleString().replace(/,/g, ' ')
}

async function fetchData() {
  loading.value = true
  error.value = ''

  try {
    const response = await fetch(`${API_BASE_URL}/data`, {
      headers: {
        'Authorization': `Bearer ${authStore.token}`
      }
    })

    if (!response.ok) {
      throw new Error('Failed to fetch data')
    }

    data.value = await response.json()
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'An error occurred'
  } finally {
    loading.value = false
  }
}

function formatTimestamp(timestamp: string): string {
  if (!timestamp) return '';
  return new Date(timestamp).toLocaleString();
}

function startRefreshTimer() {
  timeToRefresh.value = 10
  updateSpinnerOffset()

  if (refreshTimer) clearInterval(refreshTimer)
  if (spinnerInterval) clearInterval(spinnerInterval)

  refreshTimer = setInterval(() => {
    fetchData()
    timeToRefresh.value = 10
  }, 10000)

  spinnerInterval = setInterval(() => {
    timeToRefresh.value = Math.max(0, timeToRefresh.value - 0.1)
    updateSpinnerOffset()
  }, 100)
}

function updateSpinnerOffset() {
  const progress = timeToRefresh.value / 10
  spinnerOffset.value = 50.26548245743669 * (1 - progress)
}

function logout() {
  if (refreshTimer) clearInterval(refreshTimer)
  if (spinnerInterval) clearInterval(spinnerInterval)
  authStore.logout()
  router.push('/login')
}

onMounted(() => {
  fetchData()
  startRefreshTimer()
})

onUnmounted(() => {
  if (refreshTimer) clearInterval(refreshTimer)
  if (spinnerInterval) clearInterval(spinnerInterval)
})

// Recalculate total when trade type changes
watch(tradeType, calculateTotal)
</script>
