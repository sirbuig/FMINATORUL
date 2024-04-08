namespace FMInatorul.Models;
using System.ComponentModel.DataAnnotations;
public class Room
{
    [Key]
    public int id { get; set; }
    public string codRoom { get; set; }
}