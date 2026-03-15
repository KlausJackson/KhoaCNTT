
export function normalizeDate(date) {
	if (!date) return null

	const d = new Date(date)

	if (isNaN(d.getTime())) return null

	return d.toISOString().split('T')[0]
}


export const handleAction = async (action, data, setter, loader, setPopup) => {
	try {
		const res = await action(data)
		setter?.(false)
		loader?.()
		setPopup?.(res.message || "Thao tác thành công!")
	} catch (err) {
		const msg =
			err.response?.data?.message ||
			err.response?.data?.detail ||
			err.message ||
			'Có lỗi xảy ra'
		setPopup?.(msg)
	}
}