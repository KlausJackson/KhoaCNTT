
import axiosClient from './axiosClient'

const studentApi = {
	loginAdmin: (credentials) => {
		return axiosClient.post('/Auth/login/admin', credentials)
	},

	loginStudent: (credentials) => {
		return axiosClient.post('/Auth/login/student', credentials)
	},

    getGrades: () => {
        return axiosClient.get("/Students/grades")
    },

    getSchedule: (id) => {
        return axiosClient.get(`/Students/schedule/${id}`)
    },

    getSubjects: () => {
        return axiosClient.get("/Subjects")
    },

    getSemesters: () => {
        // return axiosClient.get("/Students/semesters")
        // semesterid from 1-14
        return {
            1: "Học kỳ 1 (2019-2020)",
            2: "Học kỳ 2 (2019-2020)",
            3: "Học kỳ 1 (2020-2021)",
            4: "Học kỳ 2 (2020-2021)",
            5: "Học kỳ 1 (2021-2022)",
            6: "Học kỳ 2 (2021-2022)",
            7: "Học kỳ 1 (2022-2023)",
            8: "Học kỳ 2 (2022-2023)",
            9: "Học kỳ 1 (2023-2024)",
            10: "Học kỳ 2 (2023-2024)",
            11: "Học kỳ 1 (2024-2025)", 
            12: "Học kỳ 2 (2024-2025)",
            13: "Học kỳ 1 (2025-2026)",
            14: "Học kỳ 2 (2025-2026)",
        }
    }

}

export default studentApi
