using FeedbackAI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using static FeedbackAI.Models.ChartsViewModel;
using static FeedbackAI.Models.Feedback;
using static FeedbackAI.Models.FeedbackViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FeedbackAI.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDBContext applicationDBContext;

        public FeedbackController(ApplicationDBContext applicationDBContext)
        {
            this.applicationDBContext = applicationDBContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(EmotionType searchEmotionType, string searchCategory, string searchString, SearchDates searchDate)
        {
            FeedbackViewModel model = new FeedbackViewModel();
            var feedbacks = applicationDBContext.Feedbacks.ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                feedbacks = feedbacks.FindAll(item => item.Description.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));
            }
            if (searchEmotionType != EmotionType.Empty)
            {
                feedbacks = feedbacks.FindAll(item => item.Emotion == searchEmotionType);
            }
            if (!string.IsNullOrEmpty(searchCategory) && searchCategory != "All")
            {
                feedbacks = feedbacks.FindAll(item => item.Category == searchCategory);
            }
            if (searchDate != SearchDates.Empty)
            {
                DateTime evaluateDate = DateTime.Now;
                switch (searchDate)
                {
                    case SearchDates.Past1Year:
                        evaluateDate = evaluateDate.AddYears(-1);
                        break;
                    case SearchDates.Past1Month:
                        evaluateDate = evaluateDate.AddMonths(-1);
                        break;
                    case SearchDates.Past1Week:
                        evaluateDate = evaluateDate.AddDays(-7);
                        break;
                    case SearchDates.Past1Day:
                        evaluateDate = evaluateDate.AddDays(-1);
                        break;
                }
                feedbacks = feedbacks.FindAll(item => item.CreatedTime >= evaluateDate);
            }
            model.Feedbacks = feedbacks;
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return View(feedback);
            }
            feedback.CreatedTime = DateTime.UtcNow;
            applicationDBContext.Add(feedback);
            applicationDBContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Detail(int id)
        {
            Feedback feedback = applicationDBContext.Feedbacks.FirstOrDefault(item => item.Index == id);
            return View(feedback);
        }
        public IActionResult Charts()
        {
            ChartsViewModel model = new ChartsViewModel();

            var feedbacks = applicationDBContext.Feedbacks.ToList();
            feedbacks = feedbacks.FindAll(item => item.CreatedTime >= DateTime.Now.AddMonths(-3));
            DayEmotionData dayEmotionData = new DayEmotionData();
            List<string> dates = new List<string>();
            List<int> happyCount = new List<int>();
            List<int> angryCount = new List<int>();
            List<int> neutralCount = new List<int>();
            
            foreach (var feedback in feedbacks)
            {
                dates.Add(feedback.CreatedTime.ToString("dd/MM/yyyy"));
                happyCount.Add(feedbacks.FindAll(item => item.CreatedTime.Date == feedback.CreatedTime.Date && item.Emotion == EmotionType.Angry).Count());
                angryCount.Add(feedbacks.FindAll(item => item.CreatedTime.Date == feedback.CreatedTime.Date && item.Emotion == EmotionType.Happy).Count());
                neutralCount.Add(feedbacks.FindAll(item => item.CreatedTime.Date == feedback.CreatedTime.Date && item.Emotion == EmotionType.Neutral).Count());
            }
            dayEmotionData.Dates = dates;
            dayEmotionData.HappyEmotionCount = happyCount;
            dayEmotionData.AngryEmotionCount = angryCount;
            dayEmotionData.NeutralEmotionCount = neutralCount;

            model.Past3MonthsEmotion = dayEmotionData;


            List<TopIssuesData> topIssues = new List<TopIssuesData>();
            topIssues.Add(new TopIssuesData() { IssueName = "Lag", IssueCount = 10 });
            topIssues.Add(new TopIssuesData() { IssueName = "Hackers", IssueCount = 50 });
            topIssues.Add(new TopIssuesData() { IssueName = "Clipping", IssueCount = 176 });
            topIssues.Add(new TopIssuesData() { IssueName = "Bugs", IssueCount = 17 });

            List<TopIssuesData> orderedIssues = topIssues.OrderByDescending(item => item.IssueCount).ToList();



            model.AllTimeData = new ChartsViewModel.GeneralData()
            {
                TopIssues = orderedIssues,
                AngryEmotionCount = feedbacks.FindAll(item => item.Emotion == EmotionType.Angry).Count(),
                HappyEmotionCount = feedbacks.FindAll(item => item.Emotion == EmotionType.Happy).Count(),
                NeutralEmotionCount = feedbacks.FindAll(item => item.Emotion == EmotionType.Neutral).Count(),
            };
            DateTime pastMonth = DateTime.Now.AddMonths(-1);
            model.Past30Data = new ChartsViewModel.GeneralData()
            {
                TopIssues = orderedIssues,
                AngryEmotionCount = feedbacks.FindAll(item => item.Emotion == EmotionType.Angry && item.CreatedTime >= pastMonth).Count(),
                HappyEmotionCount = feedbacks.FindAll(item => item.Emotion == EmotionType.Happy && item.CreatedTime >= pastMonth).Count(),
                NeutralEmotionCount = feedbacks.FindAll(item => item.Emotion == EmotionType.Neutral && item.CreatedTime >= pastMonth).Count(),
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
