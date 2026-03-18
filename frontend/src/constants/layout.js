import { Users, Newspaper, Folder, BarChart3, Home } from 'lucide-react'

const youtube = 'https://www.youtube.com/daihocthuyloi'
const facebook = 'https://www.facebook.com/daihocthuyloi1959/'

export const socialLinks = {
    youtube,
    facebook
}

export 	const navLinks = [
	{ path: '/', label: 'Trang chủ' },
	{ path: '/lecturers', label: 'Đội ngũ giảng viên' },
	{ path: '/files', label: 'Tài liệu học tập' },
	{ path: '/student-portal', label: 'Tra cứu học tập' },
]

export 	const menu = [
	{
		name: 'Tài khoản quản trị',
		path: '/admin/accounts',
		icon: Users
	},
	{
		name: 'Giảng viên',
		path: '/admin/lecturers',
		icon: Users
	},
	{
		name: 'Tin tức & Sự kiện',
		path: '/admin/news',
		icon: Newspaper 
	},
	{
		name: 'Tài liệu',
		path: '/admin/files',
		icon: Folder
	},
	{
		name: 'Báo cáo & Thống kê',
		path: '/admin/reports',
		icon: BarChart3
	},
	{
		name: 'Quay về Trang chủ',
		path: '/',
		icon: Home
	}
]