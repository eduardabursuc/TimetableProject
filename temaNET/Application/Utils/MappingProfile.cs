using Application.DTOs;
using Application.UseCases.Commands;
using AutoMapper;
using Domain.Entities;

namespace Application.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDoItem, ToDoItemDTO>().ReverseMap();
            CreateMap<CreateToDoItemCommand, ToDoItem>().ReverseMap();
            CreateMap<UpdateToDoItemCommand, ToDoItem>().ReverseMap();
        }
    }
}