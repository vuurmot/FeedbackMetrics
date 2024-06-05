using static FeedbackAI.Models.Feedback;

namespace FeedbackAI.Models
{
    public class ChartsViewModel
    {
        public struct SentimentOverTimeData
        {
            public string[] Dates;
            public int[] Emotions;
        }
        public struct GeneralData
        {
            public List<string> TopIssues;
            public int AngryEmotionCount;
            public int HappyEmotionCount;
            public int NeutralEmotionCount;
        }

        public SentimentOverTimeData SentimentOverTime { get; set; }
        public GeneralData General { get; set; }
    }
}