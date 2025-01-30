namespace FMInatorul.Models
{
    public class GameState
    {
        public List<IntrebariRasp> Questions { get; set; } = new();
        public int CurrentQuestionIndex { get; set; }

        public HashSet<string> UsersAnswered { get; set; } = new();

        public bool IsGameActive { get; set; } = false;
    }
}
