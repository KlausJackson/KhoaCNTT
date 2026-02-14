using AutoMapper;
using KhoaCNTT.Application.DTOs;
using KhoaCNTT.Application.DTOs.File;
using KhoaCNTT.Domain.Entities;

namespace KhoaCNTT.Application.Common.Utils
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Cấu hình map 2 chiều: Từ Entity -> DTO và ngược lại
            CreateMap<FileResource, FileResourceDto>().ReverseMap();
            // CreateMap<News, NewsDto>().ReverseMap();
            // CreateMap<Lecture, LectureDto>().ReverseMap();
            // CreateMap<Comment, CommentDto>().ReverseMap();
        }
    }
}