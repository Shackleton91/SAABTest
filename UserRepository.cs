using System;
using System.Data.SqlClient;

namespace TicketManagementSystem
{
    public class UserRepository : IDisposable
    {
        private readonly SqlConnection _connection;

        public UserRepository()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["database"].ConnectionString;
            _connection = new SqlConnection(connectionString);
        }

        public User GetUserByUsername(string username)
        {
            try
            {
                string sql = "SELECT TOP 1 * FROM Users WHERE Username = @p1";
                _connection.Open();

                using (var command = new SqlCommand(sql, _connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add("@p1", System.Data.SqlDbType.NVarChar).Value = username;

                    return (User)command.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }

        public User GetAccountManager()
        {
            // Assume this method does not need to change.
            return GetUserByUsername("Sarah");
        }

        public void Dispose()
        {
            // Assume this method does not need to change.
            _connection.Dispose();
        }
    }
} 
