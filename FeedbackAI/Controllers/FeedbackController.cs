using FeedbackAI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static FeedbackAI.Models.Feedback;
using static FeedbackAI.Models.FeedbackViewModel;

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

        [HttpGet]
        public async Task<IActionResult> GetCharts()
        {
            var feedbacks = applicationDBContext.Feedbacks.OrderBy(item => item.CreatedTime);
            var dates = feedbacks.Select(item => item.CreatedTime.ToString("dd/MM/yyyy"));
            var emotions = feedbacks.Select(item => item.Emotion);

            var countException = feedbacks.ToArray();
            return new JsonResult(new { dates, emotions });
        }
        public IActionResult Charts()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
