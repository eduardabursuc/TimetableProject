using System.Text.Json;
using Application.Services;
using Application.UseCases.Commands.ConstraintCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;
using Domain.Entities;

namespace Application.UseCases.CommandHandlers.ConstraintCommandHandlers
{
    public class CreateConstraintCommandHandler : IRequestHandler<CreateConstraintCommand, Result<Guid>>
    {
        private readonly IConstraintRepository _repository;
        private readonly ITimetableRepository _timetableRepository;
        private readonly IProfessorRepository _professorRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ICourseRepository _courseRepository;

        public CreateConstraintCommandHandler(
            IConstraintRepository repository,
            ITimetableRepository timetableRepository,
            IProfessorRepository professorRepository,
            IRoomRepository roomRepository,
            IGroupRepository groupRepository,
            ICourseRepository courseRepository)
        {
            _repository = repository;
            _timetableRepository = timetableRepository;
            _professorRepository = professorRepository;
            _roomRepository = roomRepository;
            _groupRepository = groupRepository;
            _courseRepository = courseRepository;
        }

        public async Task<Result<Guid>> Handle(CreateConstraintCommand request, CancellationToken cancellationToken)
        {
            // Get the timetable by ID
            var timetableResult = await _timetableRepository.GetByIdAsync(request.TimetableId);
            if (!timetableResult.IsSuccess)
            {
                return Result<Guid>.Failure(timetableResult.ErrorMessage);
            }

            var timetable = timetableResult.Data;

            // Dictionaries for IDs to Names
            var roomDictionary = new Dictionary<Guid, string>();
            var groupDictionary = new Dictionary<Guid, string>();
            var courseDictionary = new Dictionary<Guid, string>();

            // Extract and populate dictionaries
            foreach (var room in _roomRepository.GetAllAsync(timetable.UserEmail).Result.Data)
            {
                roomDictionary[room.Id] = room.Name;
            }
            
            foreach (var group in _groupRepository.GetAllAsync(timetable.UserEmail).Result.Data)
            {
                groupDictionary[group.Id] = group.Name;
            }
            
            foreach (var course in _courseRepository.GetAllAsync(timetable.UserEmail).Result.Data)
            {
                courseDictionary[course.Id] = course.CourseName;
            }

            // Send only the names to ChatGPT API
            var roomNames = roomDictionary.Values.ToList();
            var groupNames = groupDictionary.Values.ToList();
            var courseNames = courseDictionary.Values.ToList();

            // Call ChatGPT API
            var openAiService = new OpenAiService();
            var chatGptResponse = await openAiService.ProcessConstraintAsync(
                request.Input,
                roomNames,
                groupNames,
                courseNames
            );

            Console.WriteLine(chatGptResponse);

            // Deserialize the response
            var chatGptResult = ExtractConstraintResponse(chatGptResponse);
            if (chatGptResult.ValidationErrors.Count != 0)
            {
                return Result<Guid>.Failure(string.Join(", ", chatGptResult.ValidationErrors));
            }
            
            var professorIds = timetable.Events.Select(e => e.ProfessorId).ToList();
            var professorId = professorIds.FirstOrDefault(p =>
                _professorRepository.GetByIdAsync(p).Result.Data.Email == request.ProfessorEmail);
            
            var constraint = new Constraint
            {
                TimetableId = timetable.Id,
                Type = chatGptResult.Type,
                ProfessorId = professorId,
                CourseId = chatGptResult.CourseName != null ? courseDictionary.FirstOrDefault(c => c.Value == chatGptResult.CourseName).Key : null,
                RoomId = chatGptResult.RoomName != null ? roomDictionary.FirstOrDefault(r => r.Value == chatGptResult.RoomName).Key : null,
                GroupId = chatGptResult.GroupName != null ? groupDictionary.FirstOrDefault(g => g.Value == chatGptResult.GroupName).Key : null,
                Event = chatGptResult.Event,
                Day = chatGptResult.Day,
                Time = chatGptResult.Time
            };
            
            // Save the constraint to the repository
            var result = await _repository.AddAsync(constraint);

            return result.IsSuccess
                ? Result<Guid>.Success(result.Data)
                : Result<Guid>.Failure(result.ErrorMessage);
        }

        private ChatGptConstraintResponse ExtractConstraintResponse(string jsonString)
        {
            // Parse the JSON string into a JsonDocument
            using var doc = JsonDocument.Parse(jsonString);
            // Navigate to the "choices" array
            var choices = doc.RootElement.GetProperty("choices");

            // Get the first choice object
            var firstChoice = choices[0];

            // Get the message object
            var message = firstChoice.GetProperty("message");

            // Get the content string
            var content = message.GetProperty("content").GetString();

            // Parse the content string into another JsonDocument
            using var contentDoc = JsonDocument.Parse(content);
            // Extract the required properties
            var jsonResponse = contentDoc.RootElement;

            return new ChatGptConstraintResponse
            {
                Type = Enum.TryParse<ConstraintType>(jsonResponse.GetProperty("Type").GetString(), out var type) 
                    ? type 
                    : ConstraintType.HARD_NO_OVERLAP,
                Day = jsonResponse.TryGetProperty("Day", out var day) ? day.GetString() : null,
                Time = jsonResponse.TryGetProperty("Time", out var time) ? time.GetString() : null,
                Event = jsonResponse.TryGetProperty("Event", out var evnt) ? evnt.GetString() : null,
                RoomName = jsonResponse.TryGetProperty("RoomName", out var roomName) ? roomName.GetString() : null,
                GroupName = jsonResponse.TryGetProperty("GroupName", out var groupName) ? groupName.GetString() : null,
                CourseName = jsonResponse.TryGetProperty("CourseName", out var courseName) ? courseName.GetString() : null,
                ValidationErrors = jsonResponse.GetProperty("validationErrors").EnumerateArray()
                    .Select(e => e.GetString()).ToList()
            };
        }
    }
    
}

public class ChatGptConstraintResponse
{
    public ConstraintType Type { get; set; }
    public string? RoomName { get; set; }
    public string? GroupName { get; set; }
    public string? CourseName { get; set; }
    public string? Day { get; set; }
    public string? Time { get; set; }
    public string? Event { get; set; }
    public List<string?> ValidationErrors { get; set; } = new();
}
