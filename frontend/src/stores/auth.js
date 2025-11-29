import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../services/api'

/**
 * Authentication store for managing user state and auth operations
 */
export const useAuthStore = defineStore('auth', () => {
  const user = ref(null)
  const token = ref(localStorage.getItem('token') || null)

  // Check if user is authenticated (has valid token)
  const isAuthenticated = computed(() => !!token.value)
  
  // Check if user is admin
  const isAdmin = computed(() => {
    return user.value?.role === 'Admin'
  })

  /**
   * Logs in user with email and password
   */
  async function login(email, password) {
    try {
      const response = await api.post('/auth/login', { email, password })
      token.value = response.data.token
      user.value = response.data
      localStorage.setItem('token', token.value)
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Login failed' }
    }
  }

  /**
   * Registers a new user account
   */
  async function register(userData) {
    try {
      const response = await api.post('/auth/register', userData)
      token.value = response.data.token
      user.value = response.data.user
      localStorage.setItem('token', token.value)
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Registration failed' }
    }
  }

  /**
   * Logs out current user
   */
  async function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
  }

  /**
   * Fetches current user information
   */
  async function fetchUser() {
    if (!token.value) return
    
    try {
      const response = await api.get('/auth/me')
      user.value = response.data
    } catch (error) {
      logout()
    }
  }

  /**
   * Changes password for authenticated user
   */
  async function changePassword(currentPassword, newPassword) {
    try {
      await api.post('/auth/change-password', { oldPassword: currentPassword, newPassword })
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Password change failed' }
    }
  }

  /**
   * Sends password reset email
   */
  async function forgotPassword(email) {
    try {
      await api.post('/auth/forgot-password', { email })
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Request failed' }
    }
  }

  /**
   * Resets password using reset token
   */
  async function resetPassword(token, newPassword) {
    try {
      await api.post('/auth/reset-password', { token, newPassword })
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Reset failed' }
    }
  }

  return {
    user,
    token,
    isAuthenticated,
    isAdmin,
    login,
    register,
    logout,
    fetchUser,
    changePassword,
    forgotPassword,
    resetPassword
  }
})
