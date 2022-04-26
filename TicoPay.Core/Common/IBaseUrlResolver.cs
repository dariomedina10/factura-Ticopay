using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TicoPay.Common
{
    public interface IBaseUrlResolver
    {
        bool IsResolved { get; }

        string GetBaseUrl();
        void SetBaseUrl(string baseUrl);
        void ResolveBaseUrlFromRequest(HttpRequest httpRequest);
    }
}
