using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Printers
{
    public class Printer
    {
        protected IModelPrinter _model;

        public const string PRINTER_ID = "-PID";
        public const string INSTALLED_PRINTER_NAME = "-InstalledPrinterName";
        public const string NET_PRINTER_HOST = "-NetPrinterHost";   
        public const string NET_PRINTER_PORT = "-NetPrinterPort";
        public const string PARALLEL_PORT = "-ParallelPort";
        public const string SERIAL_PORT = "-SerialPort";
        public const string SERIAL_PORT_BAUDS = "-SerialPortBauds";
        public const string SERIAL_PORT_DATA_BITS = "-SerialPortDataBits";
        public const string SERIAL_PORT_STOP_BITS = "-SerialPortStopBits";
        public const string SERIAL_PORT_PARITY = "-SerialPortParity";
        public const string SERIAL_PORT_FLOW_CONTROL = "-SerialPortFlowControl";
        public const string PRINTER_COMMANDS = "-PrinterCommands";

        public int MaxLineDetails { get; set; }

        public Printer(PrinterTypes model)
        {
            switch (model)
            {
                case PrinterTypes.MatrizPunto:
                    _model = new Matriz();
                    MaxLineDetails = Matriz.MaxLineDetails;
                    break;
                case PrinterTypes.Epson:
                    _model = new Epson();
                    MaxLineDetails = Epson.maxLineDetails;
                    break;
                case PrinterTypes.Epson_TMT20II:
                    _model = new Epson_TMT20II();
                    MaxLineDetails = Epson_TMT20II.maxLineDetails;
                    break;
                case PrinterTypes.Zebra_iMZ320:
                    _model = new Zebra_iMZ320();
                    MaxLineDetails = Zebra_iMZ320.maxLineDetails;
                    break;
                case PrinterTypes.Bematech_LR200:
                    _model = new Bematech_LR200();
                    MaxLineDetails = Bematech_LR200.maxLineDetails;
                    break;
                case PrinterTypes.BTP_R880NP:
                    _model = new BTP_R880NP();
                    MaxLineDetails = BTP_R880NP.maxLineDetails;
                    break;
                case PrinterTypes.Zebra_ZQ320:
                    _model = new Zebra_ZQ320();
                    MaxLineDetails = Zebra_ZQ320.maxLineDetails;
                    break;
                case PrinterTypes.XP_58iih:
                    _model = new XP_58iih();
                    MaxLineDetails = XP_58iih.maxLineDetails;
                    break;
                case PrinterTypes.N3Star_PPT300BT:
                    _model = new Bematech_LR200();
                    MaxLineDetails = Bematech_LR200.maxLineDetails;
                    break;
                case PrinterTypes.POS_5890c:
                    _model = new XP_58iih();
                    MaxLineDetails = XP_58iih.maxLineDetails;
                    break;
                case PrinterTypes.POS_5805DD:
                    _model = new POS_5805DD();
                    MaxLineDetails = POS_5805DD.maxLineDetails;
                    break;
                default:
                    _model = new Epson();
                    MaxLineDetails = Epson.maxLineDetails;
                    break;
            }
           
        }

        public string print(DocumentPrint doc)
        {
            return _model.print(doc);
        }

        
        public HttpApplicationStateBase ClientPrinterSettings(HttpApplicationStateBase app, string sid,
                                            string printerCommands,
                                            string installedPrinterName=null,
                                            string netPrinterHost = null,
                                            string netPrinterPort = null,
                                            string parallelPort = null,
                                            string serialPort = null,
                                            string serialPortBauds = null,
                                            string serialPortDataBits = null,
                                            string serialPortStopBits = null,
                                            string serialPortParity = null,
                                            string serialPortFlowControl = null)
        {
            try
            {
                app[sid + PRINTER_ID] = 0;
                //save the printer commands specified by the user
                app[sid + PRINTER_COMMANDS] = printerCommands;

                return app;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
