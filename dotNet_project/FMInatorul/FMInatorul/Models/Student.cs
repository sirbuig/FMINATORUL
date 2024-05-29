using System.ComponentModel.DataAnnotations;

namespace FMInatorul.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public bool CompletedProfile { get; set; }

        [Range(1, 3, ErrorMessage = "Year must be between 1 and 3.")]
        public int Year { get; set; }

        [Range(1, 3, ErrorMessage = "Semester must be between 1 and 2.")]
        public int Semester { get; set; }
    }
}
