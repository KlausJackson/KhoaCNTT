import { useNavigate } from 'react-router-dom'
import { handleDownload, normalizeFileSize } from '../../helpers/fileHelpers'

function FileCard({ file, setPopup }) {
	const navigate = useNavigate()

	return (
		<div className='bg-white border rounded-xl shadow-sm hover:shadow-md transition p-5 flex flex-col lg:flex-row md:items-center md:justify-between gap-4'>
			<div className='flex gap-4 flex-1'>
				<div className='text-3xl text-gray-400 mt-1'>📄</div>
				<div className='flex-1'>
					<h3 className='text-lg font-semibold text-gray-800'>
						{file.title}
					</h3>
					<p className='text-sm text-gray-400 break-all'>
						{file.fileName}
					</p>

					<div className='flex flex-wrap gap-2 mt-2'>
						{file.subjectName !== 'Môn chung' && (
							<span className='bg-blue-100 text-blue-700 px-2 py-1 rounded text-xs font-medium'>
								{file.subjectName} ({file.subjectCode})
							</span>
						)}
						<span className='bg-gray-100 text-gray-700 px-2 py-1 rounded text-xs'>
							{file.fileType}
						</span>
						<span className='bg-gray-100 text-gray-700 px-2 py-1 rounded text-xs'>
							{file.permission}
						</span>
					</div>

					<div className='grid grid-cols-2 sm:grid-cols-4 gap-3 text-xs text-gray-500 mt-3'>
						<p>
							Lượt xem:{' '}
							<span className='font-semibold text-gray-700'>
								{file.viewCount}
							</span>
						</p>
						<p>
							Lượt tải:{' '}
							<span className='font-semibold text-gray-700'>
								{file.downloadCount}
							</span>
						</p>
						<p>
							Size:{' '}
							<span className='font-semibold text-gray-700'>
								{normalizeFileSize(file.fileSize)} 
							</span>
						</p>
						<p>
							Ngày tải:{' '}
							<span className='font-semibold text-gray-700'>
								{new Date(file.createdAt).toLocaleDateString()}
							</span>
						</p>
					</div>
				</div>
			</div>

			<div className='flex gap-2 md:flex-col lg:flex-row'>
				<button
					onClick={() =>
						navigate(`/files/${file.id}`, { state: file })
					}
					className='px-4 py-2 border border-[#1f4c7a] text-[#1f4c7a] font-medium rounded-md text-sm hover:bg-blue-50 transition'>
					Xem chi tiết
				</button>
				<button
					onClick={() =>
						handleDownload(file.id, file.fileName, setPopup)
					}
					className='px-4 py-2 bg-[#1f4c7a] text-white font-medium rounded-md text-sm hover:bg-[#163a5d] transition shadow-sm'>
					Tải xuống
				</button>
			</div>
		</div>
	)
}

export default FileCard
