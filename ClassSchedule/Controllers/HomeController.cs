using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClassSchedule.Models;
using System.Linq;

namespace ClassSchedule.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClassScheduleUnitOfWork data;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ClassScheduleContext ctx, IHttpContextAccessor httpContextAccessor)
        {
            data = new ClassScheduleUnitOfWork(ctx);
            _httpContextAccessor = httpContextAccessor;
        }

        public ViewResult Index(int id)
        {
            // Set or get the current day ID from session
            if (id > 0)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("dayid", id);
            }
            else
            {
                id = _httpContextAccessor.HttpContext.Session.GetInt32("dayid") ?? 0;
            }

            // Load days for view bag
            var dayOptions = new QueryOptions<Day>
            {
                OrderBy = d => d.DayId
            };
            ViewBag.Days = data.Days.List(dayOptions);

            // Query classes based on the current day ID
            var classOptions = new QueryOptions<Class>
            {
                Includes = "Teacher, Day"
            };

            if (id == 0)
            {
                classOptions.OrderBy = c => c.DayId;
            }
            else
            {
                classOptions.Where = c => c.DayId == id;
                classOptions.OrderBy = c => c.MilitaryTime;
            }

            var classes = data.Classes.List(classOptions);
            return View(classes);
        }

        public IActionResult Cancel()
        {
            int? dayId = _httpContextAccessor.HttpContext.Session.GetInt32("dayid");
            if (dayId.HasValue)
            {
                return RedirectToAction("Index", new { id = dayId.Value });
            }
            return RedirectToAction("Index");
        }

        public IActionResult SomeAction()
        {
            var sessionValue = _httpContextAccessor.HttpContext.Session.GetInt32("dayid");
            // Additional logic based on sessionValue
            return View();
        }
    }
}
