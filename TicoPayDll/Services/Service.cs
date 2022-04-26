using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TicoPayDll.Taxes;

namespace TicoPayDll.Services
{
    public class Service
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public decimal Price { get; set; }

        [JsonProperty]
        public Guid? TaxId { get; set; }

        [JsonProperty]
        public virtual Tax Tax { get; set; }

        [JsonProperty]
        public UnidadMedidaType UnitMeasurement { get; set; }

        [JsonProperty]
        public string UnitMeasurementOthers { get; set; }

        [JsonProperty]
        public bool IsRecurrent { get; set; }

        [JsonProperty]
        public string CronExpression { get; set; }

        [JsonProperty]
        public decimal Quantity { get; set; }

        [JsonProperty]
        public decimal DiscountPercentage { get; set; }

        [JsonProperty]
        public string Type { get; set; } = "Service";
    }

    public enum UnidadMedidaType
    {

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Sp")]
        Servicios_Profesionales,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("m")]
        Metro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("kg")]
        Kilogramo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("s")]
        Segundo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("A")]
        Ampere,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("K")]
        Kelvin,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("mol")]
        Mol,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("cd")]
        Candela,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("m²")]
        Metro_Cuadrado,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("m³")]
        Metro_Cubico,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("m/s")]
        Metro_por_Segundo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("m/s²")]
        Metro_por_Segundo_Cuadrado,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("1/m")]
        Uno_por_Metro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("kg/m³")]
        kilogramo_por_Metro_Cubico,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("A/m²")]
        Ampere_por_Metro_Cuadrado,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("A/m")]
        Ampere_por_Metro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("mol/m³")]
        Mol_por_Metro_Cubico,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("cd/m²")]
        Candela_por_Metro_Cuadrado,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Uno,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("rad")]
        Radian,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("sr")]
        Estereorradian,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Hz")]
        Hertz,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("N")]
        Newton,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Pa")]
        Pascal,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("J")]
        Joule,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("W")]
        Watt,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("C")]
        Coulomb,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("V")]
        Volt,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("F")]
        Farad,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Ω")]
        Ohm,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("S")]
        Siemens,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Wb")]
        Weber,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("T")]
        Tesla,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("H")]
        Henry,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("°C")]
        Grado_Celsius,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("lm")]
        Lumen,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("lx")]
        Lux,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Bq")]
        Becquerel,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Gy")]
        Gray,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Sv")]
        Sievert,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("kat")]
        Katal,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Pa·s")]
        Pascal_Segundo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("N·m")]
        Newton_Metro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("N/m")]
        Newton_por_Metro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("rad/s")]
        Radian_por_Segundo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("rad/s²")]
        Radian_por_Segund_Cuadrado,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("W/m²")]
        Watt_por_Metro_Cuadrado,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("J/K")]
        Joule_por_Kelvin,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("J/(kg·K)")]
        Joule_por_Kilogramo_Kelvin,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("J/kg")]
        Joule_por_Kilogramo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("W/(m·K)")]
        Watt_por_Metro_Kevin,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("J/m³")]
        Joule_por_Metro_Cubico,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("V/m")]
        Volt_por_Metro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("C/m³")]
        Coulomb_por_Metro_Cubico,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("C/m²")]
        Coulomb_por_Metro_Cuadrado,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("F/m")]
        Farad_por_Metro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("H/m")]
        Henry_por_Metro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("J/mol")]
        Joule_por_Mol,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("J/(mol·K)")]
        Joule_por_Mol_Kelvin,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("C/kg")]
        Coulomb_por_Kilogramo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Gy/s")]
        Gray_por_Segundo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("W/sr")]
        Watt_por_Estereorradian,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("W/(m²·sr)")]
        Watt_por_Metro_Cuadrado_Estereorradian,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("kat/m³")]
        Katal_por_Metro_Cubico,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("min")]
        Minuto,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("h")]
        Hora,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("d")]
        Dia,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("º")]
        Grado,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("´")]
        Minuto_,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("´´")]
        Segundo_,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("L")]
        Litro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("t")]
        Tonelada,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Np")]
        Neper,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("B")]
        Bel,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("eV")]
        Electronvolt,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("u")]
        Unidad_de_Masa_Atomica_Unificada,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("ua")]
        Unidad_Astronomica,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Unid")]
        Unidad,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Gal")]
        Galon,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("g")]
        Gramo,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Km")]
        Kilometro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("ln")]
        Pulgada,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("cm")]
        Centimetro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("mL")]
        Mililitro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("mm")]
        Milimetro,

        /// <comentarios/>
        [System.Xml.Serialization.XmlEnumAttribute("Oz")]
        Onzas,

        /// <comentarios/>
        Otros,
    }
}
