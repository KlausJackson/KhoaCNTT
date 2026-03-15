import React, { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import lecturerApi from '../../api/lecturerApi'

const LecturerList = () => {
	const [lecturers, setLecturers] = useState([])

	useEffect(() => {
		const fetchLecturers = async () => {
			try {
				const res = await lecturerApi.getAll()
				setLecturers(res.items)
			} catch (error) {
				console.error('Lỗi tải giảng viên:', error)
			}
		}
		fetchLecturers()
	}, [])

	return (
		<div className='max-w-6xl mx-auto px-4 py-12'>
			<h2 className='text-3xl font-bold text-[#1f4c7a] mb-2 border-l-4 border-red-500 pl-4'>
				Đội ngũ Giảng viên
			</h2>
			<p className='text-gray-600 mb-10 pl-5 max-w-2xl'>
				Khoa Công nghệ thông tin quy tụ đội ngũ cán bộ, giảng viên tận
				tâm, giàu kinh nghiệm và có trình độ chuyên môn cao.
			</p>

			<div className='grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6'>
				{lecturers.map((lec) => (
					<Link
						to={`/lecturers/${lec.id}`}
						key={lec.id}
						className='bg-white p-6 rounded-xl shadow-sm border border-gray-100 hover:shadow-lg transition flex flex-col items-center text-center'>
						<div className='w-24 h-24 rounded-full overflow-hidden border-2 border-gray-100 mb-4'>
							<img
								src={
									lec.imageUrl ||
									'https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fcdn.pixabay.com%2Fphoto%2F2023%2F02%2F18%2F11%2F00%2Ficon-7797704_1280.png&f=1&nofb=1&ipt=e5b247863a2c27e373d974ae42d68d2c5d2573e6bc7ee94cfbcf3364a5b1f5a3'
								}
								alt={lec.fullName}
								className='w-full h-full object-cover'
							/>
						</div>
						<h3 className='font-bold text-[#1f4c7a] mb-1'>
							{lec.degree} {lec.fullName}
						</h3>
						<p className='text-sm text-gray-500'>{lec.position}</p>
					</Link>
				))}
			</div>
		</div>
	)
}

export default LecturerList
