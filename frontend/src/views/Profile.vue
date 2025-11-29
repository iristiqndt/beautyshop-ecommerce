<template>
  <div class="bg-gray-50 min-h-screen">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <h1 class="text-3xl font-bold text-gray-900 mb-8">My Account</h1>

      <div class="lg:grid lg:grid-cols-12 lg:gap-x-8">
        <!-- Sidebar -->
        <div class="lg:col-span-3">
          <nav class="space-y-1 bg-white rounded-lg shadow p-4">
            <a
              v-for="item in navigation"
              :key="item.name"
              :class="[
                item.current
                  ? 'bg-indigo-50 text-indigo-600'
                  : 'text-gray-900 hover:bg-gray-50',
                'group flex items-center px-3 py-2 text-sm font-medium rounded-md cursor-pointer'
              ]"
              @click="currentTab = item.id"
            >
              {{ item.name }}
            </a>
          </nav>
        </div>

        <!-- Main Content -->
        <div class="lg:col-span-9 mt-8 lg:mt-0">
          <!-- Profile Info -->
          <div v-if="currentTab === 'profile'" class="bg-white rounded-lg shadow">
            <div class="px-6 py-5 border-b border-gray-200">
              <h2 class="text-lg font-medium text-gray-900">Personal Information</h2>
            </div>
            <form @submit.prevent="updateProfile" class="px-6 py-5 space-y-6">
              <div v-if="successMessage" class="rounded-md bg-green-50 p-4">
                <p class="text-sm text-green-800">{{ successMessage }}</p>
              </div>

              <div v-if="errorMessage" class="rounded-md bg-red-50 p-4">
                <p class="text-sm text-red-800">{{ errorMessage }}</p>
              </div>

              <div>
                <label for="fullName" class="block text-sm font-medium text-gray-700">Full Name</label>
                <input
                  id="fullName"
                  v-model="profileForm.fullName"
                  type="text"
                  class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                />
              </div>

              <div>
                <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
                <input
                  id="email"
                  v-model="profileForm.email"
                  type="email"
                  disabled
                  class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 bg-gray-100 text-gray-500 sm:text-sm"
                />
              </div>

              <div>
                <label for="phoneNumber" class="block text-sm font-medium text-gray-700">Phone Number</label>
                <input
                  id="phoneNumber"
                  v-model="profileForm.phoneNumber"
                  type="tel"
                  class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                />
              </div>

              <div>
                <label for="address" class="block text-sm font-medium text-gray-700">Address</label>
                <textarea
                  id="address"
                  v-model="profileForm.address"
                  rows="3"
                  class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                ></textarea>
              </div>

              <div class="flex justify-end">
                <button
                  type="submit"
                  :disabled="updating"
                  class="bg-indigo-600 border border-transparent rounded-md shadow-sm py-2 px-4 text-sm font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
                >
                  {{ updating ? 'Updating...' : 'Update' }}
                </button>
              </div>
            </form>
          </div>

          <!-- Change Password -->
          <div v-if="currentTab === 'password'" class="bg-white rounded-lg shadow">
            <div class="px-6 py-5 border-b border-gray-200">
              <h2 class="text-lg font-medium text-gray-900">Change Password</h2>
            </div>
            <form @submit.prevent="changePassword" class="px-6 py-5 space-y-6">
              <div v-if="passwordSuccess" class="rounded-md bg-green-50 p-4">
                <p class="text-sm text-green-800">{{ passwordSuccess }}</p>
              </div>

              <div v-if="passwordError" class="rounded-md bg-red-50 p-4">
                <p class="text-sm text-red-800">{{ passwordError }}</p>
              </div>

              <div>
                <label for="oldPassword" class="block text-sm font-medium text-gray-700">Current Password</label>
                <input
                  id="oldPassword"
                  v-model="passwordForm.oldPassword"
                  type="password"
                  required
                  class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                />
              </div>

              <div>
                <label for="newPassword" class="block text-sm font-medium text-gray-700">New Password</label>
                <input
                  id="newPassword"
                  v-model="passwordForm.newPassword"
                  type="password"
                  required
                  class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                />
              </div>

              <div>
                <label for="confirmPassword" class="block text-sm font-medium text-gray-700">Confirm New Password</label>
                <input
                  id="confirmPassword"
                  v-model="passwordForm.confirmPassword"
                  type="password"
                  required
                  class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                />
              </div>

              <div class="flex justify-end">
                <button
                  type="submit"
                  :disabled="changingPassword"
                  class="bg-indigo-600 border border-transparent rounded-md shadow-sm py-2 px-4 text-sm font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
                >
                  {{ changingPassword ? 'Changing...' : 'Change Password' }}
                </button>
              </div>
            </form>
          </div>

          <!-- Orders Link -->
          <div v-if="currentTab === 'orders'">
            <router-link to="/orders" class="block bg-white rounded-lg shadow p-6 hover:bg-gray-50">
              <h2 class="text-lg font-medium text-gray-900">My Orders</h2>
              <p class="mt-1 text-sm text-gray-500">View all my orders</p>
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import api from '../services/api'

const authStore = useAuthStore()

const currentTab = ref('profile')

const navigation = reactive([
  { id: 'profile', name: 'Personal Information', current: true },
  { id: 'password', name: 'Change Password', current: false },
  { id: 'orders', name: 'My Orders', current: false }
])

const profileForm = ref({
  fullName: '',
  email: '',
  phoneNumber: '',
  address: ''
})

const passwordForm = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const updating = ref(false)
const changingPassword = ref(false)
const successMessage = ref('')
const errorMessage = ref('')
const passwordSuccess = ref('')
const passwordError = ref('')

const updateProfile = async () => {
  updating.value = true
  successMessage.value = ''
  errorMessage.value = ''

  try {
    await api.put('/users/profile', profileForm.value)
    successMessage.value = 'Updated successfully!'
    await authStore.fetchUser()
  } catch (error) {
    errorMessage.value = error.response?.data?.message || 'An error occurred'
  } finally {
    updating.value = false
  }
}

const changePassword = async () => {
  if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
    passwordError.value = 'Password confirmation does not match'
    return
  }

  changingPassword.value = true
  passwordSuccess.value = ''
  passwordError.value = ''

  const result = await authStore.changePassword(
    passwordForm.value.oldPassword,
    passwordForm.value.newPassword
  )

  if (result.success) {
    passwordSuccess.value = 'Password changed successfully!'
    passwordForm.value = {
      oldPassword: '',
      newPassword: '',
      confirmPassword: ''
    }
  } else {
    passwordError.value = result.error
  }

  changingPassword.value = false
}

onMounted(async () => {
  await authStore.fetchUser()
  if (authStore.user) {
    profileForm.value = {
      fullName: authStore.user.fullName || '',
      email: authStore.user.email || '',
      phoneNumber: authStore.user.phoneNumber || '',
      address: authStore.user.address || ''
    }
  }
})
</script>
