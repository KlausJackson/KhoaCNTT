import { Outlet, Link, useLocation, useNavigate } from 'react-router-dom'
import { menu } from '../../constants/layout'
import { useEffect } from 'react'

function AdminLayout() {
	const location = useLocation()
	const navigate = useNavigate()
	const username = localStorage.getItem('username')
	const role = localStorage.getItem('role')
	const canViewAccounts = role === 'Admin1'

	// frontend based route protection
	// Redirect nếu vào /admin
	useEffect(() => {
		if (!canViewAccounts) {
			if (
				location.pathname === '/admin' ||
				location.pathname === '/admin/accounts'
			) {
				navigate('/admin/lecturers', { replace: true })
			}
		} else {
			if (location.pathname === '/admin') {
				navigate('/admin/accounts', { replace: true })
			}
		}
	}, [canViewAccounts, location.pathname, navigate])

	return (
		<div className='flex h-screen bg-gray-100'>
			{/* SIDEBAR */}

			<aside className='w-55 bg-blue-950 text-white flex flex-col'>
				<div className='px-5 py-5 border-b border-blue-800'>
					<h2 className='font-semibold text-lg'>Đại học Thủy lợi</h2>
					<p className='text-xs text-blue-200'>Admin Dashboard</p>
				</div>

				<nav className='flex-1 px-2 py-4 space-y-2'>
					{canViewAccounts === null
						? [...Array(5)].map((_, i) => (
								<div
									key={i}
									className='h-8 bg-blue-900 rounded animate-pulse'
								/>
							))
						: menu
								.filter((item) => {
									if (
										item.path === '/admin/accounts' &&
										!canViewAccounts
									)
										return false
									return true
								})
								.map((item) => {
									const Icon = item.icon
									const active =
										location.pathname === item.path

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
