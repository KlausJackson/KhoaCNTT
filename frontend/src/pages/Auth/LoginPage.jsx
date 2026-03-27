import React, { useState } from 'react'
import authApi from '../../api/authApi'
import { handleError } from '../../helpers/commonHelpers'
import { useNavigate } from 'react-router-dom'

const LoginPage = () => {
	const [username, setUsername] = useState('')
	const [password, setPassword] = useState('')
	const [showPassword, setShowPassword] = useState(false)

	const [error, setError] = useState(null)
	const [isLoading, setIsLoading] = useState(false)
	const navigate = useNavigate()

	const handleLogin = async (role) => {
		if (!username || !password) {
			setError('Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.')
			return
		}

		setIsLoading(true)
		setError(null)

		try {
			let response

			if (role === 'admin') {
				response = await authApi.loginAdmin({ username, password })
			} else {
				response = await authApi.loginStudent({ username, password })
			}

			console.log('Login success:', response)
			localStorage.setItem('token', response.token)
			localStorage.setItem('role', response.role)
			localStorage.setItem('username', username)
			navigate('/')
		} catch (err) {
			handleError(err, setError)
		} finally {
			setIsLoading(false)
		}
	}

	return (
		<div className='min-h-screen flex items-center justify-center bg-slate-200 p-6'>
			<div className='w-full max-w-5xl bg-white rounded-2xl shadow-xl flex overflow-hidden'>
				{/* LEFT PANEL */}
				<div className='w-1/2 bg-gradient-to-br from-[#2f6fad] to-[#1f4c7a] text-white p-12 flex flex-col justify-center'>
					<h1 className='text-3xl font-bold mb-4'>
						Chào mừng trở lại!
					</h1>

					<p className='text-sm text-blue-100 mb-8 leading-relaxed'>
						Cổng thông tin Khoa Công nghệ thông tin - Trường Đại học
						Thủy Lợi. Nền tảng kết nối giảng viên và sinh viên.
					</p>

					<ul className='space-y-4 text-sm'>
						<li className='flex items-center gap-3'>
							<span className='bg-white/20 p-1 rounded-full'>
								<svg
									className='w-4 h-4'
									fill='none'
									stroke='currentColor'
									viewBox='0 0 24 24'>
									<path
										strokeWidth='2'
										strokeLinecap='round'
										strokeLinejoin='round'
										d='M5 13l4 4L19 7'
									/>
								</svg>
							</span>
							Tra cứu điểm & Thời khóa biểu
						</li>

						<li className='flex items-center gap-3'>
							<span className='bg-white/20 p-1 rounded-full'>
								<svg
									className='w-4 h-4'
									fill='none'
									stroke='currentColor'
									viewBox='0 0 24 24'>
									<path
										strokeWidth='2'
										strokeLinecap='round'
										strokeLinejoin='round'
										d='M5 13l4 4L19 7'
									/>
								</svg>
							</span>
							Cập nhật tin tức Khoa nhanh chóng
						</li>

						<li className='flex items-center gap-3'>
							<span className='bg-white/20 p-1 rounded-full'>
								<svg
									className='w-4 h-4'
									fill='none'
									stroke='currentColor'
									viewBox='0 0 24 24'>
									<path
										strokeWidth='2'
										strokeLinecap='round'
										strokeLinejoin='round'
										d='M5 13l4 4L19 7'
									/>
								</svg>
							</span>
							Kho tài liệu học tập phong phú
						</li>
					</ul>
				</div>

				{/* RIGHT PANEL */}
				<div className='w-1/2 p-12 flex flex-col'>
					{/* LOGO */}
					<div className='flex items-center gap-3 mb-8'>
						<img src='/logo.jpg' className='h-12' />
						<div>
							<h2 className='text-[#1f4c7a] font-bold leading-tight'>
								ĐẠI HỌC THỦY LỢI
							</h2>
							<p className='text-gray-500 text-xs tracking-wider'>
								KHOA CNTT
							</p>
						</div>
					</div>

					<h2 className='text-xl font-semibold text-gray-800 mb-6'>
						Đăng nhập hệ thống
					</h2>

					{error && (
						<div className='bg-red-100 text-red-600 px-4 py-2 rounded-md mb-6 text-sm'>
							{error}
						</div>
					)}

					{/* USERNAME */}
					<div className='mb-4'>
						<label className='text-sm text-gray-600'>
							Mã sinh viên/Username
						</label>

						<input
							type='text'
							placeholder='Nhập tài khoản của bạn...'
							className='w-full mt-1 border border-gray-300 px-4 py-2 rounded-md focus:ring-2 focus:ring-blue-500 focus:outline-none'
							value={username}
							onChange={(e) => setUsername(e.target.value)}
						/>
					</div>

					{/* PASSWORD */}
					<div className='mb-6'>
						<label className='text-sm text-gray-600'>
							Mật khẩu
						</label>

						<div className='relative'>
							<input
								type={showPassword ? 'text' : 'password'}
								placeholder='••••••••'
								className='w-full mt-1 border border-gray-300 px-4 py-2 rounded-md focus:ring-2 focus:ring-blue-500 focus:outline-none'
								value={password}
								onChange={(e) => setPassword(e.target.value)}
							/>

							<button
								type='button'
								className='absolute right-3 top-3 text-gray-400'
								onClick={() => setShowPassword(!showPassword)}>
								👁
							</button>
						</div>
					</div>

					{/* BUTTONS */}
					<div className='flex gap-4'>
						<button
							type='button'
							onClick={() => handleLogin('student')}
							disabled={isLoading}
							className='flex-1 bg-[#1f4c7a] text-white py-2 rounded-md hover:bg-[#163a5d] transition flex items-center justify-center gap-2'>
							Sinh viên
						</button>

						<button
							type='button'
							onClick={() => handleLogin('admin')}
							disabled={isLoading}
							className='flex-1 border border-[#1f4c7a] text-[#1f4c7a] py-2 rounded-md hover:bg-blue-50 transition flex items-center justify-center gap-2'>
							Quản trị viên
						</button>
					</div>

					<div className='text-center text-xs text-gray-400 mt-8'>
						© 2026 Faculty of Information Technology - TLU
					</div>
				</div>
			</div>
		</div>
	)
}

export default LoginPage
