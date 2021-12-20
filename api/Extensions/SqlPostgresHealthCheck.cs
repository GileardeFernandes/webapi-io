using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace api.Extension
{

	public class SqlPostgresHealthCheck : IHealthCheck
	{
		private readonly string _connection;
		public SqlPostgresHealthCheck(string connection)
		{

			_connection = connection;
		}
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			try
			{
				using (var connection = new NpgsqlConnection(_connection))
				{
					await connection.OpenAsync(cancellationToken);

					var cmd = new NpgsqlCommand("select count(*) from \"Produtos\" ", connection);
					var result = await cmd.ExecuteReaderAsync(cancellationToken);
					var total = 0;
					while (await result.ReadAsync())
						total++;

					return total > 0 ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
				}
			}
			catch (Exception)
			{

				return HealthCheckResult.Unhealthy();
			}
		}
	}
}