namespace FMInatorul.Models;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }

<<<<<<< HEAD
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

    }

    public class Student : ApplicationUser
    {
        //some details for student
    }

    public class Profesor : ApplicationUser
    {
        //some details for profesor
    }

}
=======
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }
}
>>>>>>> 4c8354be914c97f93e5d434cf420e89861115484
