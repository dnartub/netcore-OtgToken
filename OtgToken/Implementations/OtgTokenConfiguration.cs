using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace OtgToken
{
    public static class OtgTokenConfiguration
    {
        /// <summary>
        /// Add OtgToken to DI-services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOtgToken(this IServiceCollection services)
        {
            services.AddSingleton<IOtgToken, OtgToken>();
            services.AddMemoryCache(); // using in ValidateOtgTokenAttribute
            return services;
        }
    }
}
