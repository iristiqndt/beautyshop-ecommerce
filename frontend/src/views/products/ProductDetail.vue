<template>
  <div class="bg-white">
    <div v-if="loading" class="flex justify-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
    </div>
    
    <div v-else-if="product" class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Breadcrumb -->
      <nav class="flex mb-8" aria-label="Breadcrumb">
        <ol class="flex items-center space-x-4">
          <li>
            <router-link to="/" class="text-gray-400 hover:text-gray-500">Trang chủ</router-link>
          </li>
          <li>
            <span class="text-gray-400">/</span>
          </li>
          <li>
            <router-link to="/products" class="text-gray-400 hover:text-gray-500">Sản phẩm</router-link>
          </li>
          <li>
            <span class="text-gray-400">/</span>
          </li>
          <li class="text-gray-900">{{ product.name }}</li>
        </ol>
      </nav>

      <!-- Product Detail -->
      <div class="lg:grid lg:grid-cols-2 lg:gap-x-8 lg:items-start">
        <!-- Image -->
        <div class="flex flex-col">
          <div class="w-full aspect-w-1 aspect-h-1 rounded-lg overflow-hidden">
            <img
              :src="product.imageUrl || 'https://placehold.co/600x600/e5e7eb/6b7280?text=No+Image'"
              :alt="product.name"
              class="w-full h-full object-center object-cover"
            />
          </div>
        </div>

        <!-- Product Info -->
        <div class="mt-10 px-4 sm:px-0 sm:mt-16 lg:mt-0">
          <h1 class="text-3xl font-extrabold tracking-tight text-gray-900">{{ product.name }}</h1>
          
          <div class="mt-3">
            <h2 class="sr-only">Thông tin sản phẩm</h2>
            <p class="text-3xl text-gray-900">{{ formatPrice(product.price) }}</p>
          </div>

          <div v-if="product.brand" class="mt-4">
            <p class="text-sm text-gray-500">Thương hiệu: <span class="font-medium text-gray-900">{{ product.brand }}</span></p>
          </div>

          <div class="mt-6">
            <h3 class="sr-only">Mô tả</h3>
            <p class="text-base text-gray-700">{{ product.description }}</p>
          </div>

          <div class="mt-6">
            <div class="flex items-center">
              <span class="text-sm text-gray-500">Status:</span>
              <span :class="[product.stockQuantity > 0 ? 'text-green-600' : 'text-red-600', 'ml-2 font-medium']">
                {{ product.stockQuantity > 0 ? `${product.stockQuantity} in stock` : 'Out of stock' }}
              </span>
            </div>
          </div>

          <!-- Quantity & Add to Cart -->
          <div v-if="!authStore.isAdmin" class="mt-8">
            <div class="flex items-center space-x-4">
              <div class="flex items-center border border-gray-300 rounded-md">
                <button
                  @click="decreaseQuantity"
                  class="px-3 py-2 text-gray-600 hover:text-gray-800"
                  :disabled="quantity <= 1"
                >
                  -
                </button>
                <input
                  v-model.number="quantity"
                  type="number"
                  min="1"
                  :max="product.stockQuantity"
                  class="w-16 text-center border-x border-gray-300 py-2 focus:outline-none"
                />
                <button
                  @click="increaseQuantity"
                  class="px-3 py-2 text-gray-600 hover:text-gray-800"
                  :disabled="quantity >= product.stockQuantity"
                >
                  +
                </button>
              </div>

              <button
                @click="addToCart"
                :disabled="product.stockQuantity === 0 || addingToCart"
                class="flex-1 bg-indigo-600 border border-transparent rounded-md py-3 px-8 flex items-center justify-center text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ addingToCart ? 'Adding...' : 'Add to Cart' }}
              </button>
            </div>
          </div>

          <!-- Success Message -->
          <div v-if="addedToCart" class="mt-4 p-4 bg-green-50 rounded-md">
            <p class="text-sm text-green-800">Added to cart!</p>
          </div>
        </div>
      </div>
    </div>
    
    <div v-else class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <p class="text-center text-gray-500">Product not found</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCartStore } from '../../stores/cart'
import { useAuthStore } from '../../stores/auth'
import api from '../../services/api'

const route = useRoute()
const router = useRouter()
const cartStore = useCartStore()
const authStore = useAuthStore()

const product = ref(null)
const loading = ref(true)
const quantity = ref(1)
const addingToCart = ref(false)
const addedToCart = ref(false)

const formatPrice = (price) => {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(price)
}

const increaseQuantity = () => {
  if (quantity.value < product.value.stockQuantity) {
    quantity.value++
  }
}

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--
  }
}

const addToCart = async () => {
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }

  addingToCart.value = true
  addedToCart.value = false
  
  const result = await cartStore.addToCart(product.value.id, quantity.value)
  
  if (result.success) {
    addedToCart.value = true
    setTimeout(() => {
      addedToCart.value = false
    }, 3000)
  }
  
  addingToCart.value = false
}

const loadProduct = async () => {
  loading.value = true
  try {
    const response = await api.get(`/products/${route.params.id}`)
    product.value = response.data
  } catch (error) {
    console.error('Error loading product:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadProduct()
})
</script>
