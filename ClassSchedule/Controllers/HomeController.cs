using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClassSchedule.Models;

namespace ClassSchedule.Controllers
{
    public class HomeController : Controller

    {

        private readonly IRepository<Day> _dayRepository;
        private readonly IRepository<Class> _classRepository;

        public HomeController(IRepository<Day> dayRepository, IRepository<Class> classRepository)
        {
            _dayRepository = dayRepository;
            _classRepository = classRepository;
        }

        public ViewResult Index(int id)
        {

            if (id > 0)
            {
                HttpContext.Session.SetInt32("dayid", id);
            }

            // options for Days query
            var dayOptions = new QueryOptions<Day>
            {
                OrderBy = d => d.DayId
            };

            // options for Classes query
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


            ViewBag.Days = _dayRepository.List(dayOptions);
            return View(_classRepository.List(classOptions));
        }
    }
}