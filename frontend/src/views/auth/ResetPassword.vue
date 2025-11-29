<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Đặt lại mật khẩu
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Nhập mật khẩu mới của bạn
        </p>
      </div>
      
      <form class="mt-8 space-y-6" @submit.prevent="handleSubmit">
        <div v-if="error" class="rounded-md bg-red-50 p-4">
          <p class="text-sm text-red-800">{{ error }}</p>
        </div>
        
        <div v-if="success" class="rounded-md bg-green-50 p-4">
          <p class="text-sm text-green-800">
            Mật khẩu đã được đặt lại thành công!
          </p>
        </div>
        
        <div class="space-y-4">
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700">Mật khẩu mới</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              placeholder="Tối thiểu 6 ký tự"
            />
          </div>
          
          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700">Xác nhận mật khẩu</label>
            <input
              id="confirmPassword"
              v-model="form.confirmPassword"
              type="password"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              placeholder="Nhập lại mật khẩu"
            />
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
          >
            {{ loading ? 'Đang đặt lại...' : 'Đặt lại mật khẩu' }}
          </button>
        </div>
        
        <div class="text-center">
          <router-link to="/login" class="font-medium text-indigo-600 hover:text-indigo-500">
            Quay lại đăng nhập
          </router-link>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const form = ref({
  password: '',
  confirmPassword: ''
})

const loading = ref(false)
const error = ref('')
const success = ref(false)
const token = ref('')

onMounted(() => {
  token.value = route.query.token || ''
  if (!token.value) {
    error.value = 'Token không hợp lệ'
  }
})

const handleSubmit = async () => {
  if (form.value.password !== form.value.confirmPassword) {
    error.value = 'Mật khẩu xác nhận không khớp'
    return
  }
  
  loading.value = true
  error.value = ''
  success.value = false
  
  const result = await authStore.resetPassword(token.value, form.value.password)
  
  if (result.success) {
    success.value = true
    setTimeout(() => {
      router.push('/login')
    }, 2000)
  } else {
    error.value = result.error
  }
  
  loading.value = false
}
</script>
