import { Outlet, Link, useLocation, useNavigate } from 'react-router-dom'
import { menu } from '../../constants/layout'
import adminApi from '../../api/adminApi'
import { useEffect, useState } from 'react'

function AdminLayout() {
	const location = useLocation()
	const navigate = useNavigate()
	const username = localStorage.getItem('username')

	const [canViewAccounts, setCanViewAccounts] = useState(true)

	// Redirect nếu vào /admin
	useEffect(() => {
		if (location.pathname === '/admin') {
			navigate('/admin/accounts')
		}
	}, [location.pathname, navigate])

	// Check quyền xem accounts
	useEffect(() => {
		const checkAccounts = async () => {
			try {
				await adminApi.getAll()
				setCanViewAccounts(true)
			} catch {
				setCanViewAccounts(false)
			}
		}

		checkAccounts()
	}, [])

	return (
		<div className='flex h-screen bg-gray-100'>
			{/* SIDEBAR */}

			<aside className='w-55 bg-blue-950 text-white flex flex-col'>
				<div className='px-5 py-5 border-b border-blue-800'>
					<h2 className='font-semibold text-lg'>Đại học Thủy lợi</h2>
					<p className='text-xs text-blue-200'>Admin Dashboard</p>
				</div>

				<nav className='flex-1 px-2 py-4 space-y-1'>
					{menu
						.filter((item) => {
							// Ẩn menu accounts nếu không có quyền
							if (
								item.path === '/admin/accounts' &&
								!canViewAccounts
							) {
								return false
							}
							return true
						})
						.map((item) => {
							const Icon = item.icon
							const active = location.pathname === item.path

							return (
								<Link
									key={item.path}
									to={item.path}
									className={`flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition
										${active ? 'bg-blue-800' : 'hover:bg-blue-900'}`}>
									{Icon && <Icon size={18} />}
									{item.name}
								</Link>
							)
						})}
				</nav>
			</aside>

			{/* MAIN */}

			<div className='flex-1 flex flex-col overflow-hidden'>
				{/* HEADER */}

				<header className='bg-white border-b px-6 py-3 flex justify-end gap-3 text-sm'>
					<p className='font-medium'>{username}</p>
				</header>

				{/* PAGE CONTENT */}

				<main className='flex-1 overflow-y-auto p-6'>
					<Outlet />
				</main>
			</div>
		</div>
	)
}

export default AdminLayout
