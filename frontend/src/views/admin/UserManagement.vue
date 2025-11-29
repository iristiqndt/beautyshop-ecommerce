<template>
  <div class="min-h-screen bg-gray-100 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="mb-6">
        <h1 class="text-3xl font-bold text-gray-900">User Management</h1>
      </div>

      <!-- Users List -->
      <div class="bg-white shadow-md rounded-lg overflow-hidden overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Full Name</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Email</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Role</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Phone</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="user in users" :key="user.id">
              <td class="px-4 py-3 whitespace-nowrap text-sm">{{ user.id }}</td>
              <td class="px-4 py-3 whitespace-nowrap">{{ user.fullName }}</td>
              <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-500">{{ user.email }}</td>
              <td class="px-4 py-3 whitespace-nowrap">
                <span 
                  :class="getRoleName(user) === 'Admin' ? 'bg-purple-100 text-purple-800' : 'bg-blue-100 text-blue-800'"
                  class="px-2 py-1 text-xs font-semibold rounded-full"
                >
                  {{ getRoleName(user) }}
                </span>
              </td>
              <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-500">{{ user.phoneNumber || 'N/A' }}</td>
              <td class="px-4 py-3 whitespace-nowrap text-sm">
                <button 
                  @click="viewUser(user)"
                  class="text-indigo-600 hover:text-indigo-900 mr-2"
                >
                  View
                </button>
                <button 
                  @click="toggleUserRole(user)"
                  class="text-green-600 hover:text-green-900 mr-2"
                >
                  {{ isAdmin(user) ? 'Remove Admin' : 'Make Admin' }}
                </button>
                <button 
                  v-if="!isAdmin(user)"
                  @click="deleteUser(user.id)"
                  class="text-red-600 hover:text-red-900"
                >
                  Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- User Detail Modal -->
      <div
        v-if="showModal"
        class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50"
        @click.self="closeModal"
      >
        <div class="relative top-20 mx-auto p-5 border w-full max-w-2xl shadow-lg rounded-md bg-white">
          <h3 class="text-lg font-medium mb-4">User Details</h3>
          
          <div v-if="selectedUser" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Full Name</label>
                <p class="text-sm text-gray-900">{{ selectedUser.fullName }}</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
                <p class="text-sm text-gray-900">{{ selectedUser.email }}</p>
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Phone Number</label>
                <p class="text-sm text-gray-900">{{ selectedUser.phoneNumber || 'N/A' }}</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Role</label>
                <p class="text-sm text-gray-900">{{ getRoleName(selectedUser) }}</p>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Address</label>
              <p class="text-sm text-gray-900">{{ selectedUser.address || 'N/A' }}</p>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Created Date</label>
                <p class="text-sm text-gray-900">{{ formatDate(selectedUser.createdAt) }}</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Last Updated</label>
                <p class="text-sm text-gray-900">{{ formatDate(selectedUser.updatedAt) }}</p>
              </div>
            </div>

            <div class="flex justify-end pt-4">
              <button
                type="button"
                @click="closeModal"
                class="px-4 py-2 bg-gray-500 text-white rounded-md hover:bg-gray-600"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'

const users = ref([])
const showModal = ref(false)
const selectedUser = ref(null)

onMounted(async () => {
  await fetchUsers()
})

const fetchUsers = async () => {
  try {
    const response = await api.get('/users')
    users.value = response.data
  } catch (error) {
    console.error('Error fetching users:', error)
    alert('Error loading user list')
  }
}

const viewUser = (user) => {
  selectedUser.value = user
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  selectedUser.value = null
}

const toggleUserRole = async (user) => {
  const currentRole = user.role?.name || user.role
  const newRole = currentRole === 'User' ? 'Admin' : 'User'
  const confirmMsg = newRole === 'Admin' 
    ? `Are you sure you want to make ${user.fullName} an Admin?`
    : `Are you sure you want to remove Admin privileges from ${user.fullName}?`
  
  if (!confirm(confirmMsg)) return
  
  try {
    await api.put(`/users/${user.id}/role`, { role: newRole })
    await fetchUsers()
    alert('Role updated successfully')
  } catch (error) {
    alert('Error updating: ' + (error.response?.data?.message || error.message))
  }
}

const deleteUser = async (id) => {
  if (!confirm('Are you sure you want to delete this user?')) return
  
  try {
    await api.delete(`/users/${id}`)
    await fetchUsers()
    alert('User deleted successfully')
  } catch (error) {
    alert('Error deleting: ' + (error.response?.data?.message || error.message))
  }
}

const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const getRoleName = (user) => {
  if (!user || !user.role) return 'N/A'
  const roleName = user.role.name || user.role
  return roleName === 'Admin' ? 'Admin' : 'User'
}

const isAdmin = (user) => {
  if (!user || !user.role) return false
  const roleName = user.role.name || user.role
  return roleName === 'Admin'
}
</script>
