using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.SqlServer;
using System;

namespace Hangfire.Intro.Extentions
{
	public static class StartupExtentions
	{
		public static void ConfigureHangFire(this IServiceCollection services, IConfiguration configuration)
		{
			// Add Hangfire services.
			services.AddHangfire(config => config
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
				.UseSimpleAssemblyNameTypeSerializer()
			.UseRecommendedSerializerSettings()
				.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServer.SqlServerStorageOptions { 
					CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
					SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
					QueuePollInterval = TimeSpan.Zero,
					UseRecommendedIsolationLevel= true,
					DisableGlobalLocks=true
				}));

			services.AddHangfireServer();
		}
	}
}
