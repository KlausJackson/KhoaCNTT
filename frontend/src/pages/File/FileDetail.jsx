import React, { useState, useEffect } from 'react'
import { useParams, useLocation } from 'react-router-dom'
import fileApi from '../../api/fileApi'
import { handleDownload } from '../../helpers/fileHelpers'
import { fileTypeMap, permissionMap } from '../../constants/file'
import PopupMessage from '../../components/parts/PopupMessage'
import Button from '../../components/parts/Button'

const FileDetail = () => {
	const { id } = useParams()
	const location = useLocation()
	const [popup, setPopup] = useState(null)
	const [file, setFile] = useState(location.state || null)
	const [error, setError] = useState(null)
	const [previewUrl, setPreviewUrl] = useState(null)
	const [previewError, setPreviewError] = useState(null)

	useEffect(() => {
		if (!file) {
			fileApi
				.getById(id)
				.then((res) => setFile(res))
				.catch(() => setError('Không tải được dữ liệu file'))
		}
	}, [id])

	useEffect(() => {
		const loadPreview = async () => {
			try {
				const res = await fetch(streamUrl)

				if (!res.ok) {
					const data = await res.json()
					throw new Error(data.detail)
				}

				const blob = await res.blob()
				const url = URL.createObjectURL(blob)

				setPreviewUrl(url)
			} catch (err) {
				setPreviewError(err.message)
			}
		}

		loadPreview()
	}, [id])

	useEffect(() => {
		if (popup) {
			const t = setTimeout(() => setPopup(null), 3000)
			return () => clearTimeout(t)
		}
	}, [popup])

	// URL preview
	const streamUrl = `https://localhost:7108/api/Files/${id}`

	return (
		<div className='max-w-7xl mx-auto px-4 py-8'>
			<Button link='/files' message='Quay lại danh sách' />

			<div className='flex flex-col lg:flex-row gap-8'>
				{/* THÔNG TIN TÀI LIỆU */}
				<div className='w-full lg:w-1/3 bg-white p-6 rounded-xl shadow-sm border border-gray-100 h-fit'>
					<div className='flex items-center gap-3 mb-4'>
						<div className='text-red-500 text-4xl'>📄</div>
						<h2 className='text-2xl font-bold text-[#1f4c7a] leading-tight'>
							{file.title}
						</h2>
					</div>

					<div className='space-y-4 text-gray-700 mt-6 border-t pt-4'>
						<p className='flex justify-between border-b pb-2'>
							<span className='font-medium'>Môn học:</span>
							<span className='font-bold text-[#1f4c7a]'>
								{file.subjectCode || 'Tài liệu chung'}
							</span>
						</p>

						<p className='flex justify-between border-b pb-2'>
							<span className='font-medium'>Loại tài liệu:</span>
							<span>{fileTypeMap[file.fileType] || file.fileType}</span>
						</p>

						<p className='flex justify-between border-b pb-2'>
							<span className='font-medium'>Kích thước:</span>
							<span>
								{(file.size / 1024 / 1024).toFixed(2)} MB
							</span>
						</p>

						<p className='flex justify-between border-b pb-2'>
							<span className='font-medium'>Lượt xem:</span>
							<span className='text-blue-600 font-bold'>
								{file.viewCount}
							</span>
						</p>

						<p className='flex justify-between pb-2'>
							<span className='font-medium'>Lượt tải:</span>
							<span className='text-green-600 font-bold'>
								{file.downloadCount}
							</span>
						</p>
						<p className='flex justify-between border-b pb-2'>
							<span className='font-medium'>File name:</span>
							<span>{file.fileName}</span>
						</p>

						<p className='flex justify-between border-b pb-2'>
							<span className='font-medium'>Permission:</span>
							<span>{permissionMap[file.permission] || file.permission}</span>
						</p>

						<p className='flex justify-between border-b pb-2'>
							<span className='font-medium'>Uploaded:</span>
							<span>
								{new Date(file.createdAt).toLocaleString()}
							</span>
						</p>
					</div>

					<button
						onClick={() => handleDownload(id, file.fileName, setPopup)}
						className='w-full mt-8 bg-[#1f4c7a] text-white py-3 rounded-lg font-medium hover:bg-[#163a5d] transition flex items-center justify-center gap-2 shadow-md'>
						Tải tài liệu này về máy
					</button>
				</div>

				{/* PREVIEW */}
				<div className='w-full lg:w-2/3 bg-gray-100 rounded-xl overflow-hidden shadow-inner border border-gray-200 h-[800px] flex flex-col'>
					<div className='bg-[#1f4c7a] text-white px-4 py-2 text-sm font-medium flex justify-between'>
						<span>Chế độ xem trước</span>
						<span>{file.title}</span>
					</div>

					{previewError ? (
						<div className='flex items-center justify-center h-full text-red-600 font-medium'>
							{previewError}
						</div>
					) : previewUrl ? (
						<iframe
							src={previewUrl}
							className='w-full h-full bg-white'
							title='File Preview'
						/>
					) : (
						<div className='flex items-center justify-center h-full text-gray-500'>
							Đang tải xem trước...
						</div>
					)}
				</div>
			</div>
			{popup && (
                <PopupMessage message={popup} onClose={() => setPopup(null)} />
			)}
		</div>
	)
}

export default FileDetail
