using DynamicSPInvocation.Interface;
using DynamicSPInvocation.Model.Request;
using DynamicSPInvocation.Model.Response;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace DynamicSPInvocation.Service
{
    public class DynamicSPService : IHandlingMultipleSP
    {
        private readonly string _connectionString;
        private readonly ILogger<DynamicSPService> _logger;


        public DynamicSPService(IConfiguration configuration, ILogger<DynamicSPService> logger)
        {
            _connectionString = configuration.GetConnectionString("DevConnectionString");
            _logger = logger;
        }
        public async Task<APIResponse<Dictionary<string, object>>> ExecuteMultileSP(SPRequest request)
        {
            try
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                var procedureResults = new List<string>();
                bool hasError = false;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    foreach (var procedure in request.procedureNames)
                    {
                        if (string.IsNullOrEmpty(procedure.spName))
                        {
                            _logger.LogError("Invalid Request: Stored Procedure name is missing", "DynamicSPRepo");

                            return new APIResponse<Dictionary<string, object>>
                            {
                                statusCode = HttpStatusCode.BadRequest,
                                statusMessage = "Invalid Request: Stored Procedure name is missing"
                            };
                        }
                        //procedure.parameters = await ParameterMapping(procedure.parameters);
                        
                        _logger.LogInformation($"Successfully completed the parameter mapping", "DynamicSPRepo");

                        using (var command = new SqlCommand(procedure.spName, connection))
                        {
                            
                            _logger.LogInformation($"Started Execution of SP : {procedure.spName}", "DynamicSPRepo");
                            command.CommandType = CommandType.StoredProcedure;
                            foreach (var paramDict in procedure.parameters)
                            {
                                foreach (var param in paramDict)
                                {
                                    command.Parameters.AddWithValue(param.Key, param.Value.ToString());
                                }
                            }

                            try
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    var dataList = new List<Dictionary<string, object>>();

                                    if (reader.HasRows)
                                    {
                                        _logger.LogInformation($"Started to Fetch the result of SP : {procedure.spName}", "DynamicSPRepo");
                                        while (reader.Read())
                                        {
                                            var row = new Dictionary<string, object>();
                                            for (int i = 0; i < reader.FieldCount; i++)
                                            {
                                                row[reader.GetName(i)] = reader.GetValue(i);
                                            }
                                            dataList.Add(row);
                                        }
                                        result[procedure.spName] = dataList;
                                        _logger.LogInformation($"Operation completed successfully : {procedure.spName}", "DynamicSPRepo");
                                    }
                                    else
                                    {
                                        result[procedure.spName] = "Operation completed successfully.";
                                        _logger.LogInformation($"Operation completed successfully : {procedure.spName}", "DynamicSPRepo");
                                    }
                                }
                            }
                            catch (SqlException ex)
                            {
                                _logger.LogError($"An error occurred while executing {procedure.spName}: {ex.Message}", "DynamicSPRepo");
                                procedureResults.Add($"Procedure {procedure.spName} failed: {ex.Message}");
                                hasError = true;
                                break; // Exit loop if a procedure fails
                            }
                            _logger.LogInformation($"Ended Execution of SP : {procedure.spName}", "DynamicSPRepo");
                        }
                    }
                }
                if (hasError)
                {
                    return new APIResponse<Dictionary<string, object>>
                    {
                        Data = result,
                        statusCode = HttpStatusCode.BadRequest,
                        statusMessage = $"One or more procedures failed. {string.Join("; ", procedureResults)}",
                    };
                }
                return new APIResponse<Dictionary<string, object>>
                {
                    Data = result,
                    statusCode = HttpStatusCode.OK,
                    statusMessage = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}", "DynamicSPRepo");
                return new APIResponse<Dictionary<string, object>>
                {
                    statusCode = HttpStatusCode.InternalServerError,
                    statusMessage = ex.Message
                };
            }
        }

        //private async Task<Dictionary<string, object>> ParameterMapping(Dictionary<string, object>requestParameters)
        //{
        //    _logger.LogInformation($"Started mapping of different types of parameters", "DynamicSPRepo");
        //    var parameters = new Dictionary<string, object>();
        //    foreach (var kvp in requestParameters)
        //    {
        //        string value = kvp.Value.ToString();
        //        parameters.Add(kvp.Key, value);
        //    }
        //    return requestParameters = parameters;
        //}
    }
}

