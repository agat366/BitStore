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
      <div class="card-body">
        <pre class="mb-0">{{ data }}</pre>
      </div>
    </div>
  </div>
</template><script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { API_BASE_URL } from '@/config'

const router = useRouter()
const authStore = useAuthStore()

const data = ref(null)
const loading = ref(false)
const error = ref('')

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

function logout() {
  authStore.logout()
  router.push('/login')
}

onMounted(() => {
  fetchData()
})
</script>
