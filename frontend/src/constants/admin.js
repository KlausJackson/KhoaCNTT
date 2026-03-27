export const columns = [
	{
		key: 'id',
		label: 'ID'
	},
	{
		key: 'fullName',
		label: 'Tên người dùng'
	},
	{
		key: 'username',
		label: 'Tên đăng nhập'
	},
	{
		key: 'email',
		label: 'Email'
	},
	{
		key: 'level',
		label: 'Cấp quản trị'
	},
	{
		key: 'isActive',
		label: 'Trạng thái',
		render: (value) => ((value === 1 || value === true)? 'Đang hoạt động' : 'Bị khóa')
	}
]

export const fields = [
	{
		name: 'fullName',
		label: 'Tên người dùng',
		placeholder: 'Nhập tên',
	},
	{
		name: 'username',
		label: 'Tên đăng nhập',
		placeholder: 'Nhập username',
		disabled: true,
		readOnly: true
	},
	{
		name: 'email',
		label: 'Email',
		// type: 'email',
	},
	{
		name: 'password',
		label: 'Mật khẩu',
		type: 'password'
	},
	{
		name: 'level',
		label: 'Cấp quản trị',
		type: 'select',
		options: [
			{ label: 'Cấp 2', value: 2 },
			{ label: 'Cấp 3', value: 3 }
		]
	},
	{
		name: 'isActive',
		label: 'Trạng thái',
		type: 'select',
		options: [
			{ label: 'Đang hoạt động', value: true },
			{ label: 'Bị khóa', value: false }
		]
	}
]
