<template>
  <footer class="bg-gray-800">
    <div class="max-w-7xl mx-auto py-12 px-4 sm:px-6 lg:px-8">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
        <!-- About -->
        <div>
          <h3 class="text-white text-lg font-semibold mb-4">BeautyShop</h3>
          <p class="text-gray-400 text-sm">
            Premium beauty store with the best prices on the market.
          </p>
        </div>

        <!-- Quick Links -->
        <div>
          <h3 class="text-white text-lg font-semibold mb-4">Quick Links</h3>
          <ul class="space-y-2">
            <li>
              <router-link to="/products" class="text-gray-400 hover:text-white text-sm">
                Products
              </router-link>
            </li>
            <li>
              <router-link to="/about" class="text-gray-400 hover:text-white text-sm">
                About Us
              </router-link>
            </li>
            <li>
              <router-link to="/contact" class="text-gray-400 hover:text-white text-sm">
                Contact
              </router-link>
            </li>
          </ul>
        </div>

        <!-- Customer Service -->
        <div>
          <h3 class="text-white text-lg font-semibold mb-4">Support</h3>
          <ul class="space-y-2">
            <li>
              <a href="#" class="text-gray-400 hover:text-white text-sm">
                Return Policy
              </a>
            </li>
            <li>
              <a href="#" class="text-gray-400 hover:text-white text-sm">
                Privacy Policy
              </a>
            </li>
            <li>
              <a href="#" class="text-gray-400 hover:text-white text-sm">
                Terms of Service
              </a>
            </li>
          </ul>
        </div>

        <!-- Contact -->
        <div>
          <h3 class="text-white text-lg font-semibold mb-4">Contact</h3>
          <ul class="space-y-2 text-gray-400 text-sm">
            <li>Email: support@beautyshop.com</li>
            <li>Hotline: 1-800-BEAUTY</li>
            <li>Address: 123 Main Street, New York, NY</li>
          </ul>
        </div>
      </div>

      <div class="mt-8 pt-8 border-t border-gray-700">
        <p class="text-center text-gray-400 text-sm">
          &copy; 2025 BeautyShop. All rights reserved.
        </p>
      </div>
    </div>
  </footer>
</template>

<script setup>
</script>
