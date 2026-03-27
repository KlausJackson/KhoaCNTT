import React, { useState } from 'react'
import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom'
import { socialLinks, navLinks } from '../../constants/layout'
import { Icon } from '@iconify/react'
import ConfirmModal from '../modal/ConfirmModal'

const UserLayout = () => {
	const navigate = useNavigate()
	const location = useLocation()
	const [isDropdownOpen, setIsDropdownOpen] = useState(false)
	const [popUp, setPopup] = useState(null)

	// Kiểm tra xem đã đăng nhập chưa
	const username = localStorage.getItem('username')
	const role = localStorage.getItem('role')

	const isAdmin = role && role.toLowerCase().includes('admin') // Kiểm tra nếu role chứa "admin"

	const handleLogout = () => {
		localStorage.removeItem('token')
		localStorage.removeItem('role')
		localStorage.removeItem('username')
		navigate('/') // Đá về trang chủ sau khi logout
	}

	return (
		<div className='min-h-screen flex flex-col bg-slate-50'>
			{popUp && (
				<ConfirmModal
					title={popUp.title}
					message={popUp.message}
					color={popUp.color}
					icon={popUp.icon}
					onConfirm={() => {
						setPopup(null)
						handleLogout()}
					}
					onClose={() => setPopup(null)}
					confirmText='Xác nhận'
				/>
			)}
			{/* Top Bar */}
			<div className='bg-[#0f172a] text-white text-xs py-2 px-8 flex justify-between items-center'>
				<div className='flex gap-6'></div>
				<div className='flex gap-4 items-center'>
					<a
						href={socialLinks.facebook}
						className='hover:text-blue-400'>
						Facebook
					</a>
					<a
						href={socialLinks.youtube}
						className='hover:text-red-400'>
						Youtube
					</a>
					<span className='text-gray-500'>|</span>
					{username ? (
						<div className='relative'>
							<button
								onClick={() =>
									setIsDropdownOpen(!isDropdownOpen)
								}
								className='flex items-center gap-2 hover:text-blue-300'>
								{username} ▾
							</button>
							{isDropdownOpen && (
								<div className='absolute right-0 mt-2 w-48 bg-white text-black rounded shadow-lg py-2 z-50'>
									{/* Nếu là admin thì hiện nút Trang quản trị */}
									{isAdmin && (
										<Link
											to='/admin'
											className='block px-4 py-2 hover:bg-gray-100'>
											Trang quản trị
										</Link>
									)}
									<button
										onClick={() => {
											setPopup({
												title: 'Xác nhận đăng xuất',
												message: 'Bạn có chắc chắn muốn đăng xuất?',
												color: 'blue',
												icon: 'mdi:logout'
											})
										}}
										className='w-full text-left px-4 py-2 hover:bg-gray-100 text-red-600'>
										Đăng xuất
									</button>
								</div>
							)}
						</div>
					) : (
						<Link to='/login' className='hover:text-blue-300'>
							Đăng nhập
						</Link>
					)}
				</div>
			</div>

			{/* Main Header */}
			<header className='bg-white shadow-sm sticky top-0 z-40'>
				<div className='max-w-7xl mx-auto px-4 py-4 flex justify-between items-center'>
					<Link to='/' className='flex items-center gap-3'>
						<img src='/logo.jpg' alt='Logo' className='h-12' />
						<div>
							<h1 className='text-[#1f4c7a] font-bold text-lg leading-tight uppercase'>
								Khoa Công nghệ thông tin
							</h1>
							<p className='text-gray-500 text-xs'>
								Trường Đại học Thủy Lợi - TLU
							</p>
						</div>
					</Link>

					<nav className='hidden md:flex gap-1'>
						{navLinks.map((link) => (
							<Link
								key={link.path}
								to={link.path}
								className={`px-4 py-2 rounded-full text-sm font-medium transition-colors ${
									location.pathname === link.path
										? 'bg-[#1f4c7a] text-white'
										: 'text-gray-700 hover:bg-blue-50 hover:text-[#1f4c7a]'
								}`}>
								{link.label}
							</Link>
						))}
					</nav>
				</div>
			</header>

			{/* Main Content (Đổ các trang con vào đây) */}
			<main className='flex-grow'>
				<Outlet />
			</main>

			{/* Footer */}
			<footer className='bg-[#0f172a] text-white pt-12 pb-6'>
				<div className='max-w-7xl mx-auto px-4 grid grid-cols-1 md:grid-cols-3 gap-8 mb-8'>
					<div>
						<h3 className='font-bold text-lg mb-4 uppercase'>
							Khoa Công Nghệ Thông Tin
						</h3>
						<p className='text-gray-400 text-sm mb-2'>
							Trường Đại học Thủy Lợi - TLU
						</p>
						<p className='text-gray-400 text-sm mb-2'>
							📍 175 Tây Sơn, Đống Đa, Hà Nội
						</p>
						<p className='text-gray-400 text-sm'>
							✉️ vpkcntt@tlu.edu.vn
						</p>
					</div>
					<div>
						<h3 className='font-bold text-lg mb-4'>
							Liên kết hữu ích
						</h3>
						<ul className='text-gray-400 text-sm space-y-2'>
							<li>
								<a
									href='#'
									className='hover:text-white transition'>
									Giới thiệu Khoa
								</a>
							</li>
							<li>
								<a
									href='#'
									className='hover:text-white transition'>
									Tuyển sinh 2026
								</a>
							</li>
							<li>
								<a
									href='/lecturers'
									className='hover:text-white transition'>
									Đội ngũ Giảng viên
								</a>
							</li>
							<li>
								<a
									href='#'
									className='hover:text-white transition'>
									Quy chế Đào tạo
								</a>
							</li>
						</ul>
					</div>
					<div>
						<h3 className='font-bold text-lg mb-4'>
							Kết nối với chúng tôi
						</h3>
						<div className='flex gap-4'>
							<a href='https://www.facebook.com/daihocthuyloi1959/'>
								<Icon
									icon='mdi:facebook'
									width='50'
									className='text-blue-500 hover:text-blue-400 transition'
								/>
							</a>

							<a href='https://www.youtube.com/daihocthuyloi'>
								<Icon
									icon='mdi:youtube'
									width='50'
									className='text-red-500 hover:text-red-400 transition'
								/>
							</a>
						</div>
					</div>
				</div>
				<div className='text-center text-gray-500 text-xs border-t border-white/10 pt-6'>
					© 2026 Faculty of Information Technology - Thuy Loi
					University. All rights reserved.
				</div>
			</footer>
		</div>
	)
}

export default UserLayout
