<template>
  <div class="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-md">
      <div class="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
        <div class="text-center">
          <svg class="mx-auto h-16 w-16 text-yellow-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
          <h2 class="mt-4 text-2xl font-bold text-gray-900">Payment Cancelled</h2>
          <p class="mt-2 text-gray-600">Your payment was cancelled. No charges were made.</p>
          <div class="mt-6 space-y-3">
            <router-link
              to="/checkout"
              class="block w-full bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
            >
              Try Again
            </router-link>
            <router-link
              to="/cart"
              class="block w-full bg-gray-200 text-gray-700 px-4 py-2 rounded-md hover:bg-gray-300"
            >
              Return to Cart
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
</script>
