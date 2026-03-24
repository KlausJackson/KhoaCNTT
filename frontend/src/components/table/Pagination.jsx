import { getPagination } from '../../helpers/fileHelpers'

function Pagination({ page, totalPages, setPage }) {
	if (totalPages <= 1) return null

	return (
		<div className='flex justify-center mt-10 gap-2 flex-wrap'>
			<button
				disabled={page === 1}
				onClick={() => setPage(page - 1)}
				className='px-3 py-1 border rounded hover:bg-gray-50 disabled:opacity-50 transition'>
				‹
			</button>

			{getPagination(page, totalPages).map((p, index) => (
				<button
					key={index}
					disabled={p === '...'}
					onClick={() => p !== '...' && setPage(p)}
					className={`px-3 py-1 border rounded transition ${
						p === page
							? 'bg-[#1f4c7a] text-white font-medium shadow-sm'
							: 'hover:bg-gray-50'
					} ${p === '...' ? 'border-transparent' : ''}`}>
					{p}
				</button>
			))}

			<button
				disabled={page === totalPages}
				onClick={() => setPage(page + 1)}
				className='px-3 py-1 border rounded hover:bg-gray-50 disabled:opacity-50 transition'>
				›
			</button>
		</div>
	)
}

export default Pagination
