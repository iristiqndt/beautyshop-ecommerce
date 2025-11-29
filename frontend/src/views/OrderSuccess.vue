<template>
  <div class="bg-gray-50 min-h-screen flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full bg-white rounded-lg shadow-xl p-8">
      <div class="text-center">
        <div class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-green-100">
          <svg class="h-6 w-6 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
          </svg>
        </div>
        
        <h2 class="mt-6 text-3xl font-extrabold text-gray-900">
          Order Placed Successfully!
        </h2>
        
        <p class="mt-2 text-sm text-gray-600">
          Thank you for your purchase. Your order is being processed.
        </p>
        
        <div v-if="orderNumber" class="mt-6 bg-gray-50 rounded-lg p-4">
          <p class="text-sm text-gray-500">Order Number</p>
          <p class="text-lg font-semibold text-gray-900">{{ orderNumber }}</p>
        </div>
        
        <p class="mt-4 text-sm text-gray-600">
          We've sent an order confirmation email to you. Please check your inbox.
        </p>
        
        <div class="mt-8 space-y-3">
          <router-link
            to="/orders"
            class="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            View My Orders
          </router-link>
          
          <router-link
            to="/products"
            class="w-full flex justify-center py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            Continue Shopping
          </router-link>
          
          <router-link
            to="/"
            class="w-full flex justify-center py-2 px-4 text-sm font-medium text-indigo-600 hover:text-indigo-500"
          >
            Về trang chủ
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const orderNumber = ref('')

onMounted(() => {
  orderNumber.value = route.params.orderNumber || ''
})
</script>
