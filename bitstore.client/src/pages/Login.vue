<template>
  <div class="login-container">
    <form @submit.prevent="onLogin">
      <label for="username">User Name</label>
      <input id="username" v-model="userName" type="text" required />
      <button type="submit" :disabled="loading">{{ loading ? 'Logging in...' : 'Login' }}</button>
      <div v-if="error" class="error">{{ error }}</div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const router = useRouter();
const authStore = useAuthStore();

const userName = ref('');
const error = ref('');
const loading = ref(false);

async function onLogin() {
  if (!userName.value.trim()) {
    error.value = 'Username is required';
    return;
  }

  loading.value = true;
  error.value = '';

  try {
    const success = await authStore.login(userName.value);
    if (success) {
      router.push('/');
    } else {
      error.value = 'Login failed. Please try again.';
    }
  } catch (e) {
    error.value = 'An error occurred during login.';
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
.login-container {
  max-width: 400px;
  margin: 100px auto;
  padding: 2rem;
  border: 1px solid #ccc;
  border-radius: 8px;
  background: #fff;
}
label {
  display: block;
  margin-bottom: 0.5rem;
}
input {
  width: 100%;
  padding: 0.5rem;
  margin-bottom: 1rem;
}
button {
  width: 100%;
  padding: 0.5rem;
  cursor: pointer;
}
button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}
.error {
  color: red;
  margin-top: 1rem;
}
</style>
