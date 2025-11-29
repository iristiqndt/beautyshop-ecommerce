<template>
  <div class="group relative">
    <div class="w-full aspect-w-1 aspect-h-1 bg-gray-200 rounded-lg overflow-hidden">
      <img
        :src="product.imageUrl || 'https://via.placeholder.com/300'"
        :alt="product.name"
        class="w-full h-64 object-center object-cover group-hover:opacity-75 transition"
      />
    </div>
    <div class="mt-4">
      <div>
        <h3 class="text-sm text-gray-700">
          <router-link :to="`/products/${product.id}`">
            <span class="absolute inset-0" />
            {{ product.name }}
          </router-link>
        </h3>
        <p v-if="product.brand" class="mt-1 text-sm text-gray-500">{{ product.brand }}</p>
      </div>
      <div class="mt-2 flex items-center justify-between">
        <p class="text-lg font-medium text-gray-900">{{ formatPrice(product.price) }}</p>
        <span
          v-if="product.stockQuantity > 0"
          class="text-xs text-green-600"
        >
          Còn hàng
        </span>
        <span v-else class="text-xs text-red-600">
          Hết hàng
        </span>
      </div>
    </div>
    <div v-if="!authStore.isAdmin" class="mt-4">
      <button
        @click.prevent="addToCart"
        :disabled="product.stockQuantity === 0 || adding"
        class="w-full bg-indigo-600 border border-transparent rounded-md py-2 px-4 flex items-center justify-center text-sm font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
      >
        {{ adding ? 'Đang thêm...' : 'Thêm vào giỏ' }}
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useCartStore } from '../stores/cart'
import { useAuthStore } from '../stores/auth'
import { useRouter } from 'vue-router'

const props = defineProps({
  product: {
    type: Object,
    required: true
  }
})

const cartStore = useCartStore()
const authStore = useAuthStore()
const router = useRouter()
const adding = ref(false)

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

const addToCart = async () => {
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }

  adding.value = true
  await cartStore.addToCart(props.product.id, 1)
  adding.value = false
}
</script>
