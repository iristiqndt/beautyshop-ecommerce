<template>
  <div class="bg-white">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="border-b border-gray-200 pb-4 mb-8">
        <nav class="flex mb-4" aria-label="Breadcrumb">
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
            <li class="text-gray-900">{{ categoryName }}</li>
          </ol>
        </nav>
        
        <h1 class="text-3xl font-bold text-gray-900">{{ categoryName }}</h1>
      </div>

      <!-- Products Grid -->
      <div v-if="loading" class="flex justify-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
      
      <div v-else-if="products.length > 0" class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
        <ProductCard
          v-for="product in products"
          :key="product.id"
          :product="product"
        />
      </div>
      
      <div v-else class="text-center py-12">
        <p class="text-gray-500 text-lg">No products in this category</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import ProductCard from '../../components/ProductCard.vue'
import api from '../../services/api'

const route = useRoute()

const products = ref([])
const categoryName = ref('')
const loading = ref(true)

const loadCategoryProducts = async () => {
  loading.value = true
  try {
    // First get category info
    const categoriesResponse = await api.get('/categories')
    const category = categoriesResponse.data.find(c => c.slug === route.params.slug)
    
    if (category) {
      categoryName.value = category.name
      
      // Then get products for this category
      const productsResponse = await api.get(`/products/category/${category.id}`)
      products.value = productsResponse.data
    }
  } catch (error) {
    console.error('Error loading category products:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadCategoryProducts()
})
</script>
