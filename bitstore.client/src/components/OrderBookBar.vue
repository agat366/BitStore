<template>
  <div class="bar-wrapper-ex">
    <div class="amount text-center mb-1">{{ formatAmount(amount) }}</div>
    <div class="bar-wrapper d-flex justify-content-center">
      <div class="bar" :class="barTypeClass" :style="{ height: percentage + '%' }"></div>
    </div>
    <div class="price text-center mt-1">{{ formatPrice(price) }}</div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  type: 'bid' | 'ask';
  price: number;
  amount: number;
  percentage: number;
}

const props = defineProps<Props>();

const barTypeClass = computed(() => ({
  'bid-bar': props.type === 'bid',
  'ask-bar': props.type === 'ask'
}));

function formatPrice(price: number): string {
  return Math.round(price).toLocaleString().replace(/,/g, " ");
}

function formatAmount(amount: number): string {
  if (amount >= 1) {
    return Math.round(amount).toLocaleString().replace(/,/g, " ");
  }
  // For amounts less than 1, keep 6 decimal places
  return amount.toFixed(6).replace(/\B(?=(\d{3})+(?!\d))/g, " ");
}
</script>

<style scoped>
.bar-wrapper {
  height: 200px;
  align-items: flex-end;
}

.bar {
  width: 20px;
  transition: height 0.3s ease;
  border-radius: 2px 2px 0 0;
}

.bid-bar {
  background-color: rgba(40, 167, 69, 0.3);
  border: 1px solid rgba(40, 167, 69, 0.5);
}

.ask-bar {
  background-color: rgba(220, 53, 69, 0.3);
  border: 1px solid rgba(220, 53, 69, 0.5);
}

.amount {
  font-size: 0.5rem;
  opacity: 0.8;
  text-wrap: nowrap;
}

.price {
  font-size: 0.5rem;
  font-weight: 500;
  text-wrap: nowrap;
}
</style>
