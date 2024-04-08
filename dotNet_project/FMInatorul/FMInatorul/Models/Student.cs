﻿using System.ComponentModel.DataAnnotations;

namespace FMInatorul.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string IdApplicationUser { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
