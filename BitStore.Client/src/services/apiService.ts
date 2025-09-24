import axios from 'axios'
import { API_BASE_URL } from '@/config'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

export class ApiService {
  private authStore = useAuthStore()
  private router = useRouter()

  async getData() {
    return axios.get(`${API_BASE_URL}/data`, {
      headers: {
        'Authorization': `Bearer ${this.authStore.token}`
      }
    })
      .then(response => {
        if (response.status === 200) {
          return { success: true, data: response.data }
        } else {
          return { success: false, error: `Unexpected status: ${response.status}` }
        }
      })
      .catch(error => {
        if (error.code === "ERR_NETWORK") {
          this.authStore.logout()
          this.router.push('/login')
          return { success: false, error: 'Network error - logging out' }
        } else {
          return { success: false, error: `Data fetching error: ${error.message}` }
        }
      })
  }
}
