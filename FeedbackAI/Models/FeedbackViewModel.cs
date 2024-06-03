using static FeedbackAI.Models.Feedback;

namespace FeedbackAI.Models
{
    public class FeedbackViewModel
    {
        public List<Feedback> Feedbacks { get; set; }
        public string SearchString { get; set; }
        public EmotionType SearchEmotionType { get; set; }
    }
}
