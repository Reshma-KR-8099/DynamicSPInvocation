using DynamicSPInvocation.Model.Request;
using DynamicSPInvocation.Model.Response;

namespace DynamicSPInvocation.Interface
{
    public interface IHandlingMultipleSP
    {
        Task<APIResponse<Dictionary<string, object>>> ExecuteMultileSP(SPRequest request);
    }
}