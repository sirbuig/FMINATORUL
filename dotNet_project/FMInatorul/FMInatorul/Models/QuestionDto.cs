namespace FMInatorul.Models
{
    public class QuestionDto
    {
        public string question { get; set; }
        public string answer { get; set; }
        public Dictionary <string,string> choices { get; set; }
    }
}
