using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Bari.Prova.Application;
using Bari.Prova.Application.Interface;
using Bari.Prova.Application.Service;

namespace Bari.Prova.IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IMessageService, MessageService>();
        }
    }
}
