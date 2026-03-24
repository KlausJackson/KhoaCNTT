import fileApi from '../api/fileApi'

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
		let message = 'Không thể kết nối đến máy chủ, thử lại sau.'

		if (error.response?.data instanceof Blob) {
			try {
				const text = await error.response.data.text()
				const json = JSON.parse(text)

				message = json.message || json.error || json.detail || message
			} catch {
				message = await error.response.data.text()
			}
		} else {
			message =
				error.response?.data?.message ||
				error.response?.data?.error ||
				error.response?.data?.detail ||
				error.message ||
				message
		}

		setPopup(message)
	}
}

export function checkSize(file, maxSizeMB) {
	if (!file) return false

	const maxSize = parseInt(maxSizeMB)
	const maxSizeBytes = maxSize * 1024 * 1024

	return file.size <= maxSizeBytes
}
