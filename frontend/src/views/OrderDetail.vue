<template>
  <div class="bg-gray-50 min-h-screen">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div v-if="loading" class="flex justify-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>

      <div v-else-if="order" class="space-y-6">
        <!-- Header -->
        <div class="bg-white shadow rounded-lg p-6">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900">Order #{{ order.orderNumber }}</h1>
              <p class="mt-1 text-sm text-gray-500">Placed on {{ formatDate(order.createdAt) }}</p>
            </div>
            <span
              :class="[
                'px-3 py-1 inline-flex text-sm leading-5 font-semibold rounded-full',
                getStatusClass(order.status)
              ]"
            >
              {{ getStatusText(order.status) }}
            </span>
          </div>
        </div>

        <!-- Order Items -->
        <div class="bg-white shadow rounded-lg overflow-hidden">
          <div class="px-6 py-5 border-b border-gray-200">
            <h2 class="text-lg font-medium text-gray-900">Products</h2>
          </div>
          <ul class="divide-y divide-gray-200">
            <li v-for="item in order.items" :key="item.id" class="p-6">
              <div class="flex">
                <div class="flex-shrink-0">
                  <img
                    :src="item.product?.imageUrl || 'https://via.placeholder.com/150'"
                    :alt="item.product?.name"
                    class="w-20 h-20 rounded-md object-cover"
                  />
                </div>
                <div class="ml-6 flex-1">
                  <div class="flex items-start justify-between">
                    <div>
                      <h3 class="text-sm font-medium text-gray-900">{{ item.product?.name }}</h3>
                      <p class="mt-1 text-sm text-gray-500">Quantity: {{ item.quantity }}</p>
                    </div>
                    <p class="text-sm font-medium text-gray-900">{{ formatPrice(item.price * item.quantity) }}</p>
                  </div>
                </div>
              </div>
            </li>
          </ul>
        </div>

        <div class="lg:grid lg:grid-cols-2 lg:gap-x-6">
          <!-- Shipping Info -->
          <div class="bg-white shadow rounded-lg p-6">
            <h2 class="text-lg font-medium text-gray-900 mb-4">Shipping Information</h2>
            <dl class="space-y-2 text-sm">
              <div>
                <dt class="text-gray-500">Address</dt>
                <dd class="mt-1 text-gray-900">{{ order.shippingAddress }}</dd>
              </div>
              <div>
                <dt class="text-gray-500">Phone Number</dt>
                <dd class="mt-1 text-gray-900">{{ order.phoneNumber }}</dd>
              </div>
              <div v-if="order.notes">
                <dt class="text-gray-500">Notes</dt>
                <dd class="mt-1 text-gray-900">{{ order.notes }}</dd>
              </div>
            </dl>
          </div>

          <!-- Order Summary -->
          <div class="bg-white shadow rounded-lg p-6 mt-6 lg:mt-0">
            <h2 class="text-lg font-medium text-gray-900 mb-4">Order Summary</h2>
            <dl class="space-y-2 text-sm">
              <div class="flex justify-between">
                <dt class="text-gray-500">Subtotal</dt>
                <dd class="text-gray-900">{{ formatPrice(calculateSubtotal()) }}</dd>
              </div>
              <div class="flex justify-between">
                <dt class="text-gray-500">Shipping Fee</dt>
                <dd class="text-gray-900">{{ formatPrice(30000) }}</dd>
              </div>
              <div class="flex justify-between pt-2 border-t border-gray-200">
                <dt class="font-medium text-gray-900">Total</dt>
                <dd class="font-medium text-gray-900">{{ formatPrice(order.totalAmount) }}</dd>
              </div>
            </dl>
            
            <div class="mt-4 pt-4 border-t border-gray-200">
              <div class="flex justify-between text-sm">
                <dt class="text-gray-500">Payment Method</dt>
                <dd class="text-gray-900">{{ getPaymentMethodText(order.paymentMethod) }}</dd>
              </div>
            </div>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex justify-between">
          <router-link
            :to="backToOrdersRoute"
            class="inline-flex items-center px-4 py-2 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50"
          >
            ← Back to Orders
          </router-link>

          <button
            v-if="order.status === 'Pending'"
            @click="cancelOrder"
            :disabled="cancelling"
            class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 disabled:opacity-50"
          >
            {{ cancelling ? 'Cancelling...' : 'Cancel Order' }}
          </button>
        </div>
      </div>

      <div v-else class="text-center py-12 bg-white rounded-lg shadow">
        <p class="text-gray-500">Order not found</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import api from '../services/api'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const order = ref(null)
const loading = ref(true)
const cancelling = ref(false)

const backToOrdersRoute = computed(() => {
  return authStore.isAdmin ? '/admin/orders' : '/orders'
})

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
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
    'Pending': 'Chờ xử lý',
    'Processing': 'Đang xử lý',
    'Shipped': 'Đang giao',
    'Delivered': 'Đã giao',
    'Cancelled': 'Đã hủy'
  }
  return texts[status] || status
}

const getPaymentMethodText = (method) => {
  const methods = {
    'COD': 'Thanh toán khi nhận hàng',
    'Card': 'Thanh toán bằng thẻ',
    'Stripe': 'Thanh toán qua Stripe'
  }
  return methods[method] || method
}

const calculateSubtotal = () => {
  return order.value.items.reduce((sum, item) => sum + (item.price * item.quantity), 0)
}

const cancelOrder = async () => {
  if (!confirm('Bạn có chắc muốn hủy đơn hàng này?')) return

  cancelling.value = true
  try {
    await api.put(`/orders/${order.value.id}/cancel`)
    await loadOrder()
  } catch (error) {
    console.error('Error cancelling order:', error)
    alert('Không thể hủy đơn hàng. Vui lòng thử lại.')
  } finally {
    cancelling.value = false
  }
}

const loadOrder = async () => {
  loading.value = true
  try {
    const response = await api.get(`/orders/${route.params.id}`)
    order.value = response.data
  } catch (error) {
    console.error('Error loading order:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadOrder()
})
</script>
