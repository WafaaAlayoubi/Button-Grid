using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ButtonGrid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ButtonGrid.Controllers
{
    public class ButtonController : Controller
    {
        static List<ButtonModel> buttons = new List<ButtonModel>();
        Random random = new Random();
        const int GRID_SIZE = 25;
        public IActionResult Index()
        {
            if(buttons.Count != 25)
                for(int i = 0; i< GRID_SIZE; i++)
                {
                    buttons.Add(new ButtonModel { Id = i, ButtonState = random.Next(4) });
                }
            return View(buttons);
        }

        public IActionResult HandleButtonClick(string buttonNumber)
        {
            //My code
            /*
            int id = Convert.ToInt32(buttonNumber);
            if(buttons[id].ButtonState %3 == 0 && buttons[id].ButtonState != 0)
                buttons[id].ButtonState = 0;
            else
                buttons[id].ButtonState = buttons[id].ButtonState + 1;
            return View("index", buttons); */

            int bn = int.Parse(buttonNumber);

            buttons[bn].ButtonState = (buttons[bn].ButtonState + 1) % 4;

            return View("index", buttons);
        }

        public IActionResult ShowOneButton(int buttonNumber)
        {
            int bn = buttonNumber;

            buttons[bn].ButtonState = (buttons[bn].ButtonState + 1) % 4;

            //1. Render Button and save it to string
            string buttonString = RenderRazorViewToString(this, "ShowOneButton", buttons.ElementAt(buttonNumber));

            //2. Generate a win or loss string based on the state of the buttons array
            bool DidWinYet = true;
            for(int i = 0; i < buttons.Count; i++)
            {
                if (buttons.ElementAt(i).ButtonState != buttons.ElementAt(0).ButtonState)
                    DidWinYet = false;
            }

            string messageString = null;

            if (DidWinYet)
                messageString = "<p>Congratulations. All the buttons match.</p>";
            else
                messageString = "<p>Not all the buttons are the same color</p>";

            //3. Assembly a JSON string that has two parts 
            var package = new { part1 = buttonString , part2 = messageString };

            //4. Send the JSON result.

            return Json(package);

            //5. In the site.js file , the data will have to be interpreted as two pieces of data instead of one

            return PartialView(buttons.ElementAt(buttonNumber));
        }

        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine =
                    controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as
                        ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}