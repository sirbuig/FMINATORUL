using System.ComponentModel.DataAnnotations;

namespace FMInatorul.Models
{
    public class Profesor
    {
        [Key]
        public int Id { get; set; }
        public string IdApplicationUser { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
