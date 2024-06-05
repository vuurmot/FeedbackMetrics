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
        public struct TopIssuesData
        {
            public string IssueName;
            public int IssueCount;
        }
        public struct GeneralData
        {
            public List<TopIssuesData> TopIssues;
            public int AngryEmotionCount;
            public int HappyEmotionCount;
            public int NeutralEmotionCount;
        }

        public SentimentOverTimeData SentimentOverTime { get; set; }
        public GeneralData AllTimeData { get; set; }
        public GeneralData Past30Data { get; set; }
    }
}