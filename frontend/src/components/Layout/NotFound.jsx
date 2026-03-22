function NotFound() {
	return (
		<div className='flex items-center justify-center h-screen bg-gray-100'>
			<div className='text-center'>
				<h1 className='text-7xl font-bold text-blue-900'>404</h1>

				<p className='mt-4 text-xl text-gray-700'>
					Trang bạn tìm không tồn tại
				</p>

				<div className='mt-6 flex justify-center gap-3'>
					<button
						onClick={() => window.history.back()}
						className='px-4 py-2 bg-gray-200 rounded-lg hover:bg-gray-300 text-sm'>
						Quay lại
					</button>

					<a
						href='/'
						className='px-4 py-2 bg-blue-800 text-white rounded-lg hover:bg-blue-900 text-sm'>
						Về trang chủ
					</a>
				</div>
			</div>
		</div>
	)
}

export default NotFound
