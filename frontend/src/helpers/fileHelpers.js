import fileApi from '../api/fileApi'
import { handleError } from './commonHelpers'

export const getPagination = (page, totalPages) => {
	const pages = []

	const start = Math.max(1, page - 1)
	const end = Math.min(totalPages, page + 1)

	if (totalPages <= 5) {
		return Array.from({ length: totalPages }, (_, i) => i + 1)
	}

	if (start > 1) {
		pages.push(1)
		if (start > 2) pages.push('...')
	}

	for (let i = start; i <= end; i++) {
		pages.push(i)
	}

	if (end < totalPages) {
		if (end < totalPages - 1) pages.push('...')
		pages.push(totalPages)
	}
	return pages
}

export const normalizeFileSize = (size) => {
	if (size == null) return '-'
	if (size < 1024) return `${size} B`
	else if (size < 1024 * 1024) return `${(size / 1024).toFixed(2)} KB`
	else if (size < 1024 * 1024 * 1024)
		return `${(size / (1024 * 1024)).toFixed(2)} MB`
	else return `${(size / (1024 * 1024 * 1024)).toFixed(2)} GB`
}

export const handleDownload = async (id, file, setPopup) => {
	try {
		const blob = await fileApi.download(id)

		const url = window.URL.createObjectURL(blob)
		const link = document.createElement('a')

		link.href = url
		link.setAttribute('download', file)
		//console.log('Downloading file:', file)

		document.body.appendChild(link)
		link.click()
		link.remove()
	} catch (error) {
		let msg = 'Không thể kết nối đến máy chủ, thử lại sau.'

		if (error.response?.data instanceof Blob) {
			try {
				const text = await error.response.data.text()
				const json = JSON.parse(text)

				msg = json.message || json.error || json.detail || msg
			} catch {
				msg = await error.response.data.text()
			}
		} else {
			msg =
				error.response?.data?.message ||
				error.response?.data?.error ||
				error.response?.data?.detail ||
				error.message ||
				msg
		}

		setPopup({ message: msg, type: 'error' })
	}
}

export function checkSize(file, maxSizeMB) {
	if (!file || file.size === 0) return false

	const maxSize = parseInt(maxSizeMB)
	const maxSizeBytes = maxSize * 1024 * 1024

	return file.size <= maxSizeBytes
}

export const handleFormSubmit = async ({
	formData,
	type,
	extraData,
	onSuccess,
	setPopup
}) => {
	try {
		// ===== validate subject =====
		// console.log([...formData.entries()])

		// ===== validate file nếu có =====
		if (type === 'upload' || type === 'replace') {
			const file = formData.get('file')
			if (!file || file.size === 0) {
				setPopup({
					message: 'Vui lòng tải lên tài liệu.',
					type: 'error'
				})
				return
			}

			const ok = checkSize(file, '250MB')
			if (!ok) {
				setPopup({
					message: 'Tài liệu tải lên không được nặng hơn 250MB.',
					type: 'error'
				})
				return
			}
		}

		// ===== xử lý theo type =====
		let res

		if (type === 'upload') {
			res = await fileApi.upload(formData)
		}

		if (type === 'edit') {
			const data = Object.fromEntries(formData.entries())
			if (!data.subjectCode) delete data.subjectCode
			res = await fileApi.updateMetadata(extraData.id, data)
		}

		if (type === 'replace') {
			formData.append('Title', extraData.title)
			formData.append('SubjectCode', extraData.subjectCode || '')
			formData.append('FileType', extraData.fileType)
			formData.append('Permission', extraData.permission)

			res = await fileApi.replace(extraData.id, formData)
		}
		setPopup({ message: res.message, type: 'success' })
		onSuccess?.()
	} catch (err) {
		handleError(err, setPopup)
	}
}

export const handleApprove = async (
	isApproved,
	reason,
	id,
	setPopup,
	setSelected,
	loadRequests
) => {
	try {
		const res = await fileApi.approve(id, {
			isApproved,
			reason
		})
		setPopup({ message: res.message, type: 'success' })
		setSelected(null)
		loadRequests()
	} catch (err) {
		handleError(err, setPopup)
	}
}

export const mapToOptions = (map) =>
	Object.entries(map).map(([value, label]) => ({
		value,
		label
	}))
