import { useEffect, useState } from 'react'
import fileApi from '../../../api/fileApi'
import studentApi from '../../../api/studentApi'
import DataTable from '../../../components/table/DataTable'
import IconButton from '../../../components/parts/IconButton'
import FormModal from '../../../components/modal/FormModal'
import ConfirmModal from '../../../components/modal/ConfirmModal'
import FilterForm from '../../../components/parts/FilterForm'
import Pagination from '../../../components/table/Pagination'
import PopupMessage from '../../../components/parts/PopupMessage'
import {
	columns,
	uploadFields,
	replaceFields,
	editMetadataFields,
	getSearchConfig
} from '../../../constants/file'
import { handleDownload, handleFormSubmit } from '../../../helpers/fileHelpers'
import { Replace, Download, Trash2, Pencil } from 'lucide-react'
import { handleError, handleAction } from '../../../helpers/commonHelpers'

function FileList() {
	const [files, setFiles] = useState([])
	const [subjects, setSubjects] = useState([])
	const [showUpload, setShowUpload] = useState(false)

	// Các trạng thái Modal
	const [replaceFile, setReplaceFile] = useState(null)
	const [editFile, setEditFile] = useState(null)
	const [popup, setPopup] = useState(null)
	const [warning, setWarning] = useState(null)
	const role = localStorage.getItem('role')

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

	const loadFiles = async () => {
		try {
			const res = await fileApi.search({ ...filters, page })
			setFiles(res.items)
			const newTotal = Math.ceil(res.total / filters.pageSize)
			// setTotalPages(newTotal)
			// setPage((prev) => (prev > newTotal ? newTotal : prev))
			setTotalPages((prev) => Math.max(prev, newTotal))
		} catch (err) {
			handleError(err, setPopup)
		}
	}

	useEffect(() => {
		;(async () => {
			await loadFiles()
		})()
	}, [page, filters])

	return (
		<div>
			<div className='flex justify-between items-center mb-6'>
				<h2 className='text-xl font-bold text-gray-800'>
					Quản lý Tài liệu
				</h2>

				<button
					className='bg-[#1f4c7a] text-white px-5 py-2.5 rounded-lg hover:bg-[#163a5d] transition shadow-sm font-medium text-sm'
					onClick={() => setShowUpload(true)}>
					Thêm tài liệu mới
				</button>
			</div>

			<FilterForm
				fields={getSearchConfig(subjects)}
				onSearch={(values) => {
					setFilters(values)
					setPage(1)
				}}
			/>
			<div className='my-4'>
				<Pagination
					page={page}
					totalPages={totalPages}
					setPage={setPage}
				/>
			</div>

			<DataTable
				columns={columns}
				data={files}
				actions={(row) => (
					<>
						<IconButton
							icon={Pencil}
							color='gray'
							onClick={() => setEditFile(row)}
						/>
						<IconButton
							icon={Replace}
							color='blue'
							onClick={() => setReplaceFile(row)}
						/>
						<IconButton
							icon={Download}
							color='green'
							onClick={() =>
								handleDownload(row.id, row.title, setPopup)
							}
						/>
						<IconButton
							icon={Trash2}
							color='red'
							onClick={() => {
								if (role === 'Admin3') {
									setPopup({
										message:
											'Bạn không có quyền xóa tài liệu.',
										type: 'error'
									})
									return
								}
								setWarning({
									title: 'Xác nhận xóa tài liệu',
									message:
										'Bạn có chắc chắn muốn xóa tài liệu này?',
									action: () => fileApi.delete(row.id),
									popup: 'Xóa tài liệu thành công.',
									color: 'red',
									icon: "mdi:delete-outline"
								})
							}}
						/>
					</>
				)}
			/>

			{/* Modal Upload */}
			{showUpload && (
				<FormModal
					title='Thêm tài liệu mới'
					fields={uploadFields}
					columns={2}
					width='600px'
					confirmText='Thêm tài liệu'
					onSubmit={async (formData) =>
						handleFormSubmit({
							formData,
							type: 'upload',
							onSuccess: () => {
								setShowUpload(false)
								loadFiles()
							},
							setPopup
						})
					}
					onClose={() => setShowUpload(false)}
				/>
			)}

			{/* Modal Edit Metadata (Sửa tiêu đề, quyền, môn học) */}
			{editFile && (
				<FormModal
					title='Sửa thông tin tài liệu'
					fields={editMetadataFields}
					defaultValues={editFile}
					confirmText='Lưu thay đổi'
					onSubmit={async (formData) =>
						handleFormSubmit({
							formData,
							type: 'edit',
							extraData: editFile,
							onSuccess: () => {
								setEditFile(null)
								loadFiles()
							},
							setPopup
						})
					}
					onClose={() => setEditFile(null)}
				/>
			)}

			{/* Modal Replace */}
			{replaceFile && (
				<FormModal
					title='Đổi tài liệu'
					fields={replaceFields}
					confirmText='Đổi tài liệu'
					defaultValues={{ oldTitle: replaceFile.title }}
					onSubmit={async (formData) =>
						handleFormSubmit({
							formData,
							type: 'replace',
							extraData: replaceFile,
							onSuccess: () => {
								setReplaceFile(null)
								loadFiles()
							},
							setPopup
						})
					}
					onClose={() => setReplaceFile(null)}
				/>
			)}

			{popup && (
				<PopupMessage
					message={popup.message}
					type={popup.type}
					onClose={() => setPopup(null)}
				/>
			)}

			{warning && (
				<ConfirmModal
					title={warning.title}
					message={warning.message}
					color={warning.color}
					icon={warning.icon}
					onConfirm={async () => {
						await handleAction(
							warning.action,
							null,
							setWarning,
							loadFiles,
							setPopup,
							warning.popup
						)
					}}
					onClose={() => setWarning(null)}
					confirmText='Xác nhận'
				/>
			)}
		</div>
	)
}

export default FileList
