namespace FMInatorul.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string SelectedAnswer { get; set; } // Noul câmp pentru a reține răspunsul selectat
        public List<string> Choices { get; set; } = new List<string>(); // Optional list of choices
    }

    public class QuizModel
    {
        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
    }
}
