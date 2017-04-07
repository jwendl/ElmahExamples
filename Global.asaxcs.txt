using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Amc.Web.OrderPick.App_Start;
using Elmah;

namespace Amc.Web.OrderPick
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

    }
}

namespace Amc.Web.OrderPick.Extensions
{
    public class ElmahHandledErrorLoggerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Log only handled exceptions, because all other will be caught by ELMAH anyway.
            if (context.ExceptionHandled)
                ErrorSignal.FromCurrentContext().Raise(context.Exception);
        }
    }

    [Serializable]
    public class JavaScriptErrorException : Exception
    {
        public JavaScriptErrorException(string message)
            : base(message)
        {
        }
    }

    [Serializable]
    public class JavaScriptLogMessage : Exception
    {
        public JavaScriptLogMessage(string message)
            : base(message)
        {
        }
    }
}