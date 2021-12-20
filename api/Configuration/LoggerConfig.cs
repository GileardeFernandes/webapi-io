using System;
using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace api.Configuration
{
	public static class LoggerConfig
	{

		public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services)
		{

			services.AddElmahIo(o =>
			{
				o.ApiKey = "33fcecb1e72848dab30ca3c9901e02f4";
				o.LogId = new Guid("11977147-79eb-4aaf-b3ee-7b32a0c03706");
			});

			// services.AddLogging(builder =>
			// {
            //      builder.AddElmahIo(o => {
            //            o.ApiKey = "33fcecb1e72848dab30ca3c9901e02f4";
			// 	       o.LogId = new Guid("11977147-79eb-4aaf-b3ee-7b32a0c03706");
			// 	 });

			// 	 builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
					
			// });
			return services;
		}


		public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
		{

			app.UseElmahIo();
			return app;
		}
	}
}