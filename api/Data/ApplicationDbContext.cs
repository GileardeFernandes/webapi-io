using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace  api.Data
{
	public class ApplicationDbContext : IdentityDbContext {

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

		internal Task Query(string v)
		{
			throw new NotImplementedException();
		}
	}
}