namespace FMInatorul.Models
{
    public class StudentMistake
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int IntrebareId { get; set; }
        public IntrebariRasp Intrebare { get; set; }
    }
}
