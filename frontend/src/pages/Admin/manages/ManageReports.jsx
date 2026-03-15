import { useState, useEffect } from 'react'
import DonutChart from '../../../components/parts/DonutChart'
import fileApi from '../../../api/fileApi'
// import newsApi from '../../../api/newsApi'

function ManageReports() {
	const [tab, setTab] = useState('files')
	const [typeStats, setTypeStats] = useState({})
	const [subjectStats, setSubjectStats] = useState({})
	const [traffic, setTraffic] = useState({})

	useEffect(() => {
		const loadStats = async () => {
			try {
				const typeRes = await fileApi.getStatsByType()
				const subjectRes = await fileApi.getStatsBySubject()
				const trafficRes = await fileApi.getStatsByTraffic()

				setTypeStats(typeRes)
				setSubjectStats(subjectRes)
				setTraffic(trafficRes)
			} catch (err) {
				console.error('Load stats error', err)
			}
		}
		loadStats()
	}, [])

	const totalFiles = Object.values(typeStats).reduce((a, b) => a + b, 0)

	return (
		<>
			<div className='max-w-5xl mx-auto px-4 py-12'>
				<h2 className='text-3xl font-bold text-[#1f4c7a] mb-8 border-l-4 border-red-500 pl-4'>
					Báo cáo thống kê
				</h2>
				<div className='flex border-b mb-6'>
					<button
						onClick={() => setTab('files')}
						className={`px-5 py-2 font-medium text-sm transition
		${
			tab === 'files'
				? 'border-b-2 border-red-500 text-[#1f4c7a]'
				: 'text-gray-500 hover:text-gray-700'
		}`}>
						File Statistics
					</button>

					<button
						onClick={() => setTab('news')}
						className={`px-5 py-2 font-medium text-sm transition
		${
			tab === 'news'
				? 'border-b-2 border-red-500 text-[#1f4c7a]'
				: 'text-gray-500 hover:text-gray-700'
		}`}>
						News Statistics
					</button>
				</div>

				<div className='grid grid-cols-4 gap-6'>
					{/* STATS */}
					<div className='space-y-4'>
						<div className='bg-white shadow rounded-xl p-4 border'>
							<p className='text-gray-500 text-sm'>Views</p>
							<p className='text-2xl font-bold text-blue-600'>
								{traffic.Views || 0}
							</p>
						</div>

						<div className='bg-white shadow rounded-xl p-4 border'>
							<p className='text-gray-500 text-sm'>Downloads</p>
							<p className='text-2xl font-bold text-green-600'>
								{traffic.Downloads || 0}
							</p>
						</div>

						<div className='bg-white shadow rounded-xl p-4 border'>
							<p className='text-gray-500 text-sm'>Total Files</p>
							<p className='text-2xl font-bold text-purple-600'>
								{totalFiles}
							</p>
						</div>
					</div>

					{/* FILE TYPE TABLE */}
					<div className='col-span-3 bg-white shadow rounded-xl p-4 border'>
						<h2 className='font-semibold mb-4 text-lg'>
							File Type Distribution
						</h2>

						<table className='w-full text-sm'>
							<thead className='text-gray-500 border-b'>
								<tr>
									<th className='text-left py-2'>Type</th>
									<th className='text-right py-2'>Files</th>
									<th className='text-right py-2'>%</th>
								</tr>
							</thead>

							<tbody>
								{Object.entries(typeStats).map(([k, v]) => {
									const percent = (
										(v / totalFiles) *
										100
									).toFixed(1)

									return (
										<tr
											key={k}
											className='border-b last:border-0'>
											<td className='py-2'>{k}</td>

											<td className='py-2 text-right font-semibold'>
												{v}
											</td>

											<td className='py-2 text-right text-gray-500'>
												{percent}%
											</td>
										</tr>
									)
								})}
							</tbody>
						</table>
					</div>
				</div>
				<div className='bg-white shadow rounded-xl p-6 border mt-6'>
					<h2 className='font-semibold mb-6 text-lg'>
						Subject Distribution
					</h2>

					<div className='flex gap-10 items-center'>
						<DonutChart data={subjectStats} />
					</div>
				</div>
			</div>
		</>
	)
}

export default ManageReports
