using static FeedbackAI.Models.Feedback;

namespace FeedbackAI.Models
{
    public class FeedbackViewModel
    {
        public List<Feedback> Feedbacks { get; set; }
        public string SearchString { get; set; }
        public EmotionType SearchEmotionType { get; set; }
        public string SearchCategory { get; set; }
        public enum SearchDates { Empty, Past1Year, Past1Month, Past1Week, Past1Day}
        public SearchDates SearchDate { get; set; }
    }
}
