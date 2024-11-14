namespace FMInatorul.Models;
using System.ComponentModel.DataAnnotations;


public class Facultate
{

    [Key]
    public int Id { get; set; }
    public string nume { get; set; }
    public virtual ICollection<Materie>? Materies{ get; set; }
}

