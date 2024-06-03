using FeedbackAI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static FeedbackAI.Models.Feedback;

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
        public async Task<IActionResult> Index(EmotionType searchEmotionType, string searchString)
        {
            FeedbackViewModel model = new FeedbackViewModel();
            var feedbacks = applicationDBContext.Feedbacks.ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                feedbacks = feedbacks.FindAll(item => item.Description.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));
            }
            if(searchEmotionType != EmotionType.Empty)
            {
                feedbacks = feedbacks.FindAll(item => item.Emotion == searchEmotionType); 
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
