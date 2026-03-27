// File: src/helpers/newsHelpers.js

export const getPagination = (page, totalPages) => {
	const pages = []
	const start = Math.max(1, page - 2)
	const end = Math.min(totalPages, page + 2)

	if (start > 1) {
		pages.push(1)
		if (start > 2) pages.push('...')
	}
	for (let i = start; i <= end; i++) pages.push(i)
	if (end < totalPages) {
		if (end < totalPages - 1) pages.push('...')
		pages.push(totalPages)
	}
	return pages
}

export const formatDateTime = (date) => {
	if (!date) return ''
	return new Date(date).toLocaleString('vi-VN')
}

// BÍ DANH: Thêm dòng này để AdminNewsManagement và NewsList không bị lỗi khi gọi formatDate
export const formatDate = formatDateTime

export const timeAgo = (date) => {
	if (!date) return ''

	const diff = Date.now() - new Date(date).getTime()
	const min = Math.floor(diff / 60000)

	if (min < 1) return 'Vừa xong'
	if (min < 60) return `${min} phút trước`

	const hour = Math.floor(min / 60)
	if (hour < 24) return `${hour} giờ trước`

	const day = Math.floor(hour / 24)
	return `${day} ngày trước`
}
