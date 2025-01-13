using Domain.Entities;
namespace Application.Services
{
    public class BacktrackResult
    {
        public bool isFeasibleSolutionFound { get; set; }
        public List<(Event, Room, Timeslot)> bestSolution { get; set; }
        public double bestScore { get; set; }

        public BacktrackResult()
        {
            bestSolution = new List<(Event, Room, Timeslot)>();
            isFeasibleSolutionFound = false;
            bestScore = 0;
        }
    }

}
