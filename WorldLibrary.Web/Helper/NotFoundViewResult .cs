using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WorldLibrary.Web.Helper
{
    public class NotFoundViewResult : ViewResult
    {
        public NotFoundViewResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int)HttpStatusCode.NotFound;

        }
    }
}
