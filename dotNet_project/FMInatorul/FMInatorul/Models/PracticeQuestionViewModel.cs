namespace FMInatorul.Models
{
    public class PracticeQuestionViewModel
    {
        public int Id { get; set; }
        public string Intrebare { get; set; }
        public List<PracticeVariantViewModel> Variante { get; set; }
    }
}
