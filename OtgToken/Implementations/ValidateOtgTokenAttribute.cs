﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace OtgToken
{
    /// <summary>
    /// Action filter attribute
    /// 
    /// Checks for headers:  
    ///     "Otg-Authenticate-Key" - must be equal `Key` property
    ///     "Otg-Authenticate-RequestId" - not processed before. Used to exclude request generated by sniffers
    ///     "Otg-Authenticate-Token" - validate by DateTime.Now() and publickey which is a concatenation of "Otg-Authenticate-Key" and "Otg-Authenticate-RequestId"
    /// </summary>
    public class ValidateOtgTokenAttribute : ActionFilterAttribute
    {
        public static string OtgKeyHeaderName { get; private set; } = "Otg-Authenticate-Key";
        public static string OtgTokenHeaderName { get; private set; } = "Otg-Authenticate-Token";
        public static string OtgRequestIdHeaderName { get; private set; } = "Otg-Authenticate-RequestId";

        /// <summary>
        /// Filter by "Otg-Authenticate-Key" header
        /// </summary>
        public string Key { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                Validate(context);

                await base.OnActionExecutionAsync(context, next);
            }
            catch (Exception)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status406NotAcceptable);
            }
        }

        private void Validate(ActionExecutingContext context)
        {
            var OtgToken = context.HttpContext.RequestServices.GetService<IOtgToken>();

            var requestHeaders = context.HttpContext.Request.Headers;

            // "Otg-Authenticate-RequestId" - not processed before. Used to exclude request generated by sniffers
            var cache = context.HttpContext.RequestServices.GetService<IMemoryCache>();
            if (!requestHeaders.ContainsKey(OtgRequestIdHeaderName) || cache.Get(requestHeaders[OtgRequestIdHeaderName]) != null)
            {
                throw new Exception();
            }
            cache.Set(requestHeaders[OtgRequestIdHeaderName], 0, TimeSpan.FromMinutes(3));

            // "Otg-Authenticate-Key" - must be equal `Key` property
            if (!requestHeaders.ContainsKey(OtgKeyHeaderName) || requestHeaders[OtgKeyHeaderName] != Key)
            {
                throw new Exception();
            }
            // "Otg-Authenticate-Token" - validate by DateTime.Now() and publickey which is a concatenation of "Otg-Authenticate-Key" and "Otg-Authenticate-RequestId"
            if (!requestHeaders.ContainsKey(OtgTokenHeaderName))
            {
                throw new Exception();
            }
            if (!OtgToken.Validate(requestHeaders[OtgTokenHeaderName], requestHeaders[OtgKeyHeaderName] + requestHeaders[OtgRequestIdHeaderName]))
            {
                throw new Exception();
            }
        }
    }
}
