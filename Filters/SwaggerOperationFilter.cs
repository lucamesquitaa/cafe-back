using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Turify.Filters
{
  public class SwaggerOperationFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      // Adiciona parâmetros de rota que podem não estar sendo detectados
      if (context.ApiDescription.RelativePath?.Contains("{CafeteriaId}") == true)
      {
        if (operation.Parameters == null)
          operation.Parameters = new List<OpenApiParameter>();

        // Verifica se o parâmetro CafeteriaId já existe
        if (!operation.Parameters.Any(p => p.Name == "CafeteriaId"))
        {
          operation.Parameters.Add(new OpenApiParameter
          {
            Name = "CafeteriaId",
            In = ParameterLocation.Path,
            Required = true,
            Schema = new OpenApiSchema { Type = "string" }
          });
        }
      }
    }
  }
}
