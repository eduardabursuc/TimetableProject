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
            .ForMember(dest => dest.ProfessorId, opt => opt.MapFrom(src => src.ProfessorId))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
            .ForMember(dest => dest.WantedRoomId, opt => opt.MapFrom(src => src.WantedRoomId))
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.WantedTimeslotId, opt => opt.MapFrom(src => src.WantedTimeslotId))
            .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event))
            .ForMember(dest => dest.Timeslots, opt => opt.MapFrom(src => src.Timeslots));

        CreateMap<CourseDTO, Course>()
            .ForMember(dest => dest.Professors, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<GroupDTO, Group>().ReverseMap();

        CreateMap<ProfessorDTO, Professor>()
            .ForMember(dest => dest.Courses, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<RoomDTO, Room>().ReverseMap();

        CreateMap<TimeslotDTO, Timeslot>()
            .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.Day))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.Constraints, opt => opt.MapFrom(src => src.Constraints));

        CreateMap<Constraint, ConstraintDTO>()
            .ForMember(dest => dest.Timeslots, opt => opt.MapFrom(src => src.Timeslots))
            .ReverseMap();

        CreateMap<Timeslot, TimeslotDTO>()
            .ForMember(dest => dest.Constraints, opt => opt.MapFrom(src => src.Constraints))
            .ReverseMap();
    }
}
