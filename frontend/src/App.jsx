import { BrowserRouter, Routes, Route } from 'react-router-dom'

// Import Layout & Pages
import UserLayout from './components/Layout/UserLayout'
import LoginPage from './pages/Auth/LoginPage'
import HomePage from './pages/Home/HomePage'

import FileList from './pages/File/FileList'
import FileDetail from './pages/File/FileDetail'

import NewsList from './pages/News/NewsList'
import NewsDetail from './pages/News/NewsDetail'

import LecturerList from './pages/Lecturer/LecturerList'
import LecturerDetail from './pages/Lecturer/LecturerDetail'

import StudentPortal from './pages/Student/StudentPortal'

// Admin Pages
import AdminLayout from './components/Layout/AdminLayout'
import ManageAdmin from './pages/Admin/manages/ManageAdmins'
import ManageFiles from './pages/Admin/file/ManageFiles'
import ManageLecturers from './pages/Admin/manages/ManageLecturers'
import ManageReports from './pages/Admin/manages/ManageReports'
import AdminNewsManagement from './pages/News/Admin/AdminNewsManagement'

const HomePlaceholder = () => (
	<div className='p-12 text-center text-2xl font-bold'>
		Trang chủ / Tin tức (Đang phát triển)
	</div>
)

function App() {
	return (
		<BrowserRouter>
			<Routes>
				{/* Route Đăng nhập độc lập không có Header/Footer */}
				<Route path='/login' element={<LoginPage />} />

				{/* Các trang User bọc trong UserLayout */}
				<Route path='/' element={<UserLayout />}>
					<Route index element={<HomePage />} />
					<Route path='news' element={<NewsList />} />
					<Route path='news/:id' element={<NewsDetail />} />
					{/* Trang chủ */}
					<Route path='lecturers' element={<LecturerList />} />
					<Route path='lecturers/:id' element={<LecturerDetail />} />
					<Route path='files' element={<FileList />} />
					<Route path='files/:id' element={<FileDetail />} />
					<Route path='student-portal' element={<StudentPortal />} />
				</Route>

				{/* Route Admin */}
				<Route path='/admin' element={<AdminLayout />}>
					<Route path='accounts' element={<ManageAdmin />} />
					<Route path='files' element={<ManageFiles />} />
					<Route path='lecturers' element={<ManageLecturers />} />
					<Route path='reports' element={<ManageReports />} />
					<Route path='news' element={<AdminNewsManagement />} />
				</Route>
			</Routes>
		</BrowserRouter>
	)
}

export default App
