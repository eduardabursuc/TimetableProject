using Domain.Entities;

namespace Application.Validators
{
    public class ProfessorsValidator
    {
        private readonly Instance instance;

        public ProfessorsValidator(Instance instance)
        {
            this.instance = instance;
        }

        public Tuple<bool, string> Validate(Professor professor)
        {
            if (string.IsNullOrEmpty(professor.Name))
            {
                return Tuple.Create(false, "Professor name is required.");
            }

            if (instance.Professors.Exists(p => p.Name == professor.Name && p.Id != professor.Id))
            {
                return Tuple.Create(false, "A professor with the same name already exists.");
            }

            return Tuple.Create(true, "Professor is valid.");
        }
    }
}