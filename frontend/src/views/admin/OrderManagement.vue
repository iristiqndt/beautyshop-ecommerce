<template>
  <div class="min-h-screen bg-gray-100 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="mb-6">
        <h1 class="text-3xl font-bold text-gray-900">Quản lý đơn hàng</h1>
      </div>

      <!-- Filter Tabs -->
      <div class="mb-6 border-b border-gray-200">
        <nav class="-mb-px flex space-x-8">
          <button
            v-for="status in statuses"
            :key="status.value"
            @click="filterStatus = status.value"
            :class="[
              filterStatus === status.value
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm'
            ]"
          >
            {{ status.label }}
            <span
              v-if="getCountByStatus(status.value) > 0"
              :class="[
                filterStatus === status.value ? 'bg-indigo-100 text-indigo-600' : 'bg-gray-100 text-gray-900',
                'ml-2 py-0.5 px-2.5 rounded-full text-xs font-medium'
              ]"
            >
              {{ getCountByStatus(status.value) }}
            </span>
          </button>
        </nav>
      </div>

      <!-- Orders List -->
      <div class="bg-white shadow-md rounded-lg overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Mã đơn</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Khách hàng</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Ngày đặt</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tổng tiền</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Trạng thái</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Thao tác</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="order in filteredOrders" :key="order.id">
                <td class="px-6 py-4 text-sm font-medium text-gray-900">
                  {{ order.orderNumber }}
                </td>
                <td class="px-6 py-4 text-sm text-gray-500">
                  <div>{{ order.recipientName }}</div>
                  <div class="text-xs text-gray-400">{{ order.shippingPhone }}</div>
                </td>
                <td class="px-6 py-4 text-sm text-gray-500">
                  {{ formatDate(order.createdAt) }}
                </td>
                <td class="px-6 py-4 text-sm font-medium text-gray-900">
                  {{ formatPrice(order.totalAmount) }}
                </td>
                <td class="px-6 py-4">
                  <span :class="getStatusClass(order.status)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                    {{ getStatusText(order.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 text-sm space-x-2">
                  <button 
                    @click="viewOrder(order.id)"
                    class="text-indigo-600 hover:text-indigo-900"
                  >
                    Chi tiết
                  </button>
                  <button 
                    v-if="order.status === 'Pending' || order.status === 'Processing'"
                    @click="openStatusModal(order)"
                    class="text-blue-600 hover:text-blue-900"
                  >
                    Cập nhật
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div v-if="filteredOrders.length === 0" class="text-center py-12">
          <p class="text-gray-500">Không có đơn hàng nào</p>
        </div>
      </div>

      <!-- Update Status Modal -->
      <div v-if="showStatusModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
        <div class="bg-white rounded-lg p-8 max-w-md w-full">
          <h2 class="text-2xl font-bold mb-4">Cập nhật trạng thái đơn hàng</h2>
          
          <div class="mb-4">
            <p class="text-sm text-gray-600 mb-2">Mã đơn: <span class="font-medium">{{ selectedOrder?.orderNumber }}</span></p>
            <p class="text-sm text-gray-600 mb-4">Trạng thái hiện tại: <span class="font-medium">{{ getStatusText(selectedOrder?.status) }}</span></p>
          </div>

          <form @submit.prevent="updateOrderStatus">
            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-2">Trạng thái mới</label>
              <select
                v-model="newStatus"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option value="">Chọn trạng thái</option>
                <option value="Processing">Đang xử lý</option>
                <option value="Shipped">Đã gửi hàng</option>
                <option value="Delivered">Đã giao hàng</option>
                <option value="Cancelled">Đã hủy</option>
              </select>
            </div>

            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="closeStatusModal"
                class="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50"
              >
                Hủy
              </button>
              <button
                type="submit"
                class="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700"
              >
                Cập nhật
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../../services/api'

const router = useRouter()
const orders = ref([])
const filterStatus = ref('all')
const showStatusModal = ref(false)
const selectedOrder = ref(null)
const newStatus = ref('')

const statuses = [
  { label: 'Tất cả', value: 'all' },
  { label: 'Chờ xử lý', value: 'Pending' },
  { label: 'Đang xử lý', value: 'Processing' },
  { label: 'Đã gửi', value: 'Shipped' },
  { label: 'Đã giao', value: 'Delivered' },
  { label: 'Đã hủy', value: 'Cancelled' }
]

const filteredOrders = computed(() => {
  if (filterStatus.value === 'all') {
    return orders.value
  }
  return orders.value.filter(order => order.status === filterStatus.value)
})

onMounted(async () => {
  await fetchOrders()
})

const fetchOrders = async () => {
  try {
    const response = await api.get('/orders/all')
    orders.value = response.data
  } catch (error) {
    console.error('Error fetching orders:', error)
    alert('Lỗi khi tải danh sách đơn hàng')
  }
}

const getCountByStatus = (status) => {
  if (status === 'all') return orders.value.length
  return orders.value.filter(order => order.status === status).length
}

const viewOrder = (orderId) => {
  router.push(`/orders/${orderId}`)
}

const openStatusModal = (order) => {
  selectedOrder.value = order
  newStatus.value = ''
  showStatusModal.value = true
}

const closeStatusModal = () => {
  showStatusModal.value = false
  selectedOrder.value = null
  newStatus.value = ''
}

const updateOrderStatus = async () => {
  if (!newStatus.value) {
    alert('Vui lòng chọn trạng thái')
    return
  }

  try {
    await api.put(`/orders/${selectedOrder.value.id}/status`, JSON.stringify(newStatus.value), {
      headers: { 'Content-Type': 'application/json' }
    })
    alert('Cập nhật trạng thái thành công!')
    closeStatusModal()
    await fetchOrders()
  } catch (error) {
    alert('Lỗi khi cập nhật: ' + (error.response?.data?.message || error.message))
  }
}

const getStatusClass = (status) => {
  const classes = {
    'Pending': 'bg-yellow-100 text-yellow-800',
    'Processing': 'bg-blue-100 text-blue-800',
    'Shipped': 'bg-purple-100 text-purple-800',
    'Delivered': 'bg-green-100 text-green-800',
    'Cancelled': 'bg-red-100 text-red-800',
    'Paid': 'bg-green-100 text-green-800'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

const getStatusText = (status) => {
  const texts = {
    'Pending': 'Chờ xử lý',
    'Paid': 'Đã thanh toán',
    'Processing': 'Đang xử lý',
    'Shipped': 'Đã gửi hàng',
    'Delivered': 'Đã giao hàng',
    'Cancelled': 'Đã hủy'
  }
  return texts[status] || status
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(price)
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>
