using static FeedbackAI.Models.Feedback;

namespace FeedbackAI.Models
{
    public class ChartsViewModel
    {
        public struct DayEmotionData
        {
            public List<string> Dates;
            public List<int> AngryEmotionCount;
            public List<int> HappyEmotionCount;
            public List<int> NeutralEmotionCount;
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
        public DayEmotionData Past3MonthsEmotion { get; set; }
        public GeneralData AllTimeData { get; set; }
        public GeneralData Past30Data { get; set; }
    }
}