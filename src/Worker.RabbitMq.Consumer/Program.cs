using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Monitor.Service.Model.Settings;
using Rabbit.Domain.Configuration;
using Service.Filter;

namespace Worker.RabbitMq.Consumer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();   //валидация при старте чтения конфигурации приложения

					services.AddOptions();
					var options = hostContext.Configuration.GetSection("RabbitMq");
					services.Configure<RabbitOptions>(hostContext.Configuration.GetSection("RabbitMq"));
					services.AddSingleton(resolver =>
							   resolver.GetRequiredService<IOptions<RabbitOptions>>().Value);            //IOptionsSnapshot
																										 // Register as an IValidatable
					services.AddSingleton<IValidatable>(resolver =>
						resolver.GetRequiredService<IOptions<RabbitOptions>>().Value);

					
					//services.AddHostedService<RabbitMqListener>();
					services.AddHostedService<Worker>();
				});
	}
}
