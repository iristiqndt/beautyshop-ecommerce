import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '../services/api'

export const useProductStore = defineStore('products', () => {
  const products = ref([])
  const categories = ref([])
  const currentProduct = ref(null)
  const loading = ref(false)

  async function fetchProducts(params = {}) {
    loading.value = true
    try {
      const response = await api.get('/products', { params })
      products.value = response.data
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to fetch products' }
    } finally {
      loading.value = false
    }
  }

  async function fetchProductById(id) {
    loading.value = true
    try {
      const response = await api.get(`/products/${id}`)
      currentProduct.value = response.data
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to fetch product' }
    } finally {
      loading.value = false
    }
  }

  async function fetchCategories() {
    try {
      const response = await api.get('/categories')
      categories.value = response.data
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to fetch categories' }
    }
  }

  async function fetchProductsByCategory(categorySlug) {
    loading.value = true
    try {
      const response = await api.get(`/products/category/${categorySlug}`)
      products.value = response.data
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to fetch products' }
    } finally {
      loading.value = false
    }
  }

  async function searchProducts(query) {
    loading.value = true
    try {
      const response = await api.get('/products/search', { params: { q: query } })
      products.value = response.data
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Search failed' }
    } finally {
      loading.value = false
    }
  }

  return {
    products,
    categories,
    currentProduct,
    loading,
    fetchProducts,
    fetchProductById,
    fetchCategories,
    fetchProductsByCategory,
    searchProducts
  }
})
