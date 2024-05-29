using System.ComponentModel.DataAnnotations;

namespace FMInatorul.Models
{
    
    public class Variante
    {
        [Key]
        public int Id { get; set; }

        public string Choice {  get; set; }

        public int IntrebariRaspId { get; set; }

        public int VariantaCorecta { get; set; } = 0; //can get only 0 or 1

        public virtual IntrebariRasp IntrebariRasp {  get; set; }



    }
}
