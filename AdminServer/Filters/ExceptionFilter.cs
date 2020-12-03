using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace AdminServer.Filters
{
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        public object JsonConvert { get; private set; }

        public void OnException(ExceptionContext context)
        {
            try
            {
                throw context.Exception;
            }
            catch (InvalidOperationException exception)
            {
                
                context.Result = new ContentResult
                {
                    StatusCode = (int?)HttpStatusCode.BadRequest,
                    Content = exception.Message,
                };
            }
            catch (Exception exception)
            {

                context.Result = new ContentResult
                {
                    StatusCode = (int?)HttpStatusCode.InternalServerError,
                    Content = exception.Message,
                };
            }
        }
    }
}
