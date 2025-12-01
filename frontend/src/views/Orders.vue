<template>
  <div class="bg-gray-50 min-h-screen">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <h1 class="text-3xl font-bold text-gray-900 mb-8">My Orders</h1>

      <div v-if="loading" class="flex justify-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>

      <div v-else-if="orders.length === 0" class="text-center py-12 bg-white rounded-lg shadow">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900">No Orders</h3>
        <p class="mt-1 text-sm text-gray-500">You don't have any orders yet</p>
        <div class="mt-6">
          <router-link
            to="/products"
            class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700"
          >
            Start Shopping
          </router-link>
        </div>
      </div>

      <div v-else class="bg-white shadow overflow-hidden sm:rounded-md">
        <ul class="divide-y divide-gray-200">
          <li v-for="order in orders" :key="order.id">
            <router-link
              :to="`/orders/${order.id}`"
              class="block hover:bg-gray-50 transition"
            >
              <div class="px-6 py-4">
                <div class="flex items-center justify-between">
                  <div class="flex-1">
                    <div class="flex items-center justify-between">
                      <p class="text-sm font-medium text-indigo-600 truncate">
                        Order #{{ order.orderNumber }}
                      </p>
                      <div class="ml-2 flex-shrink-0">
                        <span
                          :class="[
                            'px-2 inline-flex text-xs leading-5 font-semibold rounded-full',
                            getStatusClass(order.status)
                          ]"
                        >
                          {{ getStatusText(order.status) }}
                        </span>
                      </div>
                    </div>
                    <div class="mt-2 flex justify-between">
                      <div class="sm:flex">
                        <p class="flex items-center text-sm text-gray-500">
                          {{ formatDate(order.createdAt) }}
                        </p>
                        <p class="mt-2 flex items-center text-sm text-gray-500 sm:mt-0 sm:ml-6">
                          {{ order.items?.length || 0 }} sản phẩm
                        </p>
                      </div>
                      <div class="text-right">
                        <p class="text-sm font-medium text-gray-900">
                          {{ formatPrice(order.totalAmount) }}
                        </p>
                      </div>
                    </div>
                  </div>
                  <div class="ml-5 flex-shrink-0">
                    <svg class="h-5 w-5 text-gray-400" viewBox="0 0 20 20" fill="currentColor">
                      <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                    </svg>
                  </div>
                </div>
              </div>
            </router-link>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const orders = ref([])
const loading = ref(true)

const formatPrice = (price) => {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(price)
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const getStatusClass = (status) => {
  const classes = {
    'Pending': 'bg-yellow-100 text-yellow-800',
    'Processing': 'bg-blue-100 text-blue-800',
    'Shipped': 'bg-purple-100 text-purple-800',
    'Delivered': 'bg-green-100 text-green-800',
    'Cancelled': 'bg-red-100 text-red-800'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

const getStatusText = (status) => {
  const texts = {
    'Pending': 'Pending',
    'Processing': 'Processing',
    'Shipped': 'Shipping',
    'Delivered': 'Delivered',
    'Cancelled': 'Cancelled'
  }
  return texts[status] || status
}

const loadOrders = async () => {
  loading.value = true
  try {
    const response = await api.get('/orders')
    orders.value = response.data
  } catch (error) {
    console.error('Error loading orders:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadOrders()
})
</script>
