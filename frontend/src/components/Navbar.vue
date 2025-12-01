<template>
  <nav class="bg-white shadow-lg">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16">
        <div class="flex">
          <div class="flex-shrink-0 flex items-center">
            <router-link to="/" class="text-2xl font-bold text-indigo-600">
              BeautyShop
            </router-link>
          </div>
          <div class="hidden sm:ml-6 sm:flex sm:space-x-8">
            <router-link
              to="/"
              class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
              :class="isActive('/') ? 'border-indigo-500 text-gray-900' : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'"
            >
              Home
            </router-link>
            <router-link
              v-if="!authStore.isAdmin"
              to="/products"
              class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
              :class="isActive('/products') ? 'border-indigo-500 text-gray-900' : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'"
            >
              Products
            </router-link>
            <router-link
              v-if="authStore.isAuthenticated && !authStore.isAdmin"
              to="/orders"
              class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
              :class="isActive('/orders') ? 'border-indigo-500 text-gray-900' : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'"
            >
              Orders
            </router-link>
            <router-link
              v-if="authStore.isAdmin"
              to="/admin/users"
              class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
              :class="isActive('/admin/users') ? 'border-indigo-500 text-gray-900' : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'"
            >
              Manage Users
            </router-link>
            <router-link
              v-if="authStore.isAdmin"
              to="/admin/categories"
              class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
              :class="isActive('/admin/categories') ? 'border-indigo-500 text-gray-900' : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'"
            >
              Manage Categories
            </router-link>
            <router-link
              v-if="authStore.isAdmin"
              to="/admin/products"
              class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
              :class="isActive('/admin/products') ? 'border-indigo-500 text-gray-900' : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'"
            >
              Manage Products
            </router-link>
            <router-link
              v-if="authStore.isAdmin"
              to="/admin/orders"
              class="inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
              :class="isActive('/admin/orders') ? 'border-indigo-500 text-gray-900' : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'"
            >
              Manage Orders
            </router-link>
          </div>
        </div>

        <div class="hidden sm:ml-6 sm:flex sm:items-center sm:space-x-4">
          <!-- Cart -->
          <router-link
            v-if="!authStore.isAdmin"
            to="/cart"
            class="relative p-2 text-gray-400 hover:text-gray-500"
          >
            <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            <span
              v-if="cartStore.itemCount > 0"
              class="absolute top-0 right-0 inline-flex items-center justify-center px-2 py-1 text-xs font-bold leading-none text-white transform translate-x-1/2 -translate-y-1/2 bg-indigo-600 rounded-full"
            >
              {{ cartStore.itemCount }}
            </span>
          </router-link>

          <!-- User Menu -->
          <div v-if="authStore.isAuthenticated" class="relative">
            <button
              @click="userMenuOpen = !userMenuOpen"
              class="flex items-center text-sm rounded-full focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              <span class="sr-only">Open user menu</span>
              <div class="h-8 w-8 rounded-full bg-indigo-600 flex items-center justify-center text-white font-medium">
                {{ getUserInitials() }}
              </div>
            </button>

            <transition
              enter-active-class="transition ease-out duration-200"
              enter-from-class="transform opacity-0 scale-95"
              enter-to-class="transform opacity-100 scale-100"
              leave-active-class="transition ease-in duration-75"
              leave-from-class="transform opacity-100 scale-100"
              leave-to-class="transform opacity-0 scale-95"
            >
              <div
                v-if="userMenuOpen"
                class="origin-top-right absolute right-0 mt-2 w-48 rounded-md shadow-lg py-1 bg-white ring-1 ring-black ring-opacity-5 z-50"
              >
                <router-link
                  v-if="!authStore.isAdmin"
                  to="/profile"
                  class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                  @click="userMenuOpen = false"
                >
                  My Account
                </router-link>
                <button
                  @click="handleLogout"
                  class="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                >
                  Logout
                </button>
              </div>
            </transition>
          </div>

          <!-- Login/Register -->
          <div v-else class="flex items-center space-x-4">
            <router-link
              to="/login"
              class="text-sm font-medium text-gray-700 hover:text-gray-900"
            >
              Sign In
            </router-link>
            <router-link
              to="/register"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700"
            >
              Sign Up
            </router-link>
          </div>
        </div>

        <!-- Mobile menu button -->
        <div class="flex items-center sm:hidden">
          <button
            @click="mobileMenuOpen = !mobileMenuOpen"
            class="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-indigo-500"
          >
            <span class="sr-only">Open main menu</span>
            <svg
              class="h-6 w-6"
              :class="{ hidden: mobileMenuOpen, block: !mobileMenuOpen }"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
            <svg
              class="h-6 w-6"
              :class="{ block: mobileMenuOpen, hidden: !mobileMenuOpen }"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Mobile menu -->
    <div v-if="mobileMenuOpen" class="sm:hidden">
      <div class="pt-2 pb-3 space-y-1">
        <router-link
          to="/"
          class="block pl-3 pr-4 py-2 border-l-4 text-base font-medium"
          :class="isActive('/') ? 'bg-indigo-50 border-indigo-500 text-indigo-700' : 'border-transparent text-gray-500 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-700'"
          @click="mobileMenuOpen = false"
        >
          Home
        </router-link>
        <router-link
          to="/products"
          class="block pl-3 pr-4 py-2 border-l-4 text-base font-medium"
          :class="isActive('/products') ? 'bg-indigo-50 border-indigo-500 text-indigo-700' : 'border-transparent text-gray-500 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-700'"
          @click="mobileMenuOpen = false"
        >
          Products
        </router-link>
        <router-link
          v-if="!authStore.isAdmin"
          to="/cart"
          class="block pl-3 pr-4 py-2 border-l-4 text-base font-medium border-transparent text-gray-500 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-700"
          @click="mobileMenuOpen = false"
        >
          Cart ({{ cartStore.itemCount }})
        </router-link>
      </div>
      <div v-if="authStore.isAuthenticated" class="pt-4 pb-3 border-t border-gray-200">
        <div class="flex items-center px-4">
          <div class="flex-shrink-0">
            <div class="h-10 w-10 rounded-full bg-indigo-600 flex items-center justify-center text-white font-medium">
              {{ getUserInitials() }}
            </div>
          </div>
          <div class="ml-3">
            <div class="text-base font-medium text-gray-800">{{ authStore.user?.fullName }}</div>
            <div class="text-sm font-medium text-gray-500">{{ authStore.user?.email }}</div>
          </div>
        </div>
        <div class="mt-3 space-y-1">
          <router-link
            v-if="!authStore.isAdmin"
            to="/profile"
            class="block px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100"
            @click="mobileMenuOpen = false"
          >
            My Account
          </router-link>
          <router-link
            to="/orders"
            class="block px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100"
            @click="mobileMenuOpen = false"
          >
            Orders
          </router-link>
          <button
            @click="handleLogout"
            class="block w-full text-left px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100"
          >
            Logout
          </button>
        </div>
      </div>
      <div v-else class="pt-4 pb-3 border-t border-gray-200">
        <div class="space-y-1">
          <router-link
            to="/login"
            class="block px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100"
            @click="mobileMenuOpen = false"
          >
            Sign In
          </router-link>
          <router-link
            to="/register"
            class="block px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100"
            @click="mobileMenuOpen = false"
          >
            Sign Up
          </router-link>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useCartStore } from '../stores/cart'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const cartStore = useCartStore()

const userMenuOpen = ref(false)
const mobileMenuOpen = ref(false)

const isActive = (path) => {
  return route.path === path || route.path.startsWith(path + '/')
}

const getUserInitials = () => {
  if (!authStore.user?.fullName) return 'U'
  const names = authStore.user.fullName.split(' ')
  return names.length > 1
    ? names[0][0] + names[names.length - 1][0]
    : names[0][0]
}

const handleLogout = async () => {
  await authStore.logout()
  userMenuOpen.value = false
  mobileMenuOpen.value = false
  router.push('/login')
}
</script>
