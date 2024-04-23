namespace FMInatorul.Models;
using System.ComponentModel.DataAnnotations;
public class Room
{
    [Key]
    public int id { get; set; }
    public string codRoom { get; set; }
    public string numeRoom { get; set; }
    public string idProf { get; set; }
    public string idMaterie { get; set; }

 

}