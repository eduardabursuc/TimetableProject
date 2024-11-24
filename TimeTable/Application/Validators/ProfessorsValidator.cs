using Domain.Entities;

namespace Application.Validators
{
    public class ProfessorsValidator(Instance instance)
    {
        public Tuple<bool, string> Validate(Professor professor)
        {
            if (string.IsNullOrEmpty(professor.Name))
            {
                return Tuple.Create(false, "Professor name is required.");
            }

            return instance.Professors.Exists(p => p.Name == professor.Name && p.Id != professor.Id) ? Tuple.Create(false, "A professor with the same name already exists.") : Tuple.Create(true, "Professor is valid.");
        }
    }
}