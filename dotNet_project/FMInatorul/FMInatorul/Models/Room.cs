using System.ComponentModel.DataAnnotations;

namespace FMInatorul.Models
{
    public class Room
    {
        [Key]
        public int id { get; set; }
        public string codRoom { get; set; }

    }
}
