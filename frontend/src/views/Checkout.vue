<template>
  <div class="bg-gray-50 min-h-screen">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <h1 class="text-3xl font-bold text-gray-900 mb-8">Checkout</h1>

      <div class="lg:grid lg:grid-cols-12 lg:gap-x-8">
        <!-- Checkout Form -->
        <div class="lg:col-span-7">
          <form @submit.prevent="handleCheckout" class="space-y-6">
            <!-- Contact Information -->
            <div class="bg-white rounded-lg shadow p-6">
              <h2 class="text-lg font-medium text-gray-900 mb-4">Contact Information</h2>
              
              <div class="space-y-4">
                <div>
                  <label for="fullName" class="block text-sm font-medium text-gray-700">Full Name</label>
                  <input
                    id="fullName"
                    v-model="form.fullName"
                    type="text"
                    required
                    class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>

                <div>
                  <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
                  <input
                    id="email"
                    v-model="form.email"
                    type="email"
                    required
                    class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>

                <div>
                  <label for="phoneNumber" class="block text-sm font-medium text-gray-700">Phone Number</label>
                  <input
                    id="phoneNumber"
                    v-model="form.phoneNumber"
                    type="tel"
                    required
                    class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>
              </div>
            </div>

            <!-- Shipping Address -->
            <div class="bg-white rounded-lg shadow p-6">
              <h2 class="text-lg font-medium text-gray-900 mb-4">Địa chỉ giao hàng</h2>
              
              <div>
                <label for="shippingAddress" class="block text-sm font-medium text-gray-700">Địa chỉ</label>
                <textarea
                  id="shippingAddress"
                  v-model="form.shippingAddress"
                  rows="3"
                  required
                  class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  placeholder="Số nhà, đường, phường/xã, quận/huyện, tỉnh/thành phố"
                ></textarea>
              </div>
            </div>

            <!-- Payment Method -->
            <div class="bg-white rounded-lg shadow p-6">
              <h2 class="text-lg font-medium text-gray-900 mb-4">Phương thức thanh toán</h2>
              
              <div class="space-y-4">
                <div class="flex items-center">
                  <input
                    id="cod"
                    v-model="form.paymentMethod"
                    type="radio"
                    value="COD"
                    class="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300"
                  />
                  <label for="cod" class="ml-3 block text-sm font-medium text-gray-700">
                    Thanh toán khi nhận hàng (COD)
                  </label>
                </div>

                <div class="flex items-center">
                  <input
                    id="stripe"
                    v-model="form.paymentMethod"
                    type="radio"
                    value="Stripe"
                    class="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300"
                  />
                  <label for="stripe" class="ml-3 flex items-center text-sm font-medium text-gray-700">
                    Thanh toán bằng thẻ Visa/Mastercard
                    <svg class="ml-2 h-8 w-auto" viewBox="0 0 48 32" xmlns="http://www.w3.org/2000/svg">
                      <rect width="48" height="32" rx="4" fill="#1434CB"/>
                      <text x="24" y="20" text-anchor="middle" fill="white" font-size="12" font-weight="bold">VISA</text>
                    </svg>
                  </label>
                </div>

                <div class="flex items-center">
                  <input
                    id="paypal"
                    v-model="form.paymentMethod"
                    type="radio"
                    value="PayPal"
                    class="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300"
                  />
                  <label for="paypal" class="ml-3 flex items-center text-sm font-medium text-gray-700">
                    Thanh toán qua PayPal
                    <svg class="ml-2 h-6 w-auto" viewBox="0 0 100 32" xmlns="http://www.w3.org/2000/svg">
                      <text x="0" y="24" fill="#003087" font-size="20" font-weight="bold">Pay</text>
                      <text x="35" y="24" fill="#009cde" font-size="20" font-weight="bold">Pal</text>
                    </svg>
                  </label>
                </div>
              </div>
            </div>

            <!-- Order Notes -->
            <div class="bg-white rounded-lg shadow p-6">
              <h2 class="text-lg font-medium text-gray-900 mb-4">Ghi chú đơn hàng</h2>
              
              <div>
                <textarea
                  v-model="form.notes"
                  rows="3"
                  class="block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  placeholder="Ghi chú về đơn hàng của bạn (tùy chọn)"
                ></textarea>
              </div>
            </div>

            <div v-if="error" class="rounded-md bg-red-50 p-4">
              <p class="text-sm text-red-800">{{ error }}</p>
            </div>
          </form>
        </div>

        <!-- Order Summary -->
        <div class="lg:col-span-5 mt-8 lg:mt-0">
          <div class="bg-white rounded-lg shadow p-6 sticky top-8">
            <h2 class="text-lg font-medium text-gray-900 mb-4">Đơn hàng của bạn</h2>

            <ul class="divide-y divide-gray-200 mb-4">
              <li v-for="item in cartStore.items" :key="item.id" class="py-4 flex">
                <img
                  :src="item.imageUrl || 'https://placehold.co/80x80/e5e7eb/6b7280?text=No+Image'"
                  :alt="item.productName"
                  class="w-16 h-16 rounded-md object-cover"
                />
                <div class="ml-4 flex-1">
                  <p class="text-sm font-medium text-gray-900">{{ item.productName }}</p>
                  <p class="text-sm text-gray-500">Số lượng: {{ item.quantity }}</p>
                  <p class="text-sm font-medium text-gray-900">{{ formatPrice(item.price * item.quantity) }}</p>
                </div>
              </li>
            </ul>

            <div class="border-t border-gray-200 pt-4 space-y-2">
              <div class="flex justify-between text-base text-gray-900">
                <p>Tạm tính</p>
                <p>{{ formatPrice(cartStore.subtotal) }}</p>
              </div>

              <div class="flex justify-between text-base text-gray-900">
                <p>Phí vận chuyển</p>
                <p>{{ formatPrice(shippingFee) }}</p>
              </div>

              <div class="border-t border-gray-200 pt-2 flex justify-between text-lg font-medium text-gray-900">
                <p>Tổng cộng</p>
                <p>{{ formatPrice(cartStore.total + shippingFee) }}</p>
              </div>
            </div>

            <div class="mt-6">
              <button
                @click="handleCheckout"
                :disabled="processing || cartStore.items.length === 0"
                class="w-full bg-indigo-600 border border-transparent rounded-md shadow-sm py-3 px-4 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
              >
                {{ processing ? 'Đang xử lý...' : 'Đặt hàng' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '../stores/cart'
import { useAuthStore } from '../stores/auth'
import api from '../services/api'

const router = useRouter()
const cartStore = useCartStore()
const authStore = useAuthStore()

const form = ref({
  fullName: '',
  email: '',
  phoneNumber: '',
  shippingAddress: '',
  paymentMethod: 'COD',
  notes: ''
})

const shippingFee = ref(30000)
const processing = ref(false)
const error = ref('')

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

const handleCheckout = async () => {
  processing.value = true
  error.value = ''

  try {
    const orderData = {
      recipientName: form.value.fullName,
      shippingAddress: form.value.shippingAddress,
      shippingCity: 'TP. Hồ Chí Minh',
      shippingPhone: form.value.phoneNumber,
      paymentMethod: form.value.paymentMethod,
      notes: form.value.notes
    }

    console.log('Creating order...')
    const response = await api.post('/orders/create', orderData)
    console.log('Order created successfully:', response.data)
    
    const orderId = response.data.id
    
    // If Stripe, redirect to Stripe checkout
    if (form.value.paymentMethod === 'Stripe') {
      const checkoutResponse = await api.post(`/orders/${orderId}/checkout`)
      if (checkoutResponse.data.url) {
        window.location.href = checkoutResponse.data.url
        return
      }
    } 
    // If PayPal, redirect to PayPal
    else if (form.value.paymentMethod === 'PayPal') {
      const paypalResponse = await api.post(`/orders/${orderId}/paypal`)
      if (paypalResponse.data.approvalUrl) {
        // Redirect directly to PayPal (return URL is already set in backend)
        window.location.href = paypalResponse.data.approvalUrl
        return
      }
    }
    
    // For COD, clear cart and redirect to success
    await cartStore.fetchCart()
    
    router.push({
      name: 'order-success',
      params: { orderNumber: response.data.orderNumber }
    })
  } catch (err) {
    console.error('Checkout error:', err)
    error.value = err.response?.data?.message || 'Đã có lỗi xảy ra. Vui lòng thử lại.'
  } finally {
    processing.value = false
  }
}

onMounted(async () => {
  await cartStore.fetchCart()
  
  // Pre-fill form with user data if logged in
  if (authStore.user) {
    form.value.fullName = authStore.user.fullName || ''
    form.value.email = authStore.user.email || ''
    form.value.phoneNumber = authStore.user.phoneNumber || ''
    form.value.shippingAddress = authStore.user.address || ''
  }
})
</script>
