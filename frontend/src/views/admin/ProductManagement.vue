<template>
  <div class="min-h-screen bg-gray-100 py-8">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="mb-6 flex justify-between items-center">
          <div class="flex items-center gap-3">
            <router-link to="/" class="text-gray-400 hover:text-gray-600">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-6 h-6">
                <path stroke-linecap="round" stroke-linejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18" />
              </svg>
            </router-link>
            <h1 class="text-3xl font-bold text-gray-900">Quản lý sản phẩm</h1>
          </div>
          <button
            @click="openCreateModal"
            class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
          >
            Thêm sản phẩm mới
          </button>
        </div>

      <!-- Products List -->
      <div class="bg-white shadow-md rounded-lg overflow-hidden">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Hình ảnh</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tên sản phẩm</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Danh mục</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Giá</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Kho</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Thao tác</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="product in products" :key="product.id">
              <td class="px-6 py-4">
                <img 
                  :src="product.imageUrl || 'https://via.placeholder.com/100'" 
                  :alt="product.name"
                  class="w-16 h-16 object-cover rounded"
                />
              </td>
              <td class="px-6 py-4 text-sm font-medium text-gray-900">{{ product.name }}</td>
              <td class="px-6 py-4 text-sm text-gray-500">{{ product.categoryName }}</td>
              <td class="px-6 py-4 text-sm text-gray-900">{{ formatPrice(product.price) }}</td>
              <td class="px-6 py-4 text-sm text-gray-500">{{ product.stockQuantity }}</td>
              <td class="px-6 py-4 text-sm space-x-2">
                <button 
                  @click="editProduct(product)"
                  class="text-indigo-600 hover:text-indigo-900"
                >
                  Sửa
                </button>
                <button 
                  @click="deleteProduct(product.id)"
                  class="text-red-600 hover:text-red-900"
                >
                  Xóa
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Create/Edit Modal -->
      <div v-if="showModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
        <div class="bg-white rounded-lg p-8 max-w-2xl w-full max-h-screen overflow-y-auto">
          <h2 class="text-2xl font-bold mb-4">{{ isEditing ? 'Sửa sản phẩm' : 'Thêm sản phẩm mới' }}</h2>
          
          <form @submit.prevent="saveProduct" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Tên sản phẩm</label>
              <input
                v-model="form.name"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Mô tả</label>
              <textarea
                v-model="form.description"
                rows="3"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              ></textarea>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Giá (VNĐ)</label>
                <input
                  v-model.number="form.price"
                  type="number"
                  step="0.01"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md"
                />
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Số lượng</label>
                <input
                  v-model.number="form.stockQuantity"
                  type="number"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Thương hiệu</label>
              <input
                v-model="form.brand"
                type="text"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Danh mục</label>
              <select
                v-model.number="form.categoryId"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option value="" disabled>Chọn danh mục</option>
                <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                  {{ cat.name }}
                </option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Hình ảnh</label>
              <div class="flex items-center space-x-4">
                <input
                  type="file"
                  ref="fileInput"
                  @change="handleFileUpload"
                  accept="image/*"
                  class="hidden"
                />
                <button
                  type="button"
                  @click="$refs.fileInput.click()"
                  class="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50"
                >
                  Chọn ảnh
                </button>
                <span v-if="uploading" class="text-sm text-gray-500">Đang tải lên...</span>
                <span v-else-if="form.imageUrl" class="text-sm text-green-600">✓ Đã tải lên</span>
              </div>
              <div v-if="form.imageUrl" class="mt-2">
                <img :src="form.imageUrl" alt="Preview" class="w-32 h-32 object-cover rounded" />
              </div>
            </div>

            <div class="flex items-center">
              <input
                v-model="form.isFeatured"
                type="checkbox"
                id="isFeatured"
                class="h-4 w-4 text-indigo-600 rounded"
              />
              <label for="isFeatured" class="ml-2 text-sm text-gray-700">Sản phẩm nổi bật</label>
            </div>

            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="closeModal"
                class="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50"
              >
                Hủy
              </button>
              <button
                type="submit"
                class="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700"
              >
                {{ isEditing ? 'Cập nhật' : 'Tạo mới' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'

const products = ref([])
const categories = ref([])
const showModal = ref(false)
const isEditing = ref(false)
const uploading = ref(false)

const form = ref({
  id: null,
  name: '',
  description: '',
  price: 0,
  stockQuantity: 0,
  brand: '',
  categoryId: '',
  imageUrl: '',
  isFeatured: false
})

onMounted(async () => {
  await fetchProducts()
  await fetchCategories()
})

const fetchProducts = async () => {
  try {
    const response = await api.get('/products')
    products.value = response.data
  } catch (error) {
    console.error('Error fetching products:', error)
  }
}

const fetchCategories = async () => {
  try {
    const response = await api.get('/categories')
    categories.value = response.data
  } catch (error) {
    console.error('Error fetching categories:', error)
  }
}

const openCreateModal = () => {
  isEditing.value = false
  resetForm()
  showModal.value = true
}

const editProduct = (product) => {
  isEditing.value = true
  form.value = { ...product }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  resetForm()
}

const resetForm = () => {
  form.value = {
    id: null,
    name: '',
    description: '',
    price: 0,
    stockQuantity: 0,
    brand: '',
    categoryId: '',
    imageUrl: '',
    isFeatured: false
  }
}

const handleFileUpload = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  uploading.value = true
  const formData = new FormData()
  formData.append('file', file)

  try {
    const response = await api.post('/products/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    form.value.imageUrl = response.data.imageUrl
  } catch (error) {
    alert('Lỗi khi tải ảnh lên: ' + (error.response?.data?.message || error.message))
  } finally {
    uploading.value = false
  }
}

const saveProduct = async () => {
  try {
    if (isEditing.value) {
      await api.put(`/products/${form.value.id}`, form.value)
      alert('Cập nhật sản phẩm thành công!')
    } else {
      await api.post('/products', form.value)
      alert('Thêm sản phẩm thành công!')
    }
    closeModal()
    await fetchProducts()
  } catch (error) {
    alert('Lỗi: ' + (error.response?.data?.message || error.message))
  }
}

const deleteProduct = async (id) => {
  if (!confirm('Bạn có chắc chắn muốn xóa sản phẩm này?')) return

  try {
    await api.delete(`/products/${id}`)
    alert('Xóa sản phẩm thành công!')
    await fetchProducts()
  } catch (error) {
    alert('Lỗi khi xóa: ' + (error.response?.data?.message || error.message))
  }
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(price)
}
</script>
