using Domain.Entities;
using System.Globalization;

namespace Application.Validators
{
    public class SoftConstraintsValidator
    {
        private static readonly Dictionary<string, int> DaysOfWeek = new Dictionary<string, int>
            {
                { "Monday", 1 },
                { "Tuesday", 2 },
                { "Wednesday", 3 },
                { "Thursday", 4 },
                { "Friday", 5 },
                { "Saturday", 6 },
                { "Sunday", 7 }
            };
        private readonly Dictionary<int, double> softConstraintScoreCache = new();

        private readonly List<(Event, Room, Timeslot)> emptySolution = new();

        public double GetCachedScore(Event ev, Room room, Timeslot timeslot, List<Constraint> softConstraints)
        {
            int key = GenerateHashKey(ev, room, timeslot);

            if (softConstraintScoreCache.TryGetValue(key, out double cachedScore))
            {
                return cachedScore;
            }

            double score = CalculateScore(ev, room, timeslot, emptySolution, softConstraints);
            softConstraintScoreCache[key] = score;
            return score;
        }


        private int GenerateHashKey(Event ev, Room room, Timeslot timeslot)
        {
            return HashCode.Combine(ev.Id, room.Id, timeslot.Day, timeslot.Time);
        }


        public double CalculateScore(Event ev, Room room, Timeslot timeslot, List<(Event, Room, Timeslot)> currentSolution, List<Constraint> softConstraints)
        {
            double score = 0;

            foreach (var constraint in softConstraints)
            {
                switch (constraint.Type)
                {
                    case ConstraintType.SOFT_ROOM_CHANGE:
                        if (constraint.ProfessorId == ev.ProfessorId &&
                            constraint.CourseId == ev.CourseId &&
                            constraint.GroupId == ev.GroupId &&
                            constraint.WantedRoomId == room.Id &&
                            (string.IsNullOrEmpty(constraint.Day) || constraint.Day == timeslot.Day) &&
                            (string.IsNullOrEmpty(constraint.Time) || constraint.Time == timeslot.Time))
                        {
                            score += 50;
                        }
                        break;

                    case ConstraintType.SOFT_ROOM_PREFERENCE:
                        if (constraint.ProfessorId == ev.ProfessorId && constraint.WantedRoomId == room.Id || constraint.CourseId == ev.CourseId && constraint.WantedRoomId == room.Id)
                        {
                            score += 50;
                        }
                        break;

                    case ConstraintType.SOFT_TIME_CHANGE:
                        if (constraint.ProfessorId == ev.ProfessorId &&
                            constraint.CourseId == ev.CourseId &&
                            constraint.GroupId == ev.GroupId &&
                            constraint.WantedTime == timeslot.Time)
                        {
                            score += 50;
                        }
                        break;

                    case ConstraintType.SOFT_DAY_CHANGE:
                        if (constraint.ProfessorId == ev.ProfessorId &&
                            constraint.CourseId == ev.CourseId &&
                            constraint.GroupId == ev.GroupId &&
                            constraint.WantedDay == timeslot.Day &&
                            constraint.WantedTime == timeslot.Time)
                        {
                            score += 50;
                        }
                        break;

                    case ConstraintType.SOFT_ADD_WINDOW:
                        if (constraint.ProfessorId == ev.ProfessorId)
                        {
                            if (!string.IsNullOrEmpty(constraint.Day))
                            {
                                var professorEvents = currentSolution
                                    .Where(e => e.Item1.ProfessorId == ev.ProfessorId && e.Item3.Day == constraint.Day)
                                    .OrderBy(e => DateTime.ParseExact(e.Item3.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture))
                                    .ToList();

                                bool hasBreak = false;

                                for (int i = 0; i < professorEvents.Count - 1; i++)
                                {
                                    var firstTimeslot = new Timeslot
                                    {
                                        Day = professorEvents[i].Item3.Day,
                                        Time = professorEvents[i].Item3.Time
                                    };

                                    var secondTimeslot = new Timeslot
                                    {
                                        Day = professorEvents[i + 1].Item3.Day,
                                        Time = professorEvents[i + 1].Item3.Time
                                    };

                                    // Dacă există o pauză între două evenimente consecutive
                                    if (!firstTimeslot.IsConsecutive(secondTimeslot))
                                    {
                                        hasBreak = true;
                                        break;
                                    }
                                }

                                if (hasBreak)
                                {
                                    score += 50;

                                }
                            }
                            else if (!string.IsNullOrEmpty(constraint.Time))
                            {
                                var constraintTimeslot = new Timeslot
                                {
                                    Day = string.Empty, // Ziua nu contează
                                    Time = constraint.Time
                                };


                                bool isFreeEveryDay = true;

                                foreach (var day in DaysOfWeek.Keys)
                                {
                                    var professorEvents = currentSolution
                                        .Where(e => e.Item1.ProfessorId == ev.ProfessorId && e.Item3.Day == day)
                                        .OrderBy(e => DateTime.ParseExact(e.Item3.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture))
                                        .ToList();

                                    bool hasBreak = false;

                                    foreach (var professorEvent in professorEvents)
                                    {
                                        var eventTimeslot = new Timeslot
                                        {
                                            Day = professorEvent.Item3.Day,
                                            Time = professorEvent.Item3.Time
                                        };

                                        // Dacă evenimentul are o pauză care se încadrează în intervalul specificat
                                        if (eventTimeslot.inInterval(constraintTimeslot))
                                        {
                                            hasBreak = true;
                                            break;
                                        }
                                    }

                                    if (!hasBreak)
                                    {
                                        isFreeEveryDay = false;
                                        break;
                                    }
                                }

                                if (isFreeEveryDay)
                                {
                                    score += 50;
                                }
                            }
                        }
                        break;

                    case ConstraintType.SOFT_REMOVE_WINDOW:
                        if (constraint.ProfessorId == ev.ProfessorId)
                        {
                            if (!string.IsNullOrEmpty(constraint.Day))
                            {
                                var professorEvents = currentSolution
                                    .Where(e => e.Item1.ProfessorId == ev.ProfessorId && e.Item3.Day == constraint.Day)
                                    .OrderBy(e => DateTime.ParseExact(e.Item3.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture))
                                    .ToList();

                                bool hasNoBreak = true;

                                for (int i = 0; i < professorEvents.Count - 1; i++)
                                {
                                    var firstTimeslot = new Timeslot
                                    {
                                        Day = professorEvents[i].Item3.Day,
                                        Time = professorEvents[i].Item3.Time
                                    };

                                    var secondTimeslot = new Timeslot
                                    {
                                        Day = professorEvents[i + 1].Item3.Day,
                                        Time = professorEvents[i + 1].Item3.Time
                                    };

                                    // Dacă nu există pauză între două evenimente consecutive
                                    if (firstTimeslot.IsConsecutive(secondTimeslot))
                                    {
                                        hasNoBreak = false;
                                        break;
                                    }
                                }

                                if (hasNoBreak)
                                {
                                    score += 50;
                                }
                            }

                            else if (!string.IsNullOrEmpty(constraint.Time))
                            {
                                var constraintTimeslot = new Timeslot
                                {
                                    Day = string.Empty, // Ziua nu contează
                                    Time = constraint.Time
                                };

                                var professorEvents = currentSolution
                                    .Where(e => e.Item1.ProfessorId == ev.ProfessorId)
                                    .OrderBy(e => DateTime.ParseExact(e.Item3.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture))
                                    .ToList();

                                bool hasOverlap = false;

                                foreach (var professorEvent in professorEvents)
                                {
                                    var eventTimeslot = new Timeslot
                                    {
                                        Day = professorEvent.Item3.Day,
                                        Time = professorEvent.Item3.Time
                                    };

                                    // Dacă evenimentul se suprapune cu intervalul specificat
                                    if (eventTimeslot.overlap(constraintTimeslot))
                                    {
                                        hasOverlap = true;
                                        break;
                                    }
                                }

                                if (hasOverlap)
                                {
                                    score += 50;
                                }
                            }
                        }
                        break;

                    case ConstraintType.SOFT_DAY_OFF:
                        if (constraint.ProfessorId == ev.ProfessorId &&
                            constraint.Day != timeslot.Day)
                        {
                            score += 50;
                        }
                        break;

                    case ConstraintType.SOFT_WEEK_EVENNESS:
                        if (constraint.CourseId == ev.CourseId &&
                            constraint.ProfessorId == ev.ProfessorId &&
                            constraint.GroupId == ev.GroupId)
                        {
                            ev.IsEven = true;
                            score += 50;
                        }
                        break;

                    case ConstraintType.SOFT_CONSECUTIVE_HOURS:
                        if (constraint.ProfessorId == ev.ProfessorId && constraint.Day == timeslot.Day)
                        {
                            var professorEvents = currentSolution
                                .Where(e => e.Item1.ProfessorId == ev.ProfessorId && e.Item3.Day == timeslot.Day)
                                .OrderBy(e => DateTime.ParseExact(e.Item3.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture))
                                .ToList();

                            bool areConsecutive = true;
                            for (int i = 0; i < professorEvents.Count - 1; i++)
                            {
                                var currentEnd = DateTime.ParseExact(professorEvents[i].Item3.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                                var nextStart = DateTime.ParseExact(professorEvents[i + 1].Item3.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                                if (!professorEvents[i].Item3.IsConsecutive(professorEvents[i + 1].Item3))
                                {
                                    areConsecutive = false;
                                    break;
                                }
                            }

                            if (areConsecutive)
                            {
                                score += 50;
                            }
                        }
                        break;

                    case ConstraintType.SOFT_INTERVAL_AVAILABILITY:
                        if (constraint.ProfessorId == ev.ProfessorId && constraint.Day == timeslot.Day)
                        {
                            var constraintStart = DateTime.ParseExact(constraint.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                            var constraintEnd = DateTime.ParseExact(constraint.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                            var eventStart = DateTime.ParseExact(timeslot.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                            var eventEnd = DateTime.ParseExact(timeslot.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                            if (eventStart >= constraintStart && eventEnd <= constraintEnd)
                            {
                                score += 50;
                            }
                        }
                        break;

                    case ConstraintType.SOFT_INTERVAL_UNAVAILABILITY:
                        if (constraint.ProfessorId == ev.ProfessorId && constraint.Day == timeslot.Day)
                        {
                            var constraintStart = DateTime.ParseExact(constraint.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                            var constraintEnd = DateTime.ParseExact(constraint.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                            var eventStart = DateTime.ParseExact(timeslot.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                            var eventEnd = DateTime.ParseExact(timeslot.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                            if (eventEnd <= constraintStart || eventStart >= constraintEnd) //verifica daca eventul e in afara intervalului indisponibil
                            {
                                score += 50;
                            }
                        }
                        break;

                    case ConstraintType.SOFT_LECTURE_BEFORE_LABS:
                        if (constraint.CourseId == ev.CourseId)
                        {
                            CheckLectureBeforeLab(ev, timeslot, currentSolution, ref score);
                        }
                        else if (constraint.ProfessorId == ev.ProfessorId)
                        {
                            foreach (var professorEvent in currentSolution)
                            {
                                if (professorEvent.Item1.ProfessorId == ev.ProfessorId && professorEvent.Item1.EventName.Contains("course"))
                                {
                                    if (CheckLectureBeforeLab(professorEvent.Item1, professorEvent.Item3, currentSolution, ref score))
                                        score += 50;
                                }
                            }
                        }
                        break;
                }
            }

            return score;
        }

        public double MaxRemainingScore(Dictionary<Event, List<(Room, Timeslot)>> variables, List<(Event, Room, Timeslot)> currentSolution, List<Constraint> softConstraints)
        {
            double maxScore = 0;
            foreach (var kv in variables)
            {
                if (!currentSolution.Any(cs => cs.Item1.Id == kv.Key.Id))
                {
                    double eventMaxScore = double.MinValue;
                    foreach (var assignment in kv.Value)
                    {
                        double score = CalculateScore(kv.Key, assignment.Item1, assignment.Item2, currentSolution, softConstraints);
                        if (score > eventMaxScore)
                        {
                            eventMaxScore = score;
                        }
                    }
                    maxScore += eventMaxScore;
                }
            }

            return maxScore;
        }

        public double CalculateTotalScore(List<(Event, Room, Timeslot)> solution, List<Constraint> softConstraints)
        {
            double totalScore = 0;
            foreach (var (ev, room, timeslot) in solution)
            {
                totalScore += CalculateScore(ev, room, timeslot, solution, softConstraints);
            }
            return totalScore;
        }

        private bool CheckLectureBeforeLab(Event ev, Timeslot timeslot, List<(Event, Room, Timeslot)> currentSolution, ref double score)
        {
            bool isLabOrSeminar = ev.EventName.Contains("laboratory") || ev.EventName.Contains("seminary");
            bool isLecture = ev.EventName.Contains("course");

            // cautam evenimentul corespunzator cursului
            var matchingEvent = isLabOrSeminar
                ? currentSolution.FirstOrDefault(e => e.Item1.CourseId == ev.CourseId && e.Item1.EventName.Contains("course") && e.Item1.GroupId == ev.GroupId)
                : currentSolution.FirstOrDefault(e => e.Item1.CourseId == ev.CourseId && (e.Item1.EventName.Contains("laboratory") || e.Item1.EventName.Contains("seminary")) && e.Item1.GroupId == ev.GroupId);

            if (matchingEvent.Item1 != null)
            {
                int eventDay = DaysOfWeek[timeslot.Day];
                int matchingEventDay = DaysOfWeek[matchingEvent.Item3.Day];

                DateTime eventEndTime = DateTime.ParseExact(timeslot.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                DateTime matchingEventStartTime = DateTime.ParseExact(matchingEvent.Item3.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                bool isValid = isLabOrSeminar
                    ? eventDay > matchingEventDay || eventDay == matchingEventDay && eventEndTime >= matchingEventStartTime
                    : eventDay < matchingEventDay || eventDay == matchingEventDay && eventEndTime <= matchingEventStartTime;

                return isValid;
            }

            return false;
        }


    }
}