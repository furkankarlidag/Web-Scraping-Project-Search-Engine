using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Zakuska_Scholar.Models;

namespace Zakuska_Scholar.Controllers
{
    public class DetayController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        
        public IActionResult Index(articleModel articleModel)
        {
            Console.WriteLine(articleModel.articleID);
            return View(articleModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
