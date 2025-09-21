import { defineStore } from 'pinia';
import { API_BASE_URL } from '@/config';
import axios from 'axios';

interface AuthState {
  token: string;
  userName: string;
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: localStorage.getItem('token') || '',
    userName: localStorage.getItem('userName') || ''
  }),

  getters: {
    isAuthenticated(): boolean {
      return !!this.token;
    }
  },

  actions: {
    async login(userName: string): Promise<boolean> {
      try {
        console.log('Attempting login with:', { userName, url: `${API_BASE_URL}/auth/login` });

        const response = await axios.post(`${API_BASE_URL}/auth/login`, {
          UserName: userName
        }, {
          headers: {
            'Content-Type': 'application/json'
          }
        });
        const data = response.data;
        this.setToken(data.token);
        this.setUserName(userName);
        return true;
      } catch (error) {
        console.error('Login error:', error);
        return false;
      }
    },

    setToken(token: string) {
      this.token = token;
      localStorage.setItem('token', token);
    },

    setUserName(userName: string) {
      this.userName = userName;
      localStorage.setItem('userName', userName);
    },

    logout() {
      this.token = '';
      this.userName = '';
      localStorage.removeItem('token');
      localStorage.removeItem('userName');
    }
  }
});
