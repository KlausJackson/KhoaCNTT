using AutoMapper;
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Application.DTOs.Admin;

public class AdminMappingProfile : Profile
{
    public AdminMappingProfile()
    {
        CreateMap<AdminUser, AdminResponse>();
    }
}
