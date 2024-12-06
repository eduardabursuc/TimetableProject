using Domain.Entities;
using Domain.Repositories;

namespace Application.Validators
{
    public class ProfessorsValidator(IProfessorRepository repository)
    {
        public Tuple<bool, string> Validate(Professor professor)
        {
            if (string.IsNullOrEmpty(professor.Name))
            {
                return Tuple.Create(false, "Professor name is required.");
            }
            return new Tuple<bool, string>(true, "Professor is valid.");
        }
    }
}