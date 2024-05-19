using System.ComponentModel.DataAnnotations;

namespace FMInatorul.Models
{
    public class Profesor
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public bool CompletedProfile { get; set; }

    }
}
