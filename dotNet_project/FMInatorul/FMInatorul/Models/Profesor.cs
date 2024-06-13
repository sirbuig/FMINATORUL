using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMInatorul.Models
{
    public class Profesor
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public bool CompletedProfile { get; set; }

        public int? MaterieId { get; set; }
        public virtual Materie Materie { get; set; }

        [NotMapped]
        public IEnumerable<Materie> Materii { get; set; }

    }
}
