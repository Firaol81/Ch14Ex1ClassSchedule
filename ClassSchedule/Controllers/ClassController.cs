using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClassSchedule.Models;

namespace ClassSchedule.Controllers
{
    public class ClassController : Controller
    {
        private readonly ClassScheduleUnitOfWork data;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ClassController(ClassScheduleContext ctx, IHttpContextAccessor httpContextAccessor)
        {
            data = new ClassScheduleUnitOfWork(ctx);
            this.httpContextAccessor = httpContextAccessor;
        }

        public RedirectToActionResult Index()
        {
            // Clear session and navigate to the list of classes
            httpContextAccessor.HttpContext.Session.Remove("dayid");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ViewResult Add()
        {
            LoadViewBag("Add");
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            LoadViewBag("Edit");
            var cls = GetClass(id);
            return View("Add", cls);
        }

        [HttpPost]
        public IActionResult Add(Class cls)
        {
            string operation = (cls.ClassId == 0) ? "Add" : "Edit";
            if (ModelState.IsValid)
            {
                if (cls.ClassId == 0)
                    data.Classes.Insert(cls);
                else
                    data.Classes.Update(cls);

                data.Classes.Save();

                string verb = (operation == "Add") ? "added" : "updated";
                TempData["message"] = $"{cls.Title} {verb}";

                return GoToClasses();
            }
            else
            {
                LoadViewBag(operation);
                return View(cls);
            }
        }

        [HttpGet]
        public ViewResult Delete(int id)
        {
            var cls = GetClass(id);
            return View(cls);
        }

        [HttpPost]
        public RedirectToActionResult Delete(Class cls)
        {
            cls = data.Classes.Get(cls.ClassId); // Retrieve class details for a notification message

            data.Classes.Delete(cls);
            data.Classes.Save();

            TempData["message"] = $"{cls.Title} deleted";

            return GoToClasses();
        }

        // Private helper methods
        private Class GetClass(int id)
        {
            var classOptions = new QueryOptions<Class>
            {
                Includes = "Teacher, Day",
                Where = c => c.ClassId == id
            };
            return data.Classes.Get(classOptions);
        }

        private void LoadViewBag(string operation)
        {
            ViewBag.Days = data.Days.List(new QueryOptions<Day>
            {
                OrderBy = d => d.DayId
            });
            ViewBag.Teachers = data.Teachers.List(new QueryOptions<Teacher>
            {
                OrderBy = t => t.LastName
            });
            ViewBag.Operation = operation;
        }

        private RedirectToActionResult GoToClasses()
        {
            // Check if the session has a value for day id
            int? dayId = httpContextAccessor.HttpContext.Session.GetInt32("dayid");
            if (dayId.HasValue)
                return RedirectToAction("Index", "Home", new { id = dayId.Value });
            else
                return RedirectToAction("Index", "Home");
        }
    }
}
