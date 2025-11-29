<template>
  <div class="bg-white">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="border-b border-gray-200 pb-4 mb-8">
        <div class="flex items-center gap-3">
          <router-link to="/" class="text-gray-400 hover:text-gray-600">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-6 h-6">
              <path stroke-linecap="round" stroke-linejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18" />
            </svg>
          </router-link>
          <h1 class="text-3xl font-bold text-gray-900">Products</h1>
        </div>
      </div>

      <!-- Filters & Search -->
      <div class="flex flex-col sm:flex-row gap-4 mb-8">
        <div class="flex-1">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search products..."
            class="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
            @input="handleSearch"
          />
        </div>
        
        <select
          v-model="selectedCategory"
          class="px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
          @change="handleFilterChange"
        >
          <option value="">All Categories</option>
          <option v-for="category in categories" :key="category.id" :value="category.id">
            {{ category.name }}
          </option>
        </select>
        
        <select
          v-model="sortBy"
          class="px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
          @change="handleSortChange"
        >
          <option value="">Sort By</option>
          <option value="price-asc">Price: Low to High</option>
          <option value="price-desc">Price: High to Low</option>
          <option value="name-asc">Name: A-Z</option>
          <option value="name-desc">Name: Z-A</option>
        </select>
      </div>

      <!-- Products Grid -->
      <div v-if="loading" class="flex justify-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
      
      <div v-else-if="filteredProducts.length > 0" class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
        <ProductCard
          v-for="product in filteredProducts"
          :key="product.id"
          :product="product"
        />
      </div>
      
      <div v-else class="text-center py-12">
        <p class="text-gray-500 text-lg">No products found</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import ProductCard from '../../components/ProductCard.vue'
import api from '../../services/api'

const products = ref([])
const categories = ref([])
const loading = ref(true)
const searchQuery = ref('')
const selectedCategory = ref('')
const sortBy = ref('')

const filteredProducts = computed(() => {
  let result = [...products.value]
  
  // Filter by search
  if (searchQuery.value) {
    result = result.filter(p => 
      p.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      p.description?.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }
  
  // Filter by category
  if (selectedCategory.value) {
    result = result.filter(p => p.categoryId === parseInt(selectedCategory.value))
  }
  
  // Sort
  if (sortBy.value) {
    switch (sortBy.value) {
      case 'price-asc':
        result.sort((a, b) => a.price - b.price)
        break
      case 'price-desc':
        result.sort((a, b) => b.price - a.price)
        break
      case 'name-asc':
        result.sort((a, b) => a.name.localeCompare(b.name))
        break
      case 'name-desc':
        result.sort((a, b) => b.name.localeCompare(a.name))
        break
    }
  }
  
  return result
})

const loadProducts = async () => {
  loading.value = true
  try {
    const response = await api.get('/products')
    products.value = response.data
  } catch (error) {
    console.error('Error loading products:', error)
  } finally {
    loading.value = false
  }
}

const loadCategories = async () => {
  try {
    const response = await api.get('/categories')
    categories.value = response.data
  } catch (error) {
    console.error('Error loading categories:', error)
  }
}

const handleSearch = () => {
  // Reactive computed will handle this
}

const handleFilterChange = () => {
  // Reactive computed will handle this
}

const handleSortChange = () => {
  // Reactive computed will handle this
}

onMounted(() => {
  loadProducts()
  loadCategories()
})
</script>
