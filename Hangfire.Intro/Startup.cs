using Hangfire.Intro.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hangfire;
using Hangfire.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Intro.Extentions;

namespace Hangfire.Intro
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddSingleton<WeatherForecastService>();

			services.ConfigureHangFire(Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
			IBackgroundJobClient backgroundJobClient,
			IRecurringJobManager recurringJobManager,
			IServiceProvider serviceProvider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
				endpoints.MapHangfireDashboard();
			});

			app.UseHangfireDashboard();

			backgroundJobClient.Enqueue(() => Console.WriteLine("Hello from HangFire"));
			backgroundJobClient.Schedule(() => Console.WriteLine("Hello with Delay"), TimeSpan.FromSeconds(30));

			recurringJobManager.AddOrUpdate("This will run every minute",
				() => Console.WriteLine("This is a recurrent job"), Cron.Minutely
			);

			var weatherForecastService = serviceProvider.GetRequiredService<WeatherForecastService>();
			recurringJobManager.AddOrUpdate("WeatherForecast Service",
				() => weatherForecastService.GetForecastAsync(DateTime.Now), Cron.Minutely);
		}
	}
}
