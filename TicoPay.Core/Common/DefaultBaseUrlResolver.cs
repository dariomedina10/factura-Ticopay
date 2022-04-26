using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using TicoPay.Web.Helpers;

namespace TicoPay.Common
{
    public class DefaultBaseUrlResolver : IBaseUrlResolver
    {
        private static string _CurrentBaseUrl;

        public bool IsResolved
        {
            get
            {
                return !string.IsNullOrEmpty(_CurrentBaseUrl);
            }
        }

        public DefaultBaseUrlResolver()
        {
            if (HttpContext.Current != null)
            {
                ResolveBaseUrlFromRequest(HttpContext.Current.Request);
            }
        }

        public string GetBaseUrl()
        {
            return _CurrentBaseUrl;
        }

        public void SetBaseUrl(string baseUrl)
        {
            _CurrentBaseUrl = baseUrl;
        }

        public void ResolveBaseUrlFromRequest(HttpRequest httpRequest)
        {
            if (httpRequest != null)
            {
                _CurrentBaseUrl = string.Format("{0}://{1}{2}{3}",
                            httpRequest.Url.Scheme,
                            ((Regex.Matches(httpRequest.Url.Host, @"[a-zA-Z]").Count == 0) || (httpRequest.Url.Host.Contains(".ticopays.com")) || (httpRequest.Url.Host == "ticopays.com")) ? "www.ticopays.com" : ((httpRequest.Url.Host.AllIndexesOf(".").Count > 1) ? httpRequest.Url.Host.Substring(httpRequest.Url.Host.IndexOf(".") + 1) : httpRequest.Url.Host),
                            (httpRequest.Url.Port == 80 || httpRequest.Url.Port == 443) ? string.Empty : ":" + httpRequest.Url.Port,
                            httpRequest.ApplicationPath);
            }
            if (!_CurrentBaseUrl.EndsWith("/"))
            {
                _CurrentBaseUrl += "/";
            }
        }
    }
}
