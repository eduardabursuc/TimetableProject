using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Application.Services;

public class OpenAiService
{
    private const string _apiKey = "secret_key";
    private const string _apiUrl = "https://api.openai.com/v1/chat/completions";

    public async Task<string> ProcessConstraintAsync(string userInput, List<string> rooms, List<string> groups, List<string> courses)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        // Build the system message dynamically with database entries
        var systemMessage = $@"
            You are an AI assistant that processes user input for creating constraints in a university timetable system.

            A constraint can be one of the following types:
            0. HARD_NO_OVERLAP: No specific fields required. Ensures that two events do not overlap.
            1. HARD_YEAR_PRIORITY: No specific fields required. Ensures priority for a specific year group.
            2. HARD_ROOM_CAPACITY: Requires:
               - Room (string)
               - Capacity (integer)
            3. SOFT_ROOM_CHANGE: Requires:
               - CourseName (string)
               - Event (lecture, laboratory, seminary)
               - GroupName (string) OR Day (string) and Time (string)
            4. SOFT_ROOM_PREFERENCE: Requires:
               - WantedRoomName (string)
               - ProfessorId (string) OR CourseName (string) and Event (lecture, laboratory, seminary)
            5. SOFT_TIME_CHANGE: Requires:
               - Day (string) and Time (string) OR CourseName (string), Event (string), and GroupName (string)
            6. SOFT_DAY_CHANGE: Requires:
               - Day (string) and Time (string) OR CourseName (string), Event (string), and GroupName (string)
            7. SOFT_ADD_WINDOW: Requires:
                - Day (string) OR Time (string)
            8. SOFT_REMOVE_WINDOW: Requires:
                - Day (string) OR Time (string)
            9. SOFT_DAY_OFF: Requires:
                - Day (string)
            10. SOFT_WEEK_EVENNESS: Requires:
                - CourseName (string)
            11. SOFT_CONSECUTIVE_HOURS: Requires:
                - Day (string)
            12. SOFT_INTERVAL_AVAILABILITY: Requires:
               - Day (string)
               - Time (string)
            13. SOFT_INTERVAL_UNAVAILABILITY: Requires:
               - Day (string)
               - Time (string)
            14. SOFT_LECTURE_BEFORE_LABS: Requires:
                - CourseName (string) OR ProfessorId (string)

            Instructions:
            1. Classify the user input into one of the constraint types.
            2. Extract the required fields for that constraint type.
            3. Validate the extracted data against the provided database entries.

            Example Database Entries:
            Rooms: [ ""Room 101"", ""Room 102"" ]
            Groups: [ ""Group A"", ""Group B"" ]
            Courses: [ ""Math 101"", ""Physics 102"" ]
            Timeslots: [ ""Monday 08:00 - 10:00"", ""Wednesday 14:00 - 16:00"" ]

            **Example User Input**: ""I am unavailable on Monday 08:00 - 10:00.""
            **Example Response**:
            {{
              ""Type"": ""13"",
              ""Day"": ""Monday"",
              ""Time"": ""08:00 - 10:00"",
              ""validationErrors"": []
            }}

            **Example User Input**: ""Schedule Math 101 in Room 101 for Group A.""
            **Example Response**:
            {{
              ""Type"": ""4"",
              ""CourseName"": ""Math 101"",
              ""RoomName"": ""Room 101"",
              ""GroupName"": ""Group A"",
              ""validationErrors"": []
            }}

            **Example User Input**: ""Change time for Math 101 lecture for Group A at 8-10 on Monday""
            **Example Response**:
            {{
              ""Type"": ""5"",
              ""CourseName"": ""Math 101"",
              ""GroupName"": ""Group A"",
              ""Event"" : ""lecture"",
              ""Day"" : ""Monday"",
              ""Time"" : ""08:00 - 10:00"",
              ""validationErrors"": []
            }}


            If the data does not exist in the database entries, include it in a field `validationErrors` in the response. Respond only in JSON format. 
                    

        These are the Database Entries you will compare against:
        Rooms: [ {string.Join(", ", rooms.Select(r => $"\"{r}\""))} ]
        Groups: [ {string.Join(", ", groups.Select(g => $"\"{g}\""))} ]
        Courses: [ {string.Join(", ", courses.Select(c => $"\"{c}\""))} ]
        ";
        
        
        // Build the request payload
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userInput }
            },
            temperature = 0.0
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(_apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            return responseJson; // JSON with constraintType, fields, validationErrors
        }
        else
        {
            throw new Exception($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
    }
}