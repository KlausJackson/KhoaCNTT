import React, { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import lecturerApi from '../../api/lecturerApi'
import Button from '../../components/parts/Button'

const LecturerDetail = () => {
	const { id } = useParams()
	const [lecturer, setLecturer] = useState(null)

	useEffect(() => {
		const fetchDetail = async () => {
			try {
				const res = await lecturerApi.getById(id)
				setLecturer(res)
			} catch (error) {
				console.error('Lỗi tải chi tiết:', error)
			}
		}
		fetchDetail()
	}, [id])

	if (!lecturer)
		return <div className='text-center py-20'>Đang tải thông tin...</div>

	return (
		<div className='max-w-4xl mx-auto px-4 py-12'>
			<Button link='/lecturers' message='Quay lại danh sách' />

			<div className='bg-white p-8 rounded-xl shadow-sm border border-gray-100 flex flex-col md:flex-row gap-8'>
				{/* Avatar */}
				<div className='w-48 h-48 flex-shrink-0 rounded-xl overflow-hidden shadow-md mx-auto md:mx-0'>
					<img
						src={
							lecturer.imageUrl ||
							'https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fcdn.pixabay.com%2Fphoto%2F2023%2F02%2F18%2F11%2F00%2Ficon-7797704_1280.png&f=1&nofb=1&ipt=e5b247863a2c27e373d974ae42d68d2c5d2573e6bc7ee94cfbcf3364a5b1f5a3'
						}
						alt={lecturer.fullName}
						className='w-full h-full object-cover'
					/>
				</div>

				{/* Info */}
				<div className='flex-1'>
					<h2 className='text-3xl font-bold text-[#1f4c7a] mb-2'>
						{lecturer.degree} {lecturer.fullName}
					</h2>
					<p className='text-lg text-gray-600 font-medium border-b pb-4 mb-4'>
						{lecturer.position}
					</p>

					<div className='space-y-3 text-gray-700'>
						<p>
							<strong className='w-32 inline-block'>
								Email:
							</strong>{' '}
							{lecturer.email}
						</p>
						<p>
							<strong className='w-32 inline-block'>
								Điện thoại:
							</strong>{' '}
							{lecturer.phoneNumber}
						</p>
						{lecturer.birthdate && (
							<p>
								<strong className='w-32 inline-block'>
									Ngày sinh:
								</strong>{' '}
								{new Date(
									lecturer.birthdate
								).toLocaleDateString('vi-VN')}
							</p>
						)}
					</div>

					<div className='mt-8'>
						<h3 className='font-bold text-lg mb-3'>
							Môn học tham gia giảng dạy
						</h3>
						<div className='flex flex-wrap gap-2'>
							{lecturer.lecturerSubjects &&
							lecturer.lecturerSubjects.length > 0 ? (
								lecturer.lecturerSubjects.map((ls, idx) => (
									<span
										key={idx}
										className='px-3 py-1 bg-blue-50 text-[#1f4c7a] rounded-full text-sm border border-blue-100'>
										{ls.subject?.subjectName ||
											ls.subjectId}
									</span>
								))
							) : (
								<span className='text-gray-400 italic'>
									Đang cập nhật...
								</span>
							)}
						</div>
					</div>
				</div>
			</div>
		</div>
	)
}

export default LecturerDetail
