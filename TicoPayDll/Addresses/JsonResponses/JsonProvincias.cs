using System;
using System.Collections.Generic;
using System.Text;
using TicoPayDll.Addresses.Dto;

namespace TicoPayDll.Addresses.JsonResponses
{
    public class JsonProvincias
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public Provincia[] listObjectResponse;
    }
}
