﻿using Newtonsoft.Json;

namespace FMInatorul.Models
{
    public class QuestionModel
    {
        public string Answer { get; set; } // This holds the correct answer key.
        public Dictionary<string, string> Choices { get; set; }
        public string Question { get; set; }
        public string SelectedAnswer { get; set; } // This will store the user's selected answer key.
    }
    
    public class QuizModel
    {
        public List<QuestionModel> Questions { get; set; }
    }
}