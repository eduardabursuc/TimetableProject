namespace Application.Services;
using Domain.Entities;
using Domain.Common;

public class TimetableGenerator
{
    private readonly Instance instance;

    public TimetableGenerator(Instance instance)
    {
        this.instance = instance;
    }

    public Result<Timetable> Generate()
    {
        foreach (var evt in instance.Events)
        {
            foreach (var constraint in instance.Constraints)
            {
                if (constraint.ProfessorId == evt.ProfessorId)
                {
                    evt.Constraints.Add(constraint);
                }
            }
        }

        List<Timeslot> timeslots = new List<Timeslot>();

        foreach (var time in instance.TimeSlots)
        {
            foreach (var room in instance.Rooms)
            {
                timeslots.Add(new Timeslot
                {
                    Day = time.Day,
                    Time = time.Time,
                    RoomName = room.Name
                });
            }
        }

        // Ideea:
        // 0. Pentru fiecare eveniment, adauga toate constrangerile care se aplica
        // 1. Cream un set de timeslots*rooms
        // 2. Alegem constrangerile hard care se aplica:
        // 2.1. Daca prioritatea dupa ani este specificata atunci se sorteaza lista de evenimente dupa an
        // 3. Parcurgem evenimentele:
        // 4. Daca evenimentul nu are constrangeri care se aplica, il adaugam in primul timeslot disponibil
        // 5. Daca evenimentul are constrangeri care se aplica, alegem un timeslot disponibil pentru care
        // satisface constrangerile
        // 6. Daca gasim un conflict ne intoarcem cu un pas si incercam sa alegem alt timeslot pentru evenimentul
        // curent din timeslot 

        return Result<Timetable>.Success(new Timetable());
    }
}