<template>
  <div class="min-h-screen bg-gray-100 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="mb-6 flex justify-between items-center">
        <h1 class="text-3xl font-bold text-gray-900">Quản lý danh mục</h1>
        <button
          @click="openCreateModal"
          class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
        >
          Thêm danh mục mới
        </button>
      </div>

      <!-- Categories List -->
      <div class="bg-white shadow-md rounded-lg overflow-hidden">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Hình ảnh</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tên danh mục</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Slug</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Mô tả</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Thao tác</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="category in categories" :key="category.id">
              <td class="px-6 py-4 whitespace-nowrap">
                <img 
                  v-if="category.imageUrl" 
                  :src="category.imageUrl" 
                  alt="Category" 
                  class="w-16 h-16 object-cover rounded"
                />
                <div v-else class="w-16 h-16 bg-gray-200 rounded flex items-center justify-center">
                  <span class="text-gray-400 text-xs">No image</span>
                </div>
              </td>
              <td class="px-6 py-4">{{ category.name }}</td>
              <td class="px-6 py-4 text-sm text-gray-500">{{ category.slug }}</td>
              <td class="px-6 py-4 text-sm text-gray-500">{{ category.description }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm">
                <button 
                  @click="editCategory(category)"
                  class="text-indigo-600 hover:text-indigo-900 mr-3"
                >
                  Sửa
                </button>
                <button 
                  @click="deleteCategory(category.id)"
                  class="text-red-600 hover:text-red-900"
                >
                  Xóa
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Modal -->
      <div
        v-if="showModal"
        class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50"
        @click.self="closeModal"
      >
        <div class="relative top-20 mx-auto p-5 border w-full max-w-md shadow-lg rounded-md bg-white">
          <h3 class="text-lg font-medium mb-4">
            {{ isEditing ? 'Sửa danh mục' : 'Thêm danh mục mới' }}
          </h3>
          
          <form @submit.prevent="saveCategory" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Tên danh mục</label>
              <input
                v-model="form.name"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Slug</label>
              <input
                v-model="form.slug"
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
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              ></textarea>
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
                {{ isEditing ? 'Cập nhật' : 'Thêm mới' }}
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

const categories = ref([])
const showModal = ref(false)
const isEditing = ref(false)
const uploading = ref(false)

const form = ref({
  id: null,
  name: '',
  slug: '',
  description: '',
  imageUrl: ''
})

onMounted(async () => {
  await fetchCategories()
})

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

const editCategory = (category) => {
  isEditing.value = true
  form.value = { ...category }
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
    slug: '',
    description: '',
    imageUrl: ''
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
    alert('Lỗi khi tải ảnh: ' + (error.response?.data?.message || error.message))
  } finally {
    uploading.value = false
  }
}

const saveCategory = async () => {
  try {
    if (isEditing.value) {
      // Update: gửi tất cả trường trừ id
      const updateData = {
        name: form.value.name,
        slug: form.value.slug,
        description: form.value.description,
        imageUrl: form.value.imageUrl
      }
      await api.put(`/categories/${form.value.id}`, updateData)
    } else {
      // Create: chỉ gửi các trường cần thiết
      const createData = {
        name: form.value.name,
        slug: form.value.slug,
        description: form.value.description,
        imageUrl: form.value.imageUrl || null
      }
      await api.post('/categories', createData)
    }
    await fetchCategories()
    closeModal()
  } catch (error) {
    alert('Lỗi: ' + (error.response?.data?.message || error.message))
  }
}

const deleteCategory = async (id) => {
  if (!confirm('Bạn có chắc muốn xóa danh mục này?')) return
  
  try {
    await api.delete(`/categories/${id}`)
    await fetchCategories()
  } catch (error) {
    alert('Lỗi khi xóa: ' + (error.response?.data?.message || error.message))
  }
}
</script>
