using DynamicSPInvocation.Interface;
using DynamicSPInvocation.Model.Request;
using DynamicSPInvocation.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text.Json;
using System.Xml.Linq;

namespace DynamicSPInvocation.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicController : ControllerBase
    {
        private readonly IHandlingMultipleSP _handlingMultipleSP;
        private readonly ILogger<DynamicController> _logger;

        public DynamicController(IHandlingMultipleSP handlingMultipleSP, ILogger<DynamicController> logger)
        {
            _handlingMultipleSP = handlingMultipleSP;
            _logger = logger;
        }

        [HttpPost]
        [Route("ExecuteMultipleSP")]
        public async Task<APIResponse<Dictionary<string, object>>> ExecuteSP(SPRequest request)
        {
            LogContext.PushProperty("API", "ExecuteMultipleSP");
            LogContext.PushProperty("Method", "ExecuteSP");
            var CorrID = Guid.NewGuid().ToString();
            LogContext.PushProperty("CorrelationID", CorrID);
            try
            {
                if (request.procedureNames == null || request.procedureNames.Count <= 0)
                {
                    _logger.LogError("Invalid Request:", "DynamicController");
                    return new APIResponse<Dictionary<string, object>>
                    {
                        statusCode = HttpStatusCode.BadRequest,
                        statusMessage = "Invalid Request"
                    };
                }

                _logger.LogInformation($"Entered into Dynamic SP controller", "DynamicController");

                APIResponse<Dictionary<string, object>> result = await _handlingMultipleSP.ExecuteMultileSP(request);

                if (result.Data != null && result.Data?.Count > 0)
                {
                    result.correlationID = CorrID;
                    _logger.LogInformation("Successfully completed the SP operation", "DynamicController");
                    return result;
                }
                else
                {
                    _logger.LogError($"No data found for the provided request: {result.statusMessage}", "DynamicController");

                    return new APIResponse<Dictionary<string, object>>
                    {
                        statusCode = HttpStatusCode.BadRequest,
                        statusMessage = result.statusMessage,
                        correlationID=CorrID
                    };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}", "DynamicController");

                return new APIResponse<Dictionary<string, object>>
                {
                    statusCode = HttpStatusCode.InternalServerError,
                    statusMessage = ex.Message,
                    correlationID = CorrID
                };
            }

        }
    }
}