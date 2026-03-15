export const handleAction = async (action, data, setter, loader, setPopup) => {
	try {
		const res = await action(data)
		setter?.(false)
		loader()
		setPopup(res.message)
	} catch (err) {
		const msg =
			err.response?.data?.message ||
			err.response?.data?.detail ||
			err.message ||
			'Có lỗi xảy ra'
		setPopup(msg)
	}
}
