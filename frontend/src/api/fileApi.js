import axiosClient from './axiosClient'

const fileApi = {
	// --- User ---
	search: async ({
	keyword,
	subjectCodes,
	fileType,
	page,
	pageSize
	}) => {

		const query = new URLSearchParams()

		if (keyword) query.append("keyword", keyword)

		if (subjectCodes && subjectCodes.length > 0) {
			subjectCodes.forEach(code => {
				query.append("subjectCodes", code)
			})
		}

		if (fileType) query.append("fileType", fileType)

		query.append("page", page)
		query.append("pageSize", pageSize)

		//console.log("Query params:", query.toString())
	
		const res = await axiosClient.get("/Files/search", {
			params: query
		})
		//console.log("API response:", res)
		return res
	},

	getById: (id) => {
		return axiosClient.get(`/Files/${id}`)
	},

	download: (id) => {
		return axiosClient.get(`/Files/${id}/download`, {
			responseType: 'blob'
		})
	},
 
	// --- ADMIN ---
	upload: (formData) => {
		// FormData: 'File', 'Title', 'SubjectCode', 'Permission', 'FileType'
		return axiosClient.post('/Files', formData, {
			headers: { 'Content-Type': 'multipart/form-data' }
		})
	},

	replace: (id, formData) => {
		return axiosClient.post(`/Files/${id}/replace`, formData, {
			headers: { 'Content-Type': 'multipart/form-data' }
		})
	},

	updateMetadata: (id, data) => {
		// data: { title, subjectCode, permission, filetype }
		return axiosClient.put(`/Files/${id}`, data)
	},

	delete: (id) => {
		return axiosClient.delete(`/Files/${id}`)
	},

	getPendingList: () => {
		return axiosClient.get('/Files/requests/pending')
	},

	approve: (id, data) => {
		// data: { isApproved: true/false, reason: '...' }
		return axiosClient.put(`/Files/requests/${id}/approve`, data)
	},

	// --- THỐNG KÊ (DASHBOARD) ---
	getStatsByType: () => axiosClient.get('/Files/stats/type'),
	getStatsBySubject: () => axiosClient.get('/Files/stats/subject'),
	getStatsByTraffic: () => axiosClient.get('/Files/stats/traffic')
}

export default fileApi
