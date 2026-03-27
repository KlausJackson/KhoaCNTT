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
		setPopup?.({ message: res.message, type: 'success' })
	} catch (err) {
		handleError(err, setPopup)
	}
}

export const handleError = async (err, setPopup) => {
	const msg =
		err.response?.data?.message ||
		err.response?.data?.error ||
		err.response?.data?.detail ||
		err.message ||
		'Không thể kết nối đến máy chủ, thử lại sau.'
	setPopup?.({ message: msg, type: 'error' })
}
