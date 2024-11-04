using Application.DTOs;
using Domain.Entities;
using AutoMapper;
using Application.UseCases.Commands;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapping for CreateConstraintCommand to Constraint entity
        CreateMap<CreateConstraintCommand, Constraint>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ProfessorId, opt => opt.MapFrom(src => src.ProfessorId))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseName))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName))
            .ForMember(dest => dest.WantedRoomName, opt => opt.MapFrom(src => src.WantedRoomName))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName))
            .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.Day))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.WantedDay, opt => opt.MapFrom(src => src.WantedDay))
            .ForMember(dest => dest.WantedTime, opt => opt.MapFrom(src => src.WantedTime))
            .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event));
        
        CreateMap<UpdateConstraintCommand, Constraint>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ProfessorId, opt => opt.MapFrom(src => src.ProfessorId))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseName))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName))
            .ForMember(dest => dest.WantedRoomName, opt => opt.MapFrom(src => src.WantedRoomName))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName))
            .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.Day))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.WantedDay, opt => opt.MapFrom(src => src.WantedDay))
            .ForMember(dest => dest.WantedTime, opt => opt.MapFrom(src => src.WantedTime))
            .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event));

        CreateMap<Constraint, ConstraintDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ProfessorId, opt => opt.MapFrom(src => src.ProfessorId))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseName))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName))
            .ForMember(dest => dest.WantedRoomName, opt => opt.MapFrom(src => src.WantedRoomName))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName))
            .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.Day))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.WantedDay, opt => opt.MapFrom(src => src.WantedDay))
            .ForMember(dest => dest.WantedTime, opt => opt.MapFrom(src => src.WantedTime))
            .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event));

        
    }
}
