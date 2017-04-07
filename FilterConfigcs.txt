using System.Web.Mvc;
using Amc.Web.OrderPick.Extensions;

namespace Amc.Web.OrderPick.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {

            filters.Add(new ElmahHandledErrorLoggerFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}