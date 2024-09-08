using System.Data.SqlClient;

namespace TMS.Models
{
    public class TaskService
    {
        private readonly string _connectionString = "Server=sql.bsite.net\\MSSQL2016;Database=bharatbuys_db;User Id=bharatbuys_db;Password=Ganesh@123.;TrustServerCertificate=True;MultipleActiveResultSets=true";

        public TaskService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task ExecuteStoredProcedureAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("EXEC DeleteTasksBeforeToday;", connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
