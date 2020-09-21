using Bari.Prova.Application.Interface;
using Bari.Prova.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bari.Prova.MicroServico
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var collection = new ServiceCollection();
            NativeInjectorBootStrapper.RegisterServices(collection);

            try
            {
                Log.Information("Starting up");
                

                IServiceProvider serviceProvider = collection.BuildServiceProvider();
                
                var messageService = serviceProvider.GetService<IMessageService>();

                Task.Run(() =>
                {
                    while(true)
                    {
                        messageService.SendMessage(new Domain.Message("Bari.MicroServico01", DateTime.Now, Guid.NewGuid().ToString(), "Hello World!"));
                        Thread.Sleep(5000);
                    }
                });
                messageService.Listener += (obj, e) =>
                {
                    //var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Message>(obj.ToString());
                    Log.Information($"Message received: {obj}");
                };
                messageService.StartListener();
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }

           

        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog();
                
        }
    }
}
