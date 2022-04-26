using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace TicoPay.Api.Common
{
    public class ApplyDocumentVendorExtensions : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            var paths = new Dictionary<string, PathItem>(swaggerDoc.paths);
            swaggerDoc.paths.Clear();
            foreach (var path in paths)
            {
                if (!path.Key.Contains("services/app") && !path.Key.Contains("Abp") && !path.Key.Contains("Tenant") && !path.Key.Contains("TypeScript"))
                    swaggerDoc.paths.Add(path);
            }
        }
    }
}
