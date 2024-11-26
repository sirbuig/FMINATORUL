﻿using Microsoft.AspNetCore.Mvc.Rendering;
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

        public int FacultateID { get; set; }

        public virtual Facultate Facultate { get; set; }
        public virtual ICollection<Materie>? MateriiPredate { get; set; }

        [NotMapped] 
        public List<int> SelectedMateriiIds { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Materii { get; set; }

    }
}
