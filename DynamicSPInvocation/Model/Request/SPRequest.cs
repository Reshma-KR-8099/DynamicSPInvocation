namespace DynamicSPInvocation.Model.Request
{
    public class SPRequest
    {
        public List<ProcedureNames> procedureNames { get; set; }
    }
    public class ProcedureNames
    {
        public string spName { get; set; }
        public List<Dictionary<string, object>> parameters { get; set; }
    }
}