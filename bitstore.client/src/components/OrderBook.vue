<template>
  <div class="order-book d-flex">
    <!-- Bids (Buy Orders) -->
    <div class="order-column me-4">
      <h6 class="text-center mb-3">Bids</h6>
      <div class="bars-container">
        <div class="bars-grid">
          <template v-for="(bid, index) in aggregatedBids" :key="'bid-' + index">
            <OrderBookBar
              type="bid"
              :price="bid.avgPrice"
              :amount="bid.totalAmount"
              :percentage="bid.percentage"
            />
          </template>
        </div>
      </div>
    </div>

    <!-- Asks (Sell Orders) -->
    <div class="order-column ms-4">
      <h6 class="text-center mb-3">Asks</h6>
      <div class="bars-container">
        <div class="bars-grid">
          <template v-for="(ask, index) in aggregatedAsks" :key="'ask-' + index">
            <OrderBookBar
              type="ask"
              :price="ask.avgPrice"
              :amount="ask.totalAmount"
              :percentage="ask.percentage"
            />
          </template>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { aggregateOrders } from '@/utils/orderBookUtils';
import OrderBookBar from './OrderBookBar.vue';

interface Props {
  bids: Array<{ price: number; amount: number }>;
  asks: Array<{ price: number; amount: number }>;
}

const props = defineProps<Props>();

const aggregatedBids = computed(() => {
  // Reverse bids to show in descending order
  return aggregateOrders(props.bids, 10);
});

const aggregatedAsks = computed(() => {
  return aggregateOrders(props.asks, 10);
});
</script>

<style scoped>
.order-book {
  padding: 1rem;
  justify-content: center;
}

.order-column {
  /*width: 400px;*/
}

.bars-container {
  min-height: 300px;
}

.bars-grid {
  display: flex;
  gap: 4px;
  align-items: end;
  height: 100%;
}
</style>
