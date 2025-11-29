<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from './stores/auth'
import { useCartStore } from './stores/cart'
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'

const authStore = useAuthStore()
const cartStore = useCartStore()

onMounted(async () => {
  if (authStore.token) {
    await authStore.fetchUser()
    await cartStore.fetchCart()
  }
})
</script>

<template>
  <div id="app" class="min-h-screen flex flex-col">
    <Navbar />
    <main class="flex-grow">
      <router-view />
    </main>
    <Footer />
  </div>
</template>

<style>
#app {
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}
</style>
