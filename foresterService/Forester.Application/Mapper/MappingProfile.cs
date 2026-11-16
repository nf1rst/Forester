using System;
using AutoMapper;
using Forester.Application.DTO.Logs;
using Forester.Domain.Entities;

namespace Forester.Application.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Log, LogDTO>();
    }
}
