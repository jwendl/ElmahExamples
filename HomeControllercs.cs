using System;
using System.Data.Objects;
using System.Web.Mvc;
using Amc.Common.Utility;
using Amc.Common.Objects;
using Amc.Web.OrderPick.DataLayer;
using Amc.Web.OrderPick.Extensions;
using Elmah;

namespace Amc.Web.OrderPick.Controllers
{
    public class HomeController : Controller
    {
        private Employee userDetails;

        private string GetClientIPAddress()
        {
            var ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) {
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            }
            if (ipAddress.Equals("::1")) {
                ipAddress = "127.0.0.1";
            } else if (ipAddress.Contains(":")) {
                ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(":"));
            }
            return ipAddress;
        }

        public ActionResult Index()
        {
            var ipAddress = GetClientIPAddress();
            string body = string.Empty;
            TempData["IpAddress"] = ipAddress;
            return View();
        }

        public ActionResult Dashboard()
        {
            var userId = Request.QueryString["UserId"];
            var ipAddress = GetClientIPAddress();
            TempData["IpAddress"] = ipAddress;
            
            userDetails = DatabaseProxy.GetUserAuthorization(userId);
            if (userDetails == null) {
                TempData["ShowAlert"] = true;
                TempData["AlertMessage"] = "Unable to find user with ID " + userId + " / No se ha podido encontrar el usuario con ID";
                return View("Index");
            }

            if (userDetails.EmployeeLockout != null && userDetails.EmployeeLockout.IsInactive) {
                TempData["ShowAlert"] = true;
                TempData["AlertMessage"] = "You have been inactivated. See the board. / Usted ha sido inactivado. Vease el tablero.";
                return View("Index");
            }

            return View(userDetails);
        }

        [HttpPost]
        public void LogJavaScriptError(string message)
        {
            ErrorSignal.FromCurrentContext().Raise(new JavaScriptErrorException(message + "; " + DateTime.Now.ToString("H:mm:ss.fff tt")));
            string userId = message.Substring(message.IndexOf("*") + 1, 4);
            ExceptionLogger.LogException(userId, new Exception(message + "; " + DateTime.Now.ToString("H:mm:ss.fff tt")), string.Format("Order Pick error from {0}", Request.UserHostAddress));
        }

        [HttpPost]
        public void LogJavaScriptLogMessage(string message)
        {
            ErrorSignal.FromCurrentContext().Raise(new JavaScriptLogMessage(message + "; " + DateTime.Now.ToString("H:mm:ss.fff tt")));
        }

        [HttpPost]
        public static void LogJavaScriptErrorController(string message)
        {
            ErrorSignal.FromCurrentContext().Raise(new JavaScriptErrorException(message + "; " + DateTime.Now.ToString("H:mm:ss.fff tt")));
            string userId = message.Substring(message.IndexOf("*") + 1, 4);
            ExceptionLogger.LogException(userId, new Exception(message + "; " + DateTime.Now.ToString("H:mm:ss.fff tt")));
        }

        [HttpPost]
        public static void LogJavaScriptLogMessageController(string message)
        {
            ErrorSignal.FromCurrentContext().Raise(new JavaScriptLogMessage(message + "; " + DateTime.Now.ToString("H:mm:ss.fff tt")));
        }
    }
}