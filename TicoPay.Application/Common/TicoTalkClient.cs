using Abp.Dependency;
using Castle.Core.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices.Dto;
using TicoPay.Invoices.XSD;

namespace TicoPay.Common
{
    public class TicoTalkClient
    {
        public const string DefaultTenancyName = "ticopay";

        public bool IsAuthenticated
        {
            get
            {
                return !string.IsNullOrWhiteSpace(AccessToken);
            }
        }

        public string AccessToken { get; private set; }

        private TicoTalkClient()
        {
        }

        public static TicoTalkClient GetAuthenticatedClient(int? tenantId = null, string userNameOrEmailAddress = null, string password = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userNameOrEmailAddress))
                    userNameOrEmailAddress = ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"];

                if (string.IsNullOrWhiteSpace(password))
                    password = ConfigurationManager.AppSettings["TicoTalk:Password"];

                string apiUri = ConfigurationManager.AppSettings["TicoTalk:ApiUri"];
                Dictionary<string, string> extra = new Dictionary<string, string>();
                if (tenantId != null)
                {
                    extra.Add("Abp.TenantId", tenantId.ToString());
                }
                ApiResponse<AuthorizeResponse> authorizeResponse = HttpClientHelper.Authenticate<AuthorizeResponse>(apiUri, $"api/TokenAuth/Authenticate", userNameOrEmailAddress, password);
                if (authorizeResponse.Success)
                {
                    return new TicoTalkClient
                    {
                        AccessToken = authorizeResponse.Result.AccessToken
                    };
                }
            }
            catch (Exception ex)
            {
                ILogger logger = IocManager.Instance.Resolve<ILogger>();
                if (logger != null)
                {
                    logger.Fatal($"Get TicoTalk Authenticated Client", ex);
                }
            }
            return new TicoTalkClient();
        }

        public void SendSms(string identificationNumber, string clientPhoneNumber, string smsFacturaACobroMessage)
        {
            SendSms(DefaultTenancyName, identificationNumber, clientPhoneNumber, smsFacturaACobroMessage);
        }

        public void SendSms(string tenancyName, string identificationNumber, string clientPhoneNumber, string smsFacturaACobroMessage)
        {
            if (IsAuthenticated && !string.IsNullOrWhiteSpace(clientPhoneNumber))
            {
                try
                {
                    string apiUri = ConfigurationManager.AppSettings["TicoTalk:ApiUri"];
                    string uri = $"api/sms/send";
                    var response = HttpClientHelper.PostAsJsonAsync<object, object>(apiUri, uri, AccessToken, new
                    {
                        TenantName = tenancyName,
                        SenderIdentificationNumber = identificationNumber,
                        PhoneNumber = clientPhoneNumber.Replace("-", "") ?? string.Empty,
                        Message = smsFacturaACobroMessage ?? string.Empty
                    }).Result;

                }
                catch (Exception ex)
                {
                    ILogger logger = IocManager.Instance.Resolve<ILogger>();
                    if (logger != null)
                    {
                        logger.Fatal($"Send SMS to {clientPhoneNumber}", ex);
                    }
                }
            }
        }

        public void CreateTenant(string name, string identificationNumber, IdentificacionType identificacionType, string adminEmailAddress, string adminPassword)
        {
            if (IsAuthenticated)
            {
                try
                {
                    string apiUri = ConfigurationManager.AppSettings["TicoTalk:ApiUri"];
                    string uri = $"api/services/app/Tenant/Create";
                    var response = HttpClientHelper.PostAsJsonAsync<object, object>(apiUri, uri, AccessToken, new
                    {
                        TenancyName = DefaultTenancyName,
                        Name = name,
                        IdentificationNumber = identificationNumber,
                        IdentificationType = identificacionType,
                        AdminEmailAddress = adminEmailAddress,
                        AdminPassword = adminPassword
                    }).Result;
                }
                catch (Exception ex)
                {
                    ILogger logger = IocManager.Instance.Resolve<ILogger>();
                    if (logger != null)
                    {
                        logger.Fatal($"Create Tenant", ex);
                    }
                }
            }
        }

        public async Task<TicoTalkTenantDto> GetTenant(string tenancyName)
        {
            try
            {
                string apiUri = ConfigurationManager.AppSettings["TicoTalk:ApiUri"];
                string uri = $"api/services/app/Tenant/GetByTenancyName?tenancyName={tenancyName}";
                ApiResponse<TicoTalkTenantDto> response = await HttpClientHelper.GenericGetAsync<ApiResponse<TicoTalkTenantDto>>(apiUri, uri, AccessToken);
                if (response.Success)
                {
                    return response.Result;
                }
            }
            catch (Exception ex)
            {
                ILogger logger = IocManager.Instance.Resolve<ILogger>();
                if (logger != null)
                {
                    logger.Fatal($"Get TicoTalk Authenticated Client", ex);
                }
            }
            return null;
        }

        public async Task CreateSmsSender(string identificationNumber, IdentificacionTypeTipo identificacionType, string name, string currency)
        {
            if (IsAuthenticated)
            {
                try
                {
                    var tenant = await GetTenant(DefaultTenancyName);
                    if (tenant == null)
                    {
                        return;
                    }

                    string apiUri = ConfigurationManager.AppSettings["TicoTalk:ApiUri"];
                    string uri = $"api/services/app/SmsSender/Create";
                    var response = await HttpClientHelper.GenericPostAsJsonAsync<object, object>(apiUri, uri, AccessToken, new
                    {
                        TenantId = tenant.Id,
                        IdentificationNumber = identificationNumber,
                        IdentificationType = (int)identificacionType,
                        Name = name,
                        Currency = currency
                    });
                }
                catch (Exception ex)
                {
                    ILogger logger = IocManager.Instance.Resolve<ILogger>();
                    if (logger != null)
                    {
                        logger.Fatal($"Create SMS Sender", ex);
                    }
                }
            }
        }

        public async Task<SmsSenderDto> GetSmsSender(string smsSenderName)
        {
            try
            {
                var tenant = await GetTenant(DefaultTenancyName);
                if (tenant == null)
                {
                    return null;
                }

                string apiUri = ConfigurationManager.AppSettings["TicoTalk:ApiUri"];
                string uri = $"api/services/app/SmsSender/GetByName?tenantId={tenant.Id}&smsSenderName={smsSenderName}";
                ApiResponse<SmsSenderDto> response = await HttpClientHelper.GenericGetAsync<ApiResponse<SmsSenderDto>>(apiUri, uri, AccessToken);
                if (response.Success)
                {
                    return response.Result;
                }
            }
            catch (Exception ex)
            {
                ILogger logger = IocManager.Instance.Resolve<ILogger>();
                if (logger != null)
                {
                    logger.Fatal($"Get TicoTalk Authenticated Client", ex);
                }
            }
            return null;
        }

        public async Task UpdateCostoSms(string identificationNumber, decimal costoSms)
        {
            try
            {
                var tenant = await GetTenant(DefaultTenancyName);
                if (tenant == null)
                {
                    return;
                }

                string apiUri = ConfigurationManager.AppSettings["TicoTalk:ApiUri"];
                string uri = $"api/sms/updateSmsCost";
                var response = await HttpClientHelper.GenericPostAsJsonAsync<object, object>(apiUri, uri, AccessToken, new
                {
                    TenantId = tenant.Id,
                    IdentificationNumber = identificationNumber,
                    SmsCost = costoSms
                });
            }
            catch (Exception ex)
            {
                ILogger logger = IocManager.Instance.Resolve<ILogger>();
                if (logger != null)
                {
                    logger.Fatal($"Updating SMS Cost.", ex);
                }
            }
        }

        public async Task<SmsDebtDto> GetSmsDebt(DateTime fromDate, DateTime toDate, string identificationNumber)
        {
            SmsDebtDto dto = new SmsDebtDto();
            if (IsAuthenticated)
            {
                try
                {
                    string apiUri = ConfigurationManager.AppSettings["TicoTalk:ApiUri"];
                    string uri = $"api/Sms/getsmsdebt";
                    var response = await HttpClientHelper.GenericPostAsJsonAsync<object, ApiResponse<SmsDebtDto>>(apiUri, uri, AccessToken, new
                    {
                        TenancyName = DefaultTenancyName,
                        IdentificationNumber = identificationNumber,
                        FromDate = fromDate,
                        ToDate = toDate
                    });
                    if (response.Success)
                    {
                        dto = response.Result;
                    }
                }
                catch (Exception ex)
                {
                    ILogger logger = IocManager.Instance.Resolve<ILogger>();
                    if (logger != null)
                    {
                        logger.Fatal($"Create SMS Sender", ex);
                    }
                }
            }
            return dto;
        }
    }

    [JsonObject("result")]
    public class AuthorizeResponse
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("encryptedAccessToken")]
        public string EncryptedAccessToken { get; set; }

        [JsonProperty("expireInSeconds")]
        public int ExpireInSeconds { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }
    }

    public class TicoTalkTenantDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }

    public class SmsSenderDto
    {
        [JsonProperty("identificationNumber")]
        public string IdentificationNumber { get; set; }
        [JsonProperty("identificationType")]
        public int IdentificationType { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("tenantId")]
        public int TenantId { get; set; }
    }
}
