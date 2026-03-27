import React, { useState, useEffect } from 'react'
import fileApi from '../../api/fileApi'
import studentApi from '../../api/studentApi'
import { handleError } from '../../helpers/commonHelpers'
import { getSearchConfig } from '../../constants/file'
import PopupMessage from '../../components/parts/PopupMessage'
import FilterForm from '../../components/parts/FilterForm'
import Pagination from '../../components/table/Pagination'
import FileCard from '../../components/table/FileCard'

const FileList = () => {
	const [files, setFiles] = useState([])
	const [subjects, setSubjects] = useState([])
	const [popup, setPopup] = useState(null)
	const [isLoading, setIsLoading] = useState(false)

	const [page, setPage] = useState(1)
	const [totalPages, setTotalPages] = useState(0)
	const [filters, setFilters] = useState({
		keyword: '',
		subjectCodes: [],
		fileType: '',
		pageSize: 10
	})

	useEffect(() => {
		studentApi.getSubjects().then(setSubjects).catch(console.error)
	}, [])

	useEffect(() => {
		const fetchFiles = async () => {
			setIsLoading(true)
			try {
				const res = await fileApi.search({ ...filters, page })
				setFiles(res.items)
				const newTotal = Math.ceil(res.total / filters.pageSize)
				// setTotalPages(newTotal)
				// setPage((prev) => (prev > newTotal ? newTotal : prev))
				setTotalPages((prev) => Math.max(prev, newTotal))
			} catch (err) {
				handleError(err, setPopup)
			} finally {
				setIsLoading(false)
			}
		}
		fetchFiles()
	}, [page, filters])

	return (
		<div className='max-w-5xl mx-auto px-4 py-12'>
			<h2 className='text-3xl font-bold text-[#1f4c7a] mb-8 border-l-4 border-red-500 pl-4'>
				Tài liệu học tập
			</h2>

			<FilterForm
				fields={getSearchConfig(subjects)}
				onSearch={(values) => {
					setFilters(values)
					setPage(1) // Reset về trang 1
				}}
			/>
			<Pagination page={page} totalPages={totalPages} setPage={setPage} />

			{/* Danh sách File */}
			<div className='bg-white p-6 rounded-xl shadow-sm border border-gray-100'>
				<p className='text-sm text-gray-500 mb-6'>
					Danh sách các tài liệu, bài giảng và đề cương được chia sẻ.
				</p>

				{isLoading ? (
					<p className='text-center py-8 text-gray-500 font-medium'>
						Đang tải dữ liệu...
					</p>
				) : files.length === 0 ? (
					<p className='text-center py-8 text-gray-500 font-medium'>
						Không tìm thấy tài liệu phù hợp.
					</p>
				) : (
					<div className='space-y-4'>
						{files.map((file) => (
							<FileCard
								key={file.id}
								file={file}
								setPopup={setPopup}
							/>
						))}
					</div>
				)}
			</div>

			{popup && (
				<PopupMessage message={popup} onClose={() => setPopup(null)} />
			)}
		</div>
	)
}

export default FileList
