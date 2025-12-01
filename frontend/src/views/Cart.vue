<template>
  <div class="bg-gray-50 min-h-screen">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <h1 class="text-3xl font-bold text-gray-900 mb-8">Your Shopping Cart</h1>

      <div v-if="loading" class="flex justify-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>

      <div v-else-if="cartStore.items.length === 0" class="text-center py-12 bg-white rounded-lg shadow">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900">Cart is Empty</h3>
        <p class="mt-1 text-sm text-gray-500">Add some products to your cart to continue shopping</p>
        <div class="mt-6">
          <router-link
            to="/products"
            class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700"
          >
            Continue Shopping
          </router-link>
        </div>
      </div>

      <div v-else class="lg:grid lg:grid-cols-12 lg:gap-x-8">
        <!-- Cart Items -->
        <div class="lg:col-span-7">
          <div class="bg-white rounded-lg shadow overflow-hidden">
            <ul class="divide-y divide-gray-200">
              <li v-for="item in cartStore.items" :key="item.id" class="p-6">
                <div class="flex">
                  <div class="flex-shrink-0">
                    <img
                      :src="item.imageUrl || 'https://placehold.co/150x150/e5e7eb/6b7280?text=No+Image'"
                      :alt="item.name"
                      class="w-24 h-24 rounded-md object-cover"
                    />
                  </div>

                  <div class="ml-6 flex-1">
                    <div class="flex justify-between">
                      <div>
                        <h3 class="text-lg font-medium text-gray-900">
                          <router-link :to="`/products/${item.productId}`" class="hover:text-indigo-600">
                            {{ item.productName }}
                          </router-link>
                        </h3>
                      </div>
                      <p class="text-lg font-medium text-gray-900">{{ formatPrice(item.price) }}</p>
                    </div>

                    <div class="mt-4 flex justify-between items-center">
                      <div class="flex items-center border border-gray-300 rounded-md">
                        <button
                          @click="updateQuantity(item.id, item.quantity - 1)"
                          class="px-3 py-1 text-gray-600 hover:text-gray-800"
                          :disabled="item.quantity <= 1"
                        >
                          -
                        </button>
                        <span class="px-4 py-1 border-x border-gray-300">{{ item.quantity }}</span>
                        <button
                          @click="updateQuantity(item.id, item.quantity + 1)"
                          class="px-3 py-1 text-gray-600 hover:text-gray-800"
                        >
                          +
                        </button>
                      </div>

                      <button
                        @click="removeItem(item.id)"
                        class="text-sm font-medium text-red-600 hover:text-red-500"
                      >
                        Remove
                      </button>
                    </div>
                  </div>
                </div>
              </li>
            </ul>
          </div>
        </div>

        <!-- Order Summary -->
        <div class="lg:col-span-5 mt-8 lg:mt-0">
          <div class="bg-white rounded-lg shadow p-6 sticky top-8">
            <h2 class="text-lg font-medium text-gray-900 mb-4">Order Summary</h2>

            <div class="space-y-4">
              <div class="flex justify-between text-base text-gray-900">
                <p>Subtotal</p>
                <p>{{ formatPrice(cartStore.subtotal) }}</p>
              </div>

              <div class="flex justify-between text-base text-gray-900">
                <p>Shipping Fee</p>
                <p>{{ formatPrice(1) }}</p>
              </div>

              <div class="border-t border-gray-200 pt-4 flex justify-between text-lg font-medium text-gray-900">
                <p>Total</p>
                <p>{{ formatPrice(cartStore.total + 1) }}</p>
              </div>
            </div>

            <div class="mt-6">
              <router-link
                to="/checkout"
                class="w-full bg-indigo-600 border border-transparent rounded-md shadow-sm py-3 px-4 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 flex justify-center"
              >
                Proceed to Checkout
              </router-link>
            </div>

            <div class="mt-6 text-center">
              <router-link to="/products" class="text-sm font-medium text-indigo-600 hover:text-indigo-500">
                Continue Shopping
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useCartStore } from '../stores/cart'

const cartStore = useCartStore()
const loading = ref(true)

const formatPrice = (price) => {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(price)
}

const updateQuantity = async (itemId, quantity) => {
  await cartStore.updateQuantity(itemId, quantity)
}

const removeItem = async (itemId) => {
  if (confirm('Are you sure you want to remove this product?')) {
    const result = await cartStore.removeFromCart(itemId)
    if (!result.success) {
      alert('Failed to remove item: ' + result.error)
    }
  }
}

onMounted(async () => {
  await cartStore.fetchCart()
  loading.value = false
})
</script>
