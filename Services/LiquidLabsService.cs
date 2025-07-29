using LiquidLapsAPI.Models;
using LiquidLapsAPI.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace LiquidLapsAPI.Services
{
    public class LiquidLabsService : ILiquidLabsService
    {
        private readonly string _connectionString;
        private readonly HttpClient _httpClient;
        public LiquidLabsService(IConfiguration configuration, HttpClient httpClient)
        {

            _httpClient = httpClient;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<List<LiquidLabsModel>> GetAll()
        {
            try
            {
                List<LiquidLabsModel> liquidLabsList = new List<LiquidLabsModel>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT Id, UserId, Title, Completed FROM Labs";
                    using (SqlCommand sqlCommand = new SqlCommand(query, connection))
                    
                    using(SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            
                            while (await reader.ReadAsync())
                            {
                                liquidLabsList.Add(
                                    new LiquidLabsModel
                                    {
                                        Id = reader.GetInt32(0),
                                        UserId = reader.GetInt32(1),
                                        Title = reader.GetString(2),
                                        Completed = reader.GetBoolean(3)
                                    });                                
                            }     
                            reader.Close();

                        }
                        else
                        {
                            reader.Close();
                            var response = await _httpClient.GetAsync($"https://jsonplaceholder.typicode.com/todos");
                            if (response.IsSuccessStatusCode)
                            {
                                var labs = await response.Content.ReadFromJsonAsync<List<LiquidLabsModel>>();
                                if (labs != null && labs.Count > 0)
                                {
                                    foreach (var lab in labs)
                                    {
                                        LiquidLabsModel liquidLabsdata = new LiquidLabsModel();
                                        liquidLabsdata.Id = lab.Id;
                                        liquidLabsdata.UserId = lab.UserId;
                                        liquidLabsdata.Title = lab.Title;
                                        liquidLabsdata.Completed = lab.Completed;
                                        liquidLabsList.Add(liquidLabsdata);

                                        query = "INSERT INTO Labs (Id,UserId, Title, Completed) VALUES (@Id, @UserId, @Title, @Completed); SELECT SCOPE_IDENTITY();";
                                        SqlCommand command = new SqlCommand(query, connection);
                                        command.Parameters.AddWithValue("@Id", liquidLabsdata.Id);
                                        command.Parameters.AddWithValue("@UserId", liquidLabsdata.UserId);
                                        command.Parameters.AddWithValue("@Title", liquidLabsdata.Title ?? (object)DBNull.Value);
                                        command.Parameters.AddWithValue("@Completed", liquidLabsdata.Completed);

                                        command.ExecuteNonQuery();
                                    }

                                }
                                else
                                {
                                    throw new Exception("No data found from external API.");
                                }

                            }
                            connection.Close();
                        }
                    }
                   
                }


                return liquidLabsList;
            }
            catch (Exception ex)
            {

                throw new Exception("unable to get the access");
            }

        }

       
        public async Task<LiquidLabsModel> GetById(int id)
        {
            try
            {
                LiquidLabsModel liquidLabsdata = new LiquidLabsModel();

               

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT Id, UserId, Title, Completed FROM Labs WHERE Id = @Id";
                   
                    SqlCommand sqlcommand = new SqlCommand(query, connection);
                    sqlcommand.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    
                    SqlDataReader reader = await sqlcommand.ExecuteReaderAsync();
                    if (reader.Read())
                    {

                        liquidLabsdata.Id = reader.GetInt32(0);
                        liquidLabsdata.UserId = reader.GetInt32(1);
                        liquidLabsdata.Title = reader.IsDBNull(2) ? null : reader.GetString(2);
                        liquidLabsdata.Completed = reader.GetBoolean(3);

                        reader.Close();
                    }
                    else
                    {
                        reader.Close();
                        var response = await _httpClient.GetAsync($"https://jsonplaceholder.typicode.com/todos/{id}");
                        if (response.IsSuccessStatusCode)
                        {
                            var lab = await response.Content.ReadFromJsonAsync<LiquidLabsModel>();
                            if (lab != null)
                            {
                                liquidLabsdata.Id = id;
                                liquidLabsdata.UserId = lab.UserId;
                                liquidLabsdata.Title = lab.Title;
                                liquidLabsdata.Completed = lab.Completed;
                            }
                            query = "INSERT INTO Labs (Id,UserId, Title, Completed) VALUES (@Id,@UserId, @Title, @Completed)";
                            SqlCommand sqlCommand = new SqlCommand(query, connection);
                            sqlCommand.Parameters.AddWithValue("@Id", liquidLabsdata.Id);
                            sqlCommand.Parameters.AddWithValue("@UserId", liquidLabsdata.UserId);
                            sqlCommand.Parameters.AddWithValue("@Title", liquidLabsdata.Title ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@Completed", liquidLabsdata.Completed);

                            sqlCommand.ExecuteNonQuery();

                        }
                        connection.Close();
                    }
                }

                return liquidLabsdata;
            }
            catch
            {
                throw new Exception("unable to get the access") ;
            }
        }

        public async Task<List<LiquidLabsModel>> GetByUserId(int userId)
        {
            try
            {
                List<LiquidLabsModel> liquidLabsList = new List<LiquidLabsModel>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT Id, UserId, Title, Completed FROM Labs WHERE UserId = @UserId";
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        
                        if (reader.HasRows)
                        {

                            while (await reader.ReadAsync())
                            {
                                liquidLabsList.Add(
                                    new LiquidLabsModel
                                    {
                                        Id = reader.GetInt32(0),
                                        UserId = reader.GetInt32(1),
                                        Title = reader.GetString(2),
                                        Completed = reader.GetBoolean(3)
                                    });
                            }
                            reader.Close();

                        }
                        else
                        {
                            reader.Close();
                            var response = await _httpClient.GetAsync($"https://jsonplaceholder.typicode.com/users/{userId}/todos");
                            if (response.IsSuccessStatusCode)
                            {
                                var labs = await response.Content.ReadFromJsonAsync<List<LiquidLabsModel>>();
                                if (labs != null && labs.Count > 0)
                                {
                                    foreach (var lab in labs)
                                    {
                                        LiquidLabsModel liquidLabsdata = new LiquidLabsModel();
                                        liquidLabsdata.Id = lab.Id;
                                        liquidLabsdata.UserId = lab.UserId;
                                        liquidLabsdata.Title = lab.Title;
                                        liquidLabsdata.Completed = lab.Completed;
                                        liquidLabsList.Add(liquidLabsdata);

                                        query = "INSERT INTO Labs (Id,UserId, Title, Completed) VALUES (@Id ,@UserId, @Title, @Completed);";
                                        SqlCommand command = new SqlCommand(query, connection);
                                        command.Parameters.AddWithValue("@Id", liquidLabsdata.Id);
                                        command.Parameters.AddWithValue("@UserId", liquidLabsdata.UserId);
                                        command.Parameters.AddWithValue("@Title", liquidLabsdata.Title ?? (object)DBNull.Value);
                                        command.Parameters.AddWithValue("@Completed", liquidLabsdata.Completed);

                                        command.ExecuteNonQuery();
                                    }

                                }
                                else
                                {
                                    throw new Exception("No data found from external API.");
                                }

                            }
                            connection.Close();
                        }
                    }

                }
                return liquidLabsList;
            }
            catch (Exception ex)
            {

                throw new Exception("unable to get the access");
            }

        }




    }
}
