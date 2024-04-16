using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClassSchedule.Models;

namespace ClassSchedule.Controllers
{
    public class ClassController : Controller
    {
        private readonly IRepository<Class> _classRepository;
        private readonly IRepository<Day> _dayRepository;
        private readonly IRepository<Teacher> _teacherRepository;

        public ClassController(IRepository<Class> classRepository, IRepository<Day> dayRepository, IRepository<Teacher> teacherRepository)
        {
            _classRepository = classRepository;
            _dayRepository = dayRepository;
            _teacherRepository = teacherRepository;
        }

        public RedirectToActionResult Index()
        {
            // Clear session and navigate to list of classes
            HttpContext.Session.Remove("dayid");
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
            var c = _classRepository.Get(id);
            return View("Add", c);
        }

        [HttpPost]
        public IActionResult Add(Class c)
        {
            string operation = (c.ClassId == 0) ? "Add" : "Edit";
            if (ModelState.IsValid)
            {
                if (c.ClassId == 0)
                    _classRepository.Insert(c);
                else
                    _classRepository.Update(c);
                _classRepository.Save();

                string verb = (operation == "Add") ? "added" : "updated";
                TempData["msg"] = $"{c.Title} {verb}";

                return GoToClasses();
            }
            else
            {
                LoadViewBag(operation);
                return View();
            }
        }

        [HttpGet]
        public ViewResult Delete(int id)
        {
            var c = _classRepository.Get(id);
            ViewBag.DayId = HttpContext.Session.GetInt32("dayid");
            return View(c);
        }

        [HttpPost]
        public RedirectToActionResult Delete(Class c)
        {
            c = _classRepository.Get(c.ClassId); // so can get class title for notification message
            _classRepository.Delete(c);
            _classRepository.Save();

            TempData["msg"] = $"{c.Title} deleted";

            return GoToClasses();
        }

        // private helper methods
        private void LoadViewBag(string operation)
        {
            ViewBag.Days = _dayRepository.List(new QueryOptions<Day>
            {
                OrderBy = d => d.DayId
            });
            ViewBag.Teachers = _teacherRepository.List(new QueryOptions<Teacher>
            {
                OrderBy = t => t.LastName
            });

            ViewBag.Operation = operation;
            ViewBag.DayId = HttpContext.Session.GetInt32("dayid");
        }

        private RedirectToActionResult GoToClasses()
        {
            // if session has a value for day id, add to id route segment when redirecting
            if (HttpContext.Session.GetInt32("dayid").HasValue)
                return RedirectToAction("Index", "Home", new { id = HttpContext.Session.GetInt32("dayid") });
            else
                return RedirectToAction("Index", "Home");
        }
    }

}