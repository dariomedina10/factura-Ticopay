using System;
using System.Collections.Generic;
using System.Text;
using TicoPayDll.Tenants.Dto;

namespace TicoPayDll.Tenants.JsonResponses
{
    public class JsonTenant
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public Tenant objectResponse;
    }
}
