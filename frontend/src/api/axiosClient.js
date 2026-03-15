
import axios from 'axios'

const axiosClient = axios.create({
	baseURL: 'https://localhost:7108/api', // port backend
	headers: {
		'Content-Type': 'application/json'
	}
})

axiosClient.interceptors.request.use(
	(config) => {
		const token = localStorage.getItem('token')
		if (token) {
			config.headers.Authorization = `Bearer ${token}`
		}
		return config
	},
	(error) => Promise.reject(error)
)

// Interceptor cho Response: Xử lý lỗi chung
axiosClient.interceptors.response.use(
	(response) => {
		return response.data
	},
	(error) => {
		if (error.response) {
			if (error.response.status === 401) {
				if (!error.config.url.includes('/Auth/login')) {
					// Hết hạn token hoặc chưa đăng nhập
					localStorage.removeItem('token')
					localStorage.removeItem('role')
					localStorage.removeItem('username')
					// window.location.href = '/login'
				}
			} else if (error.response.status === 403) {
				alert('Bạn không có quyền thực hiện hành động này!')
			}
		}
		return Promise.reject(error)
	}
)

export default axiosClient
