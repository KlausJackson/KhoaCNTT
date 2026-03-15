import fileApi from '../api/fileApi'

export const getPagination = (page, totalPages) => {
	const pages = []

	const start = Math.max(1, page - 2)
	const end = Math.min(totalPages, page + 2)

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
	else if (size < 1024 * 1024 * 1024) return `${(size / (1024 * 1024)).toFixed(2)} MB`
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
		const message =
			error.response?.data?.detail ||
			error.response?.data?.message ||
			'Bạn không có quyền tải file này.'

		setPopup(message)
	}
}
