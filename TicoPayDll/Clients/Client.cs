using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicoPayDll.Clients
{
    // Propiedades iguales a las de ClientDto
    public class Client
    {
        [JsonProperty]
        public Guid Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string LastName { get; set; }

        [JsonProperty]
        public IdentificacionTypeTipo IdentificationType { get; set; }

        [JsonProperty]
        public string Identification { get; set; }

        [JsonProperty]
        public string IdentificacionExtranjero { get; set; }

        [JsonProperty]
        public string NameComercial { get; set; }

        [JsonIgnore]
        public int TipoId { get; set; }

        [JsonProperty]
        public long Code { get; set; }

        [JsonIgnore]
        public Guid? GroupId { get; set; }

        [JsonProperty]
        public DateTime? Birthday { get; set; }

        [JsonProperty]
        public Gender Gender { get; set; }

        [JsonProperty]
        public string PhoneNumber { get; set; }

        [JsonProperty]
        public string MobilNumber { get; set; }

        [JsonProperty]
        public string Fax { get; set; }

        [JsonProperty]
        public string Email { get; set; }

        [JsonProperty]
        public string WebSite { get; set; }

        [JsonProperty]
        public int BarrioId { get; set; }

        [JsonProperty]
        public string PostalCode { get; set; }

        [JsonIgnore]
        public string IsoCountry { get; set; }

        [JsonProperty]
        public string Address { get; set; }

        [JsonIgnore]
        public long Latitud { get; set; }

        [JsonIgnore]
        public long Longitud { get; set; }

        [JsonProperty]
        public string ContactName { get; set; }

        [JsonProperty]
        public string ContactMobilNumber { get; set; }

        [JsonProperty]
        public string ContactPhoneNumber { get; set; }

        [JsonProperty]
        public string ContactEmail { get; set; }

        [JsonProperty]
        public decimal Balance { get; set; }

        [JsonProperty]
        public string Note { get; set; }

        [JsonProperty]
        public int? CreditDays { get; set; }        
    }

    public enum IdentificacionTypeTipo
    {
        Cedula_Fisica,
        Cedula_Juridica,
        DIMEX,
        NITE,
        NoAsignada,
    }

    public enum Gender
    {
        Female,
        Male,
        Other
    }
}
