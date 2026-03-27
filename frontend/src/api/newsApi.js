import axiosClient from './axiosClient'

const normalizeList = (res) => {
	if (!res) return { items: [], total: 0 }
	// backend kiểu { items, total }
	if (res.items) {
		return {
			items: res.items,
			total: res.total ?? res.items.length
		}
	}
	// backend trả array
	if (Array.isArray(res)) {
		return {
			items: res,
			total: res.length
		}
	}
	return { items: [], total: 0 }
}

const newsApi = {
	// ===== PUBLIC =====
	search: async (params = {}) => {
		const res = await axiosClient.get('/News/search', { params })
		return normalizeList(res)
	},

	getById: async (id) => {
		return await axiosClient.get(`/News/${id}`)
	},

	postComment: async (newsId, data) => {
		// data: { MSV, StudentName, Content }
		return await axiosClient.post(`/News/${newsId}/comments`, data)
	},

	// ===== ADMIN =====
	create: async (data) => {
		return await axiosClient.post('/News', data)
	},

	update: async (id, data) => {
		return await axiosClient.put(`/News/${id}`, data)
	},

	delete: async (id) => {
		return await axiosClient.delete(`/News/${id}`)
	},

	getPendingList: async () => {
		const res = await axiosClient.get('/News/pending')
		return normalizeList(res)
	},

	approve: async (id, data) => {
		// data: { isApproved: boolean, reason: string }
		return await axiosClient.put(`/News/requests/${id}/approve`, data)
	},

	deleteComment: async (commentId) => {
		return await axiosClient.delete(`/News/comments/${commentId}`)
	},

	// ===== STATS =====
	getStatsByType: () => axiosClient.get('/News/stats/type'),
	getStatsByMonth: (year) => axiosClient.get(`/News/stats/month?year=${year}`)
}

export default newsApi
