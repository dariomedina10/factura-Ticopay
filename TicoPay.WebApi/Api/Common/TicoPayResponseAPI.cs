using Abp.Application.Services.Dto;
using System.Net;
using TicoPay.MultiTenancy.Dto;
using System.ComponentModel;
using TicoPay.Application.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TicoPay.Api.Controllers
{
    public class TicoPayResponseAPI<T>
    {
        public bool Success { get; internal set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? total_elements { get; internal set; }     
        public int StatusCode { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T ObjectResponse { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<T> ListObjectResponse { get; internal set; }

        public TicoPayResponseAPI( HttpStatusCode  statusCode, T item , ListResultDto<T> items=null)
        {
            ListObjectResponse = items == null ? null : items.Items;
            ObjectResponse = item;
            Success = (statusCode == HttpStatusCode.Accepted || statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created);
            StatusCode = (int)statusCode;
            if ((item!=null)||(items!=null))
                total_elements = items == null ? 1 : items.Items.Count;
            
        }
       


    }

    //public TicoPayResponseAPI(T item, HttpStatusCode statusCode, string message = null)
    //{
    //    ObjectResponse = item;
    //    Success = (statusCode == HttpStatusCode.Accepted || statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created);
    //    StatusCode = (int)statusCode;
    //    Message = message;
    //}

    public class TicoPayResponseErrorAPI
    {
        public bool success { get; internal set; }
        public string error_msg { get; internal set; }
        public int error_code { get; internal set; }
        public string innerExcepcion { get; set; }


        public TicoPayResponseErrorAPI(Error statusCode, string Excepcion = null,  string message = null)
        {
            error_msg = message==null?EnumHelper.GetDescription(statusCode): message;
            error_code = (int)(statusCode);
            success = false;
            innerExcepcion = Excepcion;


        }


    }
    public enum Error
    {
        [Description("Bad Request")]
        BADREQUEST = 400,
        [Description("Not authorized")]
        NOTAUTHORIZED = 401,
        [Description("Record not found")]
        RECORDNOTFOUND = 404,
        [Description("Server Error")]
        SERVERERROR = 500,
        [Description("Confirme su correo electrónico en su buzón de correo")]
        EMAILCONFIRM =-2,
        [Description("Complete su dirección fiscal.")]
        ADRESSINCOMPLETE = -2,
        [Description("Has alcanzado el limite mensual de facturas. Puedes obtener un plan con mayor número de facturas o esperar hasta el próximo mes.")]
        LIMITREACHED =-3,
        [Description("Generando PDF")]
        BUILDINGPDF = -4
    }
}