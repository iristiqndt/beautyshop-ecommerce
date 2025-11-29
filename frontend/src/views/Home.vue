<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Hero Section -->
    <div class="relative bg-indigo-600 pb-32">
      <div class="absolute inset-0">
        <img
          class="w-full h-full object-cover opacity-50"
          src="https://images.unsplash.com/photo-1596462502278-27bfdc403348?w=1200"
          alt="Cosmetics background"
        />
      </div>
      <div class="relative max-w-7xl mx-auto py-24 px-4 sm:py-32 sm:px-6 lg:px-8">
        <h1 class="text-4xl font-extrabold tracking-tight text-white sm:text-5xl lg:text-6xl">
          Premium Beauty Products
        </h1>
        <p class="mt-6 text-xl text-indigo-100 max-w-3xl">
          Discover our collection of premium cosmetics at the best prices
        </p>
        <div class="mt-10">
          <button
            @click="handleShopNow"
            class="inline-block bg-white py-3 px-8 border border-transparent rounded-md text-base font-medium text-indigo-600 hover:bg-indigo-50"
          >
            Shop Now
          </button>
        </div>
      </div>
    </div>

    <!-- Featured Products Section -->
    <section class="relative z-10 -mt-32">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="bg-white rounded-lg shadow-xl overflow-hidden">
          <div class="px-4 py-5 sm:p-6">
            <h2 class="text-2xl font-bold text-gray-900 mb-6">Featured Products</h2>
            
            <div v-if="loading" class="flex justify-center py-12">
              <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
            </div>
            
            <div v-else-if="featuredProducts.length > 0" class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
              <ProductCard
                v-for="product in featuredProducts"
                :key="product.id"
                :product="product"
              />
            </div>
            
            <div v-else class="text-center py-12 text-gray-500">
              No products available
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Categories Section -->
    <section class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
      <h2 class="text-2xl font-bold text-gray-900 mb-6">Product Categories</h2>
      
      <div v-if="categoriesLoading" class="flex justify-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
      
      <div v-else-if="categories.length > 0" class="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-6">
        <router-link
          v-for="category in categories"
          :key="category.id"
          :to="`/category/${category.slug}`"
          class="group relative bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow overflow-hidden"
        >
          <div class="aspect-w-1 aspect-h-1">
            <img
              :src="category.imageUrl || 'https://via.placeholder.com/200'"
              :alt="category.name"
              class="object-cover w-full h-40"
            />
          </div>
          <div class="p-4">
            <h3 class="text-sm font-medium text-gray-900 text-center">
              {{ category.name }}
            </h3>
          </div>
        </router-link>
      </div>
    </section>

    <!-- Features Section -->
    <section class="bg-white">
      <div class="max-w-7xl mx-auto py-16 px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 gap-8 md:grid-cols-3">
          <div class="text-center">
            <div class="flex justify-center">
              <svg class="h-12 w-12 text-indigo-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <h3 class="mt-4 text-lg font-medium text-gray-900">Guaranteed Quality</h3>
            <p class="mt-2 text-base text-gray-500">100% authentic products</p>
          </div>
          
          <div class="text-center">
            <div class="flex justify-center">
              <svg class="h-12 w-12 text-indigo-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <h3 class="mt-4 text-lg font-medium text-gray-900">Fast Delivery</h3>
            <p class="mt-2 text-base text-gray-500">Nationwide delivery in 2-3 days</p>
          </div>
          
          <div class="text-center">
            <div class="flex justify-center">
              <svg class="h-12 w-12 text-indigo-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
              </svg>
            </div>
            <h3 class="mt-4 text-lg font-medium text-gray-900">Secure Payment</h3>
            <p class="mt-2 text-base text-gray-500">Multiple payment methods supported</p>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import ProductCard from '../components/ProductCard.vue'
import api from '../services/api'
import { useAuthStore } from '../stores/auth'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()
const featuredProducts = ref([])
const categories = ref([])
const loading = ref(true)
const categoriesLoading = ref(true)

const handleShopNow = () => {
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }
  router.push('/products')
}

const loadFeaturedProducts = async () => {
  loading.value = true
  try {
    const response = await api.get('/products/featured')
    featuredProducts.value = response.data
  } catch (error) {
    console.error('Failed to load featured products:', error)
  } finally {
    loading.value = false
  }
}

const loadCategories = async () => {
  categoriesLoading.value = true
  try {
    const response = await api.get('/categories')
    categories.value = response.data
  } catch (error) {
    console.error('Error loading categories:', error)
  } finally {
    categoriesLoading.value = false
  }
}

onMounted(() => {
  loadFeaturedProducts()
  loadCategories()
})
</script>
