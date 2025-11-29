<template>
  <div class="bg-white shadow">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16">
        <div class="flex">
          <div class="flex-shrink-0 flex items-center">
            <router-link to="/" class="text-xl font-bold text-indigo-600">
              Admin Panel
            </router-link>
          </div>
          <nav class="ml-10 flex space-x-8">
            <router-link
              to="/admin/products"
              class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
              active-class="border-indigo-500 text-gray-900"
              inactive-class="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700"
            >
              Sản phẩm
            </router-link>
            <router-link
              to="/admin/orders"
              class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
              active-class="border-indigo-500 text-gray-900"
              inactive-class="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700"
            >
              Đơn hàng
            </router-link>
          </nav>
        </div>
        <div class="flex items-center space-x-4">
          <router-link
            to="/"
            class="text-sm text-gray-500 hover:text-gray-700"
          >
            Về trang chủ
          </router-link>
          <button
            @click="handleLogout"
            class="text-sm text-red-600 hover:text-red-800"
          >
            Đăng xuất
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const handleLogout = async () => {
  await authStore.logout()
  router.push('/login')
}
</script>
