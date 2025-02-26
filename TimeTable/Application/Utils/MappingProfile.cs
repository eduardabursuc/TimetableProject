﻿using Application.DTOs;
using Domain.Entities;
using AutoMapper;
using Application.UseCases.Commands.ConstraintCommands;
using Application.UseCases.Commands.CourseCommands;
using Application.UseCases.Commands.GroupCommands;
using Application.UseCases.Commands.ProfessorCommands;
using Application.UseCases.Commands.RoomCommands;
using Application.UseCases.Commands.TimetableCommands;

namespace Application.Utils;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapping for Constraint
        CreateMap<Constraint, ConstraintDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ProfessorId, opt => opt.MapFrom(src => src.ProfessorId))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomId))
            .ForMember(dest => dest.WantedRoomName, opt => opt.MapFrom(src => src.WantedRoomId))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.Day))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.WantedDay, opt => opt.MapFrom(src => src.WantedDay))
            .ForMember(dest => dest.WantedTime, opt => opt.MapFrom(src => src.WantedTime))
            .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event));
        
        CreateMap<DeleteConstraintCommand, Constraint>().ReverseMap();
 
        CreateMap<Professor, ProfessorDto>().ReverseMap();
        CreateMap<CreateProfessorCommand, Professor>().ReverseMap();
        CreateMap<UpdateProfessorCommand, Professor>().ReverseMap();
        CreateMap<DeleteProfessorCommand, Professor>().ReverseMap();
        CreateMap<Course, CourseDto>().ReverseMap();
        CreateMap<CreateCourseCommand, Course>().ReverseMap();
        CreateMap<UpdateCourseCommand, Course>().ReverseMap();
        CreateMap<Timetable, TimetableDto>().ReverseMap();
        CreateMap<CreateTimetableCommand, Timetable>().ReverseMap();
        CreateMap<UpdateTimetableCommand, Timetable>().ReverseMap();
        CreateMap<DeleteCourseCommand, Course>();
        CreateMap<Room, RoomDto>().ReverseMap();
        CreateMap<Group, GroupDto>().ReverseMap();
        CreateMap<CreateGroupCommand, Group>().ReverseMap();
        CreateMap<UpdateGroupCommand, Group>().ReverseMap();
        CreateMap<DeleteGroupCommand, Group>().ReverseMap();
        CreateMap<CreateRoomCommand, Room>().ReverseMap();
        CreateMap<UpdateRoomCommand, Room>().ReverseMap();
        CreateMap<DeleteRoomCommand, Room>().ReverseMap();
        
        CreateMap<User, UserDto>().ReverseMap();
        
    }
}