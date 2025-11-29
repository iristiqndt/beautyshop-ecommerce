<template>
  <div class="bg-white min-h-screen">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
      <h1 class="text-4xl font-bold text-gray-900 mb-8">Liên hệ</h1>
      
      <div class="grid grid-cols-1 md:grid-cols-2 gap-12">
        <!-- Contact Info -->
        <div>
          <h2 class="text-2xl font-bold text-gray-900 mb-6">Thông tin liên hệ</h2>
          
          <div class="space-y-4">
            <div class="flex items-start">
              <svg class="h-6 w-6 text-indigo-600 mt-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/>
              </svg>
              <div class="ml-4">
                <p class="text-lg font-medium text-gray-900">Địa chỉ</p>
                <p class="text-gray-600">123 Đường ABC, Quận 1, TP. Hồ Chí Minh</p>
              </div>
            </div>
            
            <div class="flex items-start">
              <svg class="h-6 w-6 text-indigo-600 mt-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
              </svg>
              <div class="ml-4">
                <p class="text-lg font-medium text-gray-900">Email</p>
                <p class="text-gray-600">dthuytien85@gmail.com</p>
              </div>
            </div>
            
            <div class="flex items-start">
              <svg class="h-6 w-6 text-indigo-600 mt-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"/>
              </svg>
              <div class="ml-4">
                <p class="text-lg font-medium text-gray-900">Điện thoại</p>
                <p class="text-gray-600">0123 456 789</p>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Contact Form -->
        <div>
          <h2 class="text-2xl font-bold text-gray-900 mb-6">Gửi tin nhắn</h2>
          
          <form @submit.prevent="handleSubmit" class="space-y-4">
            <div>
              <label for="name" class="block text-sm font-medium text-gray-700">Họ tên</label>
              <input
                id="name"
                v-model="form.name"
                type="text"
                required
                class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>
            
            <div>
              <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
              <input
                id="email"
                v-model="form.email"
                type="email"
                required
                class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>
            
            <div>
              <label for="message" class="block text-sm font-medium text-gray-700">Tin nhắn</label>
              <textarea
                id="message"
                v-model="form.message"
                rows="4"
                required
                class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              ></textarea>
            </div>
            
            <button
              type="submit"
              class="w-full bg-indigo-600 text-white py-2 px-4 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Gửi tin nhắn
            </button>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const form = ref({
  name: '',
  email: '',
  message: ''
})

const handleSubmit = () => {
  alert('Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất.')
  form.value = { name: '', email: '', message: '' }
}
</script>
