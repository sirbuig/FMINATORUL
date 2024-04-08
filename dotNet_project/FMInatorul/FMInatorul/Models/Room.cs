<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;

namespace FMInatorul.Models
{
    public class Room
    {
        [Key]
        public int id { get; set; }
        public string codRoom { get; set; }
=======
﻿namespace FMInatorul.Models;
>>>>>>> 4c8354be914c97f93e5d434cf420e89861115484

public class Room
{
    public int ID { get; set; }
    public string codRoom { get; set; }
}