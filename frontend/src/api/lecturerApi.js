
import axiosClient from './axiosClient'

const lecturerApi = {
    getAll: () => {
        return axiosClient.get("/Lecturers/")
    },

    getById: (id) => {
        return axiosClient.get(`/Lecturers/${id}`)
    },

	create: (data) => {
		return axiosClient.post('/Lecturers', data)
	},

	// handleAction gọi action(payload) một tham số — hỗ trợ { id, ...body } hoặc (id, body)
	update: (idOrPayload, data) => {
		if (data !== undefined && data !== null) {
			return axiosClient.put(`/Lecturers/${idOrPayload}`, data)
		}
		if (
			idOrPayload &&
			typeof idOrPayload === 'object' &&
			'id' in idOrPayload
		) {
			const { id, ...body } = idOrPayload
			return axiosClient.put(`/Lecturers/${id}`, body)
		}
		return axiosClient.put(`/Lecturers/${idOrPayload}`, data)
	},

    delete: (id) => {
        return axiosClient.delete(`/Lecturers/${id}`)
    }
}

export default lecturerApi
