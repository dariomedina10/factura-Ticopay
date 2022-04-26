using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IsoING1_Addin
{
    public class IsoING1_Addin
    {
        public static int AFTERPROCESS(string JSonEntorno, string JSonOprData, string MsgType,ref string Msg, string Params, string Config)
        {
            EventLog eventLog = new EventLog("Application");            
            eventLog.Source = "Application";
            eventLog.WriteEntry("ContaPyme Ejecuto Método", EventLogEntryType.Information);
            
            string idDocumento = "";
            string idCliente = "";
            string idEmpresa = "";
            try
            {
                ContaPymeJson dataContaPyme = JsonConvert.DeserializeObject<ContaPymeJson>(JSonOprData);
                idDocumento = dataContaPyme.encabezado.inumoper;
                idEmpresa = dataContaPyme.encabezado.iemp;
                idCliente = dataContaPyme.datosprincipales.init;
                eventLog.WriteEntry("ContaPyme Json Deserializado" + idDocumento, EventLogEntryType.Information);
            }
            catch
            {
                eventLog.WriteEntry("ContaPyme Json Error" + idDocumento, EventLogEntryType.Error);
                Msg = "Error al leer el Json";
                return 2;
            }
            
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=07;" + idDocumento + ";" + idCliente + ";" + idEmpresa +";";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envió petición
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                if (respuesta == "1")
                {
                    return 0;
                }
                else
                {
                    Msg = "Error al crear la factura";
                    return 1;
                }
            }
            catch
            {
                return 1;
            }
        }
    }



    public class ContaPymeJson
    {
        public EncabezadoJson encabezado { get; set; }
        public DatosPrincipalesJson datosprincipales { get; set; }
    }

    public class DatosPrincipalesJson
    {
        public string init { get; set; }
        public string iinventario { get; set; }
        public string initvendedor { get; set; }
        public string initvendedor2 { get; set; }
        public string busarotramoneda { get; set; }
        public string sobserv { get; set; }
        public string imoneda { get; set; }
        public string mtasacambio { get; set; }
        public string bshowsupportinfo { get; set; }
        public string bregvrunit { get; set; }
        public string bshowcntfields { get; set; }
        public string qporcdescuento { get; set; }
        public string bregvrtotal { get; set; }
        public string ilistaprecios { get; set; }
        public string blistaconiva { get; set; }
        public string bfactporpedido { get; set; }
        public string bimprimirdescfinan { get; set; }
        public string bautocalcularcomisiones { get; set; }
        public string sperfilyreferencias { get; set; }
        public string icuentaporfacturar { get; set; }
        public string iws { get; set; }
        public string qprecisionprecio { get; set; }
        public string qprecisionliquid { get; set; }
        public string isucursalcliente { get; set; }
        public string fhultcfdigenerado { get; set; }
        public string mcambio { get; set; }
        public string mavance { get; set; }
        public string ntercero { get; set; }
        public string qregproductos { get; set; }
        public string qregreferencias { get; set; }
        public string qregingresos { get; set; }
        public string qregconcdescuento { get; set; }
        public string qregcomisiones { get; set; }
        public string qregseriesproductos { get; set; }
    }

    public class EncabezadoJson
    {
        public string iemp { get; set; }
        public string inumoper { get; set; }
        public string itdsop { get; set; }
        public string fsoport { get; set; }
        public string iclasifop { get; set; }
        public string imoneda{ get; set; }
        public string iprocess { get; set; }
        public string banulada { get; set; }
        public string inumsop { get; set; }
        public string snumsop { get; set; }
        public string tdetalle { get; set; }
        public string svaloradic1 { get; set; }
        public string svaloradic2 { get; set; }
        public string svaloradic3 { get; set; }
        public string svaloradic4 { get; set; }
        public string svaloradic5 { get; set; }
        public string svaloradic6 { get; set; }
        public string svaloradic7 { get; set; }
        public string svaloradic8 { get; set; }
        public string svaloradic9 { get; set; }
        public string svaloradic10 { get; set; }
        public string svaloradic11 { get; set; }
        public string svaloradic12 { get; set; }
    }
}
