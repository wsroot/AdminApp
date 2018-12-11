using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AdminApp.Helper.Swagger
{
    public class SwaggerHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation,OperationFilterContext context)
        {
            var fileParameters = context.ApiDescription.ActionAttributes().Where(m => m.GetType().ToString().Contains("Authorize")).ToList();
            if (fileParameters.Count>0)
            {
                operation.Parameters.Add(new NonBodyParameter()
                {
                    Name="access_token",
                    In="header",
                    Type="string",
                    Required=true,
                    Default="Bearer "
                });
            }
        }
    }
}
