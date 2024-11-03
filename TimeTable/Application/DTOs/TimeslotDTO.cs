namespace Application.DTOs
{
    public class TimeslotDTO
    {
        public Guid Id { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }

        public List<ConstraintDTO> Constraints { get; set; } = new List<ConstraintDTO>();
    }
}