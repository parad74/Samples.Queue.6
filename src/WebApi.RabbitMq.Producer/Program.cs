using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Monitor.Service.Model.Settings;
using Rabbit.Domain.Configuration;
using Rabbit.WebApi.Services;
using Service.Filter;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();   //валидация при старте чтения конфигурации приложения

// Add services to the container.
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

// Bind the configuration using IOptions   https://andrewlock.net/adding-validation-to-strongly-typed-configuration-objects-in-asp-net-core/


builder.Services.AddOptions();
var options = builder.Configuration.GetSection("RabbitMq");
builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton(resolver =>
		   resolver.GetRequiredService<IOptions<RabbitOptions>>().Value);            //IOptionsSnapshot
																					   // Register as an IValidatable
builder.Services.AddSingleton<IValidatable>(resolver =>
	resolver.GetRequiredService<IOptions<RabbitOptions>>().Value);



builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddMvc()
		.AddNewtonsoftJson()
		.AddControllersAsServices(); // IMPORTANT: Adding this line instructs WebHost to resolve Controllers from DI (Unity)

builder.Services.AddHttpClient();
//services.AddSignalR();
//services.AddSession();

//services.AddLogging();
//services.AddLogging(config =>
//{
//	// clear out default configuration
//	config.ClearProviders();

//	config.AddConfiguration(Configuration.GetSection("Logging"));
//	config.AddDebug();
//	config.AddEventSourceLogger();

//	if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ==
//	Microsoft.Extensions.Hosting.Environments.Development)
//	{
//		config.AddConsole();
//	}
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "WebApi.RabbitMq.Producer",
		Description = "RabbitMq Producer WEB API"
	});

	//// Set the comments path for the Swagger JSON and UI.
	var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	c.IncludeXmlComments(xmlPath);
	c.CustomSchemaIds(i => i.FullName);
});

//services.AddHttpContextAccessor();// вы можете запросить объект IHttpContextAccessor в конструкторе объекта. Хотя объект IHttpContextAccessor не имеет свойства User, он дает вам доступ к объекту HttpContext, который имеет свойство User. 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core 3.0 web API v1");
		//c.RoutePrefix = string.Empty;
	});
}

//app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(policy =>
policy.AllowAnyOrigin() //WithOrigins("http://localhost:5000", "https://localhost:5001"
.AllowAnyMethod()
.AllowAnyHeader());
//.WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization));//.AllowCredentials());


app.UseAuthorization();

app.MapControllers();

app.Run();