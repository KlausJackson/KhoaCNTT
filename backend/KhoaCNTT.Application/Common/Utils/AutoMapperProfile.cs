using AutoMapper;
using KhoaCNTT.Application.DTOs;
using KhoaCNTT.Application.DTOs.Admin;
using KhoaCNTT.Application.DTOs.File;
using KhoaCNTT.Application.DTOs.Lecturer;
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Entities.FileEntities;

namespace KhoaCNTT.Application.Common.Utils
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Cấu hình map 2 chiều: Từ Entity -> DTO và ngược lại

            CreateMap<FileEntity, FileDto>()
            .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType.ToString()))

            // Nếu SubjectCode bị null thì gán chuỗi rỗng
            .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.SubjectCode ?? ""))

            // Kích thước file lấy từ CurrentResource
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.CurrentResource != null ? src.CurrentResource.Size : 0))
            // FileName 
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.CurrentResource != null ? src.CurrentResource.FileName : ""))

            // Size
            //.ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.CurrentResource != null ? src.CurrentResource.Size : 0))

            // SubjectName
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : "Môn chung"));

            CreateMap<FileResource, FileRequestDto>().ReverseMap();
            CreateMap<FileRequest, FileRequestDto>()
                .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.NewResource.Admin.FullName))
                .ForMember(dest => dest.NewFileName, opt => opt.MapFrom(src => src.NewResource.FileName))
                .ForMember(dest => dest.NewFileSize, opt => opt.MapFrom(src => src.NewResource.Size))
                .ForMember(dest => dest.OldFileName, opt => opt.MapFrom(src => src.OldResource != null ? src.OldResource.FileName : null))
                .ForMember(dest => dest.OldFileSize, opt => opt.MapFrom(src => src.OldResource != null ? src.OldResource.Size : (long?)null));
            CreateMap<Admin, AdminResponse>();

            CreateMap<Lecturer, LecturerResponse>()
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src =>
                    src.LecturerSubjects.Select(ls => new SubjectBriefDto
                    {
                        SubjectCode = ls.SubjectCode,
                        SubjectName = ls.Subject != null ? ls.Subject.SubjectName : ""
                    }).ToList()));
            //CreateMap<News, NewsDto>().ReverseMap();
            //CreateMap<Lecture, LectureDto>().ReverseMap();
            //CreateMap<Comment, CommentDto>().ReverseMap();
        }
    }
}