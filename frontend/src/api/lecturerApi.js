
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

	update: (id, data) => {
		return axiosClient.put(`/Lecturers/${id}`, data)
	},

    delete: (id) => {
        return axiosClient.delete(`/Lecturers/${id}`)
    }
}

export default lecturerApi
