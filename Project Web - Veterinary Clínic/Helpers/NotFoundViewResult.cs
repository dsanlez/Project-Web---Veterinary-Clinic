using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Project_Web___Veterinary_Clínic.Helpers
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
