using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicoPayDll.Taxes
{
    public class Tax
    {
        [JsonProperty]
        public Guid Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public decimal Rate { get; set; }

        [JsonProperty]
        public ImpuestoTypeCodigo TaxTypes { get; set; }
    }

    public enum ImpuestoTypeCodigo
    {
        Impuesto_General_Sobre_Ventas,
        Impuesto_Selectivo_Consumo,
        Impuesto_Unico_Combustibles,
        Impuesto_Bebidas_Alcoholicas,
        Impuesto_Bebidas_Envasadas_sin_Alcohol_y_Jabones_de_tocador,
        Impuesto_Productos_Tabaco,
        Servicio,
        Exento,
        Impuesto_General_sobre_Ventas_Diplomaticos,
        Impuesto_General_sobre_Ventas_Compras_Autorizadas,
        Impuesto_General_sobre_Ventas_Instituciones_Publicas_y_Organismos,
        Impuesto_Selectivo_Consumo_Compras_Autorizadas,
        Otros,
    }
}
