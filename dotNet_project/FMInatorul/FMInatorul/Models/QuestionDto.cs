using FMInatorul.Models;

namespace FMInatorul.Models
{
    public class QuestionDto
    {
        public string Answer { get; set; }
        public Dictionary <string,string> Choices { get; set; }
        public string Question { get; set; }
    }
    public class QuestionDtoList
    {

        public List<QuestionDto> Questions { get; set; }
    }
}

