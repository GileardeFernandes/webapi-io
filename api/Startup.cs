
using api.Configuration;
using Api.Configuration;
using AutoMapper;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<MeuDbContext>(Options =>
			{
				Options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
			});

			services.AddIdentityConfiguration(Configuration);
			services.AddAutoMapper(typeof(Startup));
            services.WebApiConfig();
            services.AddSwaggerConfig();
			services.ResolveDependencies();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
		{
			if (env.IsDevelopment())
			{
				app.UseCors("Development");
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseCors("Producion");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts
				app.UseHsts();
			}
            
			//UseAuthentication precisa estar antes do MVC
			app.UseAuthentication();
            app.UseMvConfiuration();

			app.UseSwaggerConfig(provider);
           
		}
	}
}
