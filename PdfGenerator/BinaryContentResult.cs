// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryContentResult.cs" company="SemanticArchitecture">
//   http://www.SemanticArchitecture.net pkalkie@gmail.com
// </copyright>
// <summary>
//   An ActionResult used to send binary data to the browser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Web;
using System.Web.Mvc;

namespace PdfGenerator
{
    /// <summary>
    /// An ActionResult used to send binary data to the browser.
    /// </summary>
    public class BinaryContentResult : ActionResult
    {
        private readonly string _contentType;
        private readonly byte[] _contentBytes;

        public BinaryContentResult(byte[] contentBytes, string contentType)
        {
            _contentBytes = contentBytes;
            _contentType = contentType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = ((HttpContextBase)context.HttpContext).Response;
            response.Clear();
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = _contentType;
            using (var stream = new MemoryStream(_contentBytes))
            {
                stream.WriteTo(response.OutputStream);
                stream.Flush();
            }
        }
    }
}