using Abp.WebApi.Controllers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Description;

namespace TicoPay.Api.Common
{    
    public class TicoPayApiController : AbpApiController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage StreamToFile(Stream stream, string fileName, string mediaType)
        {
            stream.Position = 0;
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            return response;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage JsonResponse(string json)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }
    }
}
