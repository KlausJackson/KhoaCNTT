// src/api/adminApi.js
import axiosClient from './axiosClient'

const adminApi = {
	getAll: () => {
		return axiosClient.get('/Admins')
	},

	create: (data) => {
		// data: { username, password, fullName, email, level }
		return axiosClient.post('/Admins', data)
	},

	update: (data) => {
        // data: { fullName, email, level, isActive }
		return axiosClient.put(`/Admins/${data.id}`, data)
	},

	delete: (id) => {
		return axiosClient.delete(`/Admins/${id}`)
	}
}

export default adminApi
