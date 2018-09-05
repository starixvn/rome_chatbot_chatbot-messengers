namespace ChatR.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using ChatR.Models;

    public class HomeController : Controller
    {
        private InMemoryRepository _repository;

        public HomeController()
        {
            _repository = InMemoryRepository.GetInstance();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "Username is required");
                return View();
            }

            if (_repository.Users.Where(u => u.Username.Equals(username)).ToList().Count > 0)
            {
                username = _repository.GetRandomizedUsername(username);
            }

            return View("Chat", "_Layout", username);
        }
    }
}