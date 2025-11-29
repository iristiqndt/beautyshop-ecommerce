<template>
  <div class="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-md">
      <div v-if="loading" class="text-center">
        <div class="animate-spin rounded-full h-16 w-16 border-b-2 border-indigo-600 mx-auto"></div>
        <p class="mt-4 text-lg text-gray-700">Processing payment...</p>
      </div>

      <div v-else-if="success" class="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
        <div class="text-center">
          <svg class="mx-auto h-16 w-16 text-green-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <h2 class="mt-4 text-2xl font-bold text-gray-900">Payment Successful!</h2>
          <p class="mt-2 text-gray-600">Your order has been confirmed.</p>
          <p v-if="orderNumber" class="mt-4 text-lg font-medium text-gray-900">
            Order Number: <span class="text-indigo-600">{{ orderNumber }}</span>
          </p>
          <div class="mt-6 space-y-3">
            <router-link
              :to="`/orders/${orderId}`"
              class="block w-full bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
            >
              View Order Details
            </router-link>
            <router-link
              to="/products"
              class="block w-full bg-gray-200 text-gray-700 px-4 py-2 rounded-md hover:bg-gray-300"
            >
              Continue Shopping
            </router-link>
          </div>
        </div>
      </div>

      <div v-else class="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
        <div class="text-center">
          <svg class="mx-auto h-16 w-16 text-red-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <h2 class="mt-4 text-2xl font-bold text-gray-900">Payment Failed</h2>
          <p class="mt-2 text-gray-600">{{ error }}</p>
          <div class="mt-6">
            <router-link
              to="/cart"
              class="block w-full bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
            >
              Return to Cart
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../services/api'

const route = useRoute()
const router = useRouter()

const loading = ref(true)
const success = ref(false)
const error = ref('')
const orderNumber = ref('')
const orderId = ref(null)

onMounted(async () => {
  const paypalOrderId = route.query.token
  const orderIdParam = route.query.orderId
  
  if (paypalOrderId && orderIdParam) {
    // PayPal payment
    try {
      const response = await api.post('/orders/paypal/capture', {
        payPalOrderId: paypalOrderId,
        orderId: parseInt(orderIdParam)
      })
      
      if (response.data.success) {
        success.value = true
        orderNumber.value = response.data.orderNumber
        orderId.value = orderIdParam
      } else {
        error.value = 'Payment verification failed'
      }
    } catch (err) {
      error.value = err.response?.data?.message || 'Payment processing failed'
    }
  } else {
    // Stripe payment or other
    const sessionId = route.query.session_id
    if (sessionId) {
      // Handle Stripe success
      success.value = true
      // You can verify with backend if needed
    } else {
      error.value = 'Invalid payment session'
    }
  }
  
  loading.value = false
})
</script>
