namespace FMInatorul.Models;
using System.ComponentModel.DataAnnotations;
public class Materie
{
    [Key]
    public int Id { get; set; }

    public string nume { get; set; }

    public int anStudiu { get; set; }

    public int semestru { get; set; }

    public string descriere { get; set; }

    public int FacultateID { get; set; }

    public virtual Facultate Facultate { get; set; }

    public virtual ICollection<IntrebariRasp>? IntrebariRasp { get; set; }

}