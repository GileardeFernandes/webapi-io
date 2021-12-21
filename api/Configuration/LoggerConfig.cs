using System;
using api.Extension;
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore.HealthChecks;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace api.Configuration
{
	public static class LoggerConfig
	{

		public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
		{

			services.AddElmahIo(o =>
			{
				o.ApiKey = "acb70f4b0f36450bbee64abf5890e35a";
				o.LogId = new Guid("fb374214-01f6-4368-ba9b-348b097944a0");
			});

			//AddNgSQL verifica se esta tudo funcionando com o banco informado no parametro
			services.AddHealthChecks()
			        .AddElmahIoPublisher("acb70f4b0f36450bbee64abf5890e35a", new Guid("fb374214-01f6-4368-ba9b-348b097944a0"), "My WEBAPI .NET Core")
			        .AddNpgSql(configuration.GetConnectionString("DefaultConnection"), name:"Banco postgres")
					.AddCheck("Produtos", new SqlPostgresHealthCheck(configuration.GetConnectionString("DefaultConnection")));
					
					
			services.AddHealthChecksUI();

            //Configuração para pegar os logs inseridos na mão
			// services.AddLogging(builder =>
			// {
            //      builder.AddElmahIo(o => {
            //            o.ApiKey = "acb70f4b0f36450bbee64abf5890e35a";
			// 	       o.LogId = new Guid("fb374214-01f6-4368-ba9b-348b097944a0");
			// 	 });

			// 	 builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
					
			// });
			return services;
		}


		public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
		{

			app.UseElmahIo();
			app.UseHealthChecks("/api/hc", new HealthCheckOptions(){

				Predicate = _ => true,
				ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
			});

			app.UseHealthChecksUI(options =>{

				options.UIPath = "/api/hc-ui";
			});
			
			return app;
		}
	}
}