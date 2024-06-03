using System.ComponentModel.DataAnnotations;

namespace FeedbackAI.Models
{
    public class Feedback
    {
        [Key]
        public int Index { get; set; }
        public enum EmotionType { Empty,Angry, Happy, Neutral, }
        public EmotionType Emotion { get; set; }
        public string Description { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
