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
        //private readonly IConfiguration _configuration;
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
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT Id, UserId, Title, Completed FROM LiquidLabs", sqlConnection))
                    {
                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                LiquidLabsModel liquidLab = new LiquidLabsModel
                                {
                                    Id = reader.GetInt32(1),
                                    UserId = reader.GetInt32(1),
                                    Title =  reader.GetString(2),
                                    Completed = reader.GetBoolean(3)
                                };
                                liquidLabsList.Add(liquidLab);
                            }
                        }
                    }
                }

                return liquidLabsList;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<LiquidLabsModel> GetById(int id)
        {
            LiquidLabsModel liquidLabsdata = new LiquidLabsModel();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string getQuery = "SELECT Id, UserId, Title, Completed FROM LiquidLabs WHERE Id = @Id";
                SqlCommand command = new SqlCommand(getQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
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
                        string query = "INSERT INTO LiquidLabs (UserId, Title, Completed) VALUES (@UserId, @Title, @Completed); SELECT SCOPE_IDENTITY();";
                        SqlCommand sqlCommand = new SqlCommand(query, connection);

                        sqlCommand.Parameters.AddWithValue("@UserId", liquidLabsdata.UserId);
                        sqlCommand.Parameters.AddWithValue("@Title", liquidLabsdata.Title ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Completed", liquidLabsdata.Completed);
                       
                        // connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        

                    }
                    connection.Close();
                }
                    





            }

            return liquidLabsdata;
        }
    }
}
