using System;
using System.Net;
using System.Threading.Tasks;
using Elmah.Io.AspNetCore;
//using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace api.Extension
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionMiddleware(RequestDelegate next){

			_next = next;
		}


		public async Task InvokeAsync(HttpContext hpptContext){

			try
			{
				 await _next(hpptContext);
			}
			catch (Exception ex)
			{
				
				 await HandleExceptionAsync(hpptContext, ex);
			}
		}

		public static async Task HandleExceptionAsync(HttpContext httpContext, Exception exeption){
             
             await  exeption.ShipAsync(httpContext);
			 httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
		}
	}
}