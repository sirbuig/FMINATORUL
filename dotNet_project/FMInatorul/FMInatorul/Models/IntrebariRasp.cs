namespace FMInatorul.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
public class IntrebariRasp
{
    [Key]
    public int Id { get; set; }
    public string intrebare { get; set; }
    public string raspunsCorect {  get; set; }
    public int validareProfesor { get; set; }

    public int MaterieId { get; set; }

    public virtual Materie Materie { get; set; }
    
    public virtual ICollection<Variante>? Variante { get; set; }
}