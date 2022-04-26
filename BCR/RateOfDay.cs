using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BCR
{
    public class RateOfDay
    {
        const string tcIndicador = "318";
        const string tcNombre = "AsadaCloud S.A dviqquez@asadacloud.com";
        const string tnSubNiveles = "N";

        public RateOfDay()
        {

        }
        public List<ResultRateDto> GetDayRate(DateTime fechai, DateTime fechaf)
        {
            List<ResultRateDto> rate = new List<ResultRateDto>();
            DateTime tempDate = DateTime.Now;
            string tcFechaInicio = fechai.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            string tcFechaFinal = fechaf.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            ResultRateDto result;
            
            try
            {
                WSIndicadores.wsIndicadoresEconomicosSoapClient indicador = new WSIndicadores.wsIndicadoresEconomicosSoapClient();

                DataSet list = indicador.ObtenerIndicadoresEconomicos(tcIndicador, tcFechaInicio, tcFechaFinal, tcNombre, tnSubNiveles);
                //DataTable dataTable = list.Tables[0];

                foreach (DataRow data in list.Tables[0].Rows)
                {
                    string valor = data["NUM_VALOR"].ToString();
                    result = new ResultRateDto
                    {
                        Fecha = DateTime.Parse(data["DES_FECHA"].ToString()),
                        rate = Decimal.Parse(String.Format("{0:##,#}", valor))
                    };
                    rate.Add(result);
                }

            }
            catch (Exception)
            {
                TimeSpan ts = fechaf - fechai;
                int differenceInDias = ts.Days;
                for (int i = 0; i <= differenceInDias; i++)
                {
                    result = new ResultRateDto
                    {
                        Fecha = fechai.AddDays(i),
                        rate = 1
                    };
                    rate.Add(result);
                }
            }
            
            return rate;
        }

        public ResultRateDto RateDate(DateTime fechai)
        {
            string tcFechaInicio = fechai.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            ResultRateDto rate;
            try
            {
                WSIndicadores.wsIndicadoresEconomicosSoapClient indicador = new WSIndicadores.wsIndicadoresEconomicosSoapClient();
                XmlDocument xmlDoc = new XmlDocument();
                var xmlString = indicador.ObtenerIndicadoresEconomicosXML(tcIndicador, tcFechaInicio, tcFechaInicio, tcNombre, tnSubNiveles);
                xmlDoc.LoadXml(xmlString);

                string tasaXML = xmlDoc.SelectSingleNode("//NUM_VALOR").InnerText;
                decimal tasa = Decimal.Parse(tasaXML);

                rate = new ResultRateDto()
                {
                    Fecha = DateTime.Parse(xmlDoc.SelectSingleNode("//DES_FECHA").InnerText),
                    rate = decimal.Parse(tasaXML, CultureInfo.InvariantCulture)
                };
            }
            catch (Exception)
            {
                rate = new ResultRateDto()
                {
                    Fecha = fechai,
                    rate = 1
                };
            }

            return rate;
        }
    }
}
