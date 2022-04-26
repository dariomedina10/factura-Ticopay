using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Invoices
{
    public class AddInvoice
    {
        public EncabezadoInvoice encabezado { get; set; }
        public DatosPrincipalesInvoice datosprincipales { get; set; }
        public ListaProductosInvoice[] listaproductos { get; set; }
        public FormaCobroInvoice formacobro { get; set; }
        public LiquidacionImpuestos[] liquidimpuestos { get; set; }

        public AddInvoice(int cantidadDeProductos, int cobroCaja, int formaCobro, int cuentasCobrar,int cantidadDeActiculosConImpuesto)
        {
            encabezado = new EncabezadoInvoice();
            datosprincipales = new DatosPrincipalesInvoice();
            listaproductos = new ListaProductosInvoice[cantidadDeProductos];
            formacobro = new FormaCobroInvoice(cobroCaja,formaCobro,cuentasCobrar);
            liquidimpuestos = new LiquidacionImpuestos[cantidadDeActiculosConImpuesto];
        }
    }

    public class EncabezadoInvoice
    {
        public string iemp { get; set; }
        public string inumoper { get; set; }
        public string itdsop { get; set; }
        public string fsoport { get; set; }
        public string iclasifop { get; set; }
        public string imoneda { get; set; }
        public string iprocess { get; set; }
        public string banulada { get; set; }
        public string inumsop { get; set; }
        public string snumsop { get; set; }
        public string tdetalle { get; set; }
        public string svaloradic1 { get; set; }
        public string svaloradic2 { get; set; }
        public string svaloradic3 { get; set; }

        public EncabezadoInvoice()
        {
            iemp = "";
            inumoper = "";
            itdsop = "";
            fsoport = "";
            iclasifop = "";
            imoneda = "";
            iprocess = "";
            banulada = "";
            inumsop = "0";
            snumsop = "<AUTO>";
            tdetalle = "";
            svaloradic1 = "";
            svaloradic2 = "";
            svaloradic3 = "";                                      }
    }

    public class DatosPrincipalesInvoice
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

        public DatosPrincipalesInvoice()
        {
            init = "";
            iinventario = "";
            initvendedor = "";
            initvendedor2 = "";
            busarotramoneda = "";
            sobserv = "";
            imoneda = "";
            mtasacambio = "";
            bshowsupportinfo = "";
            bregvrunit = "";
            bshowcntfields = "";
            qporcdescuento = "";
            bregvrtotal = "";
            ilistaprecios = "";
            blistaconiva = "";
            bfactporpedido = "";
            bimprimirdescfinan = "";
            bautocalcularcomisiones = "";
            sperfilyreferencias = "";
            icuentaporfacturar = "";
            iws = "";
            qprecisionprecio = "";
            qprecisionliquid = "";
            isucursalcliente = "";
            fhultcfdigenerado = "";
            mcambio = "";
            mavance = "";
            ntercero = "";
            qregproductos = "";
            qregreferencias = "";
            qregingresos = "";
            qregconcdescuento = "";
            qregcomisiones = "";
            qregseriesproductos = "";
        }
    }

    public class ListaProductosInvoice
    {
        public string icc { get; set; }
        public string iinventario { get; set; }
        public string irecurso { get; set; }
        public string nrecurso { get; set; }
        public string nunidad { get; set; }
        public string itiporec { get; set; }
        public string qproducto { get; set; }
        public string mprecio { get; set; }
        public string qporcdescuento { get; set; }
        public string mvrtotal { get; set; }
        public string qporciva { get; set; }
        public string fhini { get; set; }
        public string fhfin { get; set; }
        public string dato1 { get; set; }
        public string dato2 { get; set; }
        public string dato3 { get; set; }
        public string dato4 { get; set; }
        public string dato5 { get; set; }
        public string dato6 { get; set; }
        public string valor1 { get; set; }
        public string valor2 { get; set; }
        public string valor3 { get; set; }
        public string valor4 { get; set; }
        public string qproducto2 { get; set; }

        public ListaProductosInvoice()
        {
            icc = "";
            iinventario = "";
            irecurso = "";
            nrecurso = "";
            nunidad = "";
            itiporec = "";
            qproducto = "";
            mprecio = "";
            qporcdescuento = "";
            mvrtotal = "";
            qporciva = "";
            fhini = "";
            fhfin = "";
            dato1 = "";
            dato2 = "";
            dato3 = "";
            dato4 = "";
            dato5 = "";
            dato6 = "";
            valor1 = "";
            valor2 = "";
            valor3 = "";
            valor4 = "";
            qproducto2 = "";
        }
    }

    public class FormaCobroInvoice
    {
        public string mtotalreg { get; set; }
        public string mtotalpago { get; set; }
        public CobroCajaCuenta[] fcobrocaja { get; set; }
        public CobroBancoCuenta[] fcobrobanco { get; set; }
        public CuentaCobrar[] fcobrocxc { get; set; }

        public FormaCobroInvoice(int cobroCaja, int formaCobro, int cuentasCobrar)
        {
            mtotalpago = "";
            mtotalreg = "";
            fcobrocaja = new CobroCajaCuenta[cobroCaja];
            fcobrobanco = new CobroBancoCuenta[formaCobro];
            fcobrocxc = new CuentaCobrar[cuentasCobrar];
        }
    }

    public class CobroCajaCuenta
    {
        public string id { get; set; }// Código de la formo de cobro de caja.
        public string icuenta { get; set; }// Código de la cuenta del Plan Único de Agente de Servicios Web de ContaPyme Cuentas que se está afectando con el movimiento.
        public string init { get; set; }// Código del tercero para el movimiento de caja, si la cuenta lo exige.
        public string icc { get; set; }// Si la cuenta de caja exige CC, aquí se almacena el código del centro de costos.
        public string itipotransaccion { get; set; }// Tipo de concepto de conciliación bancaria, si la cuenta es de conciliación.
        public string iflujoefec { get; set; }// Almacena el código del concepto de flujo de efectivo al que hace referencia el movimiento.
        public string ilineamov { get; set; }// Consecutivo del movimiento contable de conciliación.
        public string mvalor { get; set; }// Valor recibido en efectivo
        public string mvrotramoneda { get; set; }// Valor recibido equivalente en moneda extranjera.
        public string beditvrotramoneda { get; set; }

        public CobroCajaCuenta()
        {
            id = "";
            icuenta = "";
            init = "";
            icc = "";
            itipotransaccion = "";
            iflujoefec = "";
            ilineamov = "";
            mvalor = "";
            mvrotramoneda = "";
            beditvrotramoneda = "";
        }
    }

    public class CobroBancoCuenta
    {
        public string id { get; set; } // Código de la formo de cobro de banco.
        public string icuenta { get; set; }// Código de la cuenta del Plan Único de Cuentas que se está afectando con el movimiento.
        public string init { get; set; }// Código del tercero para el movimiento de banco, si la cuenta lo exige.
        public string icc { get; set; }// Si la cuenta de banco exige CC, aquí se almacena el código del centro de costos.
        public string itransaccion { get; set; }// Número de la transacción bancaria (consignación, cheque, etc).
        public string itipotransaccion { get; set; }// Tipo de concepto de conciliación bancaria, si la cuenta es de conciliación.
        public string ftransaccion { get; set; }// Fecha de la transacción bancaria o del cheque.
        public string iflujoefec { get; set; }// Almacena el código del concepto de flujo de efectivo al que hace referencia el movimiento.
        public string ilineamov { get; set; }// Consecutivo del movimiento contable de conciliación.
        public string mvalor { get; set; }// Valor recibido en efectivo.
        public string mvrotramoneda { get; set; }// Valor recibido equivalente en moneda extranjera.
        public string beditvrotramoneda { get; set; }

        public CobroBancoCuenta()
        {
            id = "";
            icuenta = "";
            init = "";
            icc = "";
            itransaccion = "";
            itipotransaccion = "";
            ftransaccion = "";
            iflujoefec = "";
            ilineamov = "";
            mvalor = "";
            mvrotramoneda = "";
            beditvrotramoneda = "";
        }
    }

    public class CuentaCobrar
    {
        public string id { get; set; }// Código de la formo de cobro de CxC.
        public string init { get; set; }//Código del tercero al que se creará la CxC.
        public string icuenta { get; set; }//Código de la cuenta del Plan Único de Cuentas que se está afectando con el movimiento.
        public string qdiascxc { get; set; }//Días de plazo para la CxC.
        public string qdiasvencim { get; set; }//Días para vencimiento (no se usa por ahora).
        public string icc { get; set; }//Si la cuenta de cartera exige CC, aquí se almacena el código del centro de costos.
        public string nconcepto { get; set; }//Descripción de la CxC.
        public CuentaCobrarcuota[] slistcuotas { get; set; }// Arreglo de objetos, cada objeto contiene la información de cada cuota entre las cuales se divide la CxC        
        public string mvalor { get; set; }//Valor con el cual se creará la CxC.
        public string mvrotramoneda { get; set; }//Valor equivalente en moneda extranjera con el cual se creará la CxC
        public string bconceptochanged { get; set; }//Indica si “nconcepto” fue especificado por el usuario o no.
        public string beditvrotramoneda { get; set; }

        public CuentaCobrar()
        {
            id = "";
            init = "";
            icuenta = "";
            qdiascxc = "";
            qdiasvencim = "";
            icc = "";
            nconcepto = "";
            mvalor = "";
            mvrotramoneda = "";
            bconceptochanged = "";
            beditvrotramoneda = "";
        }
    }

    public class CuentaCobrarcuota
    {
        public string icuota { get; set; } // número de la cuota
        public string fpagocuota { get; set; }// fecha de pago de la cuota
        public string mcuota { get; set; }// valor de la cuota
        public string mextranjera { get; set; }// valor de la cuota en moneda extranjera

        public CuentaCobrarcuota()
        {
            icuota = "";
            fpagocuota = "";
            mcuota = "";
            mextranjera = "";
        }
    }

    public class LiquidacionImpuestos
    {
        public string iconcepto { get; set; }
        public string nconcepto { get; set; }
        public string isigno { get; set; }
        public string mvalorbase { get; set; }
        public string qpercent { get; set; }
        public string mvalor { get; set; }
        public string bautocalc { get; set; }
        public string icuenta { get; set; }
        public string initcxx { get; set; }
        public string iasiento { get; set; }
        public string mvrbasemin { get; set; }
        public string bdefecto { get; set; }
        public string sobserv { get; set; }
        public string custom { get; set; }
        public string itdsop { get; set; }
        public string inumsop { get; set; }
        public string init { get; set; }
        public string fpago { get; set; }
        public string icc { get; set; }
        public string iaccion { get; set; }
        public string icuentaiesinsigno { get; set; }
        public string itablapago { get; set; }

        public LiquidacionImpuestos()
        {
            iconcepto = "";
            nconcepto = "";
            isigno = "";
            mvalorbase = "";
            qpercent = "";
            mvalor = "";
            bautocalc = "";
            icuenta = "";
            initcxx = "";
            iasiento = "";
            mvrbasemin = "";
            bdefecto = "";
            sobserv = "";
            custom = "";
            itdsop = "";
            inumsop = "";
            init = "";
            fpago = "";
            icc = "";
            iaccion = "";
            icuentaiesinsigno = "";
            itablapago = "";
        }
    }
}
