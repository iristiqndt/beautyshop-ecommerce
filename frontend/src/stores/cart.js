import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../services/api'

/**
 * Cart store for managing shopping cart state and operations
 */
export const useCartStore = defineStore('cart', () => {
  const items = ref([])
  const loading = ref(false)

  // Calculate total number of items in cart
  const itemCount = computed(() => items.value.reduce((sum, item) => sum + item.quantity, 0))
  
  // Calculate subtotal (sum of all item prices * quantities)
  const subtotal = computed(() => items.value.reduce((sum, item) => sum + (item.price * item.quantity), 0))
  
  // Calculate total (same as subtotal, can be extended for taxes/shipping)
  const total = computed(() => subtotal.value)

  /**
   * Fetches cart items from API
   */
  async function fetchCart() {
    loading.value = true
    try {
      console.log('Fetching cart...')
      const response = await api.get('/cart')
      console.log('Cart API response:', response.data)
      items.value = response.data.items || response.data.Items || []
      console.log('Cart items set to:', items.value.length, 'items')
    } catch (error) {
      console.error('Failed to fetch cart:', error)
      items.value = []
    } finally {
      loading.value = false
    }
  }

  /**
   * Adds a product to the cart
   */
  async function addToCart(productId, quantity = 1) {
    try {
      const response = await api.post('/cart/items', { productId, quantity })
      await fetchCart()
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to add to cart' }
    }
  }

  /**
   * Updates the quantity of a cart item
   */
  async function updateQuantity(itemId, quantity) {
    try {
      await api.put(`/cart/items/${itemId}`, { quantity })
      await fetchCart()
      return { success: true }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to update quantity' }
    }
  }

  /**
   * Removes an item from the cart
   */
  async function removeFromCart(itemId) {
    try {
      await api.delete(`/cart/items/${itemId}`)
      await fetchCart()
      return { success: true }
    } catch (error) {
      console.error('Failed to remove from cart:', error)
      return { success: false, error: error.response?.data?.message || 'Failed to remove item' }
    }
  }

  async function clearCart() {
    try {
      await api.delete('/cart')
      items.value = []
      return { success: true }
    } catch (error) {
      console.error('Failed to clear cart:', error)
      return { success: false, error: error.response?.data?.message || 'Failed to clear cart' }
    }
  }

  return {
    items,
    loading,
    itemCount,
    subtotal,
    total,
    fetchCart,
    addToCart,
    updateQuantity,
    removeFromCart,
    clearCart
  }
})
