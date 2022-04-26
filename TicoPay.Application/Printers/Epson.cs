using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TicoPay.Clients;
using TicoPay.Invoices.XSD;
using TicoPay.Invoices;

namespace TicoPay.Printers
{
    public class Epson : IModelPrinter
    {


        private const string bR = "----------------------------------------\r\n";

        private const int lineLength = 40;
        // ojo lineLength = strLength + intLength si no se hace asi se descuadra
        private const int intLength = 20;
        private const int strLength = 20;

        public const int maxLineDetails = -1;

        public virtual int LineLength => lineLength;

        public virtual int IntLength => intLength;

        public virtual int StrLength => strLength;

        public virtual int MaxLineDetails => maxLineDetails;

        public virtual string BR => bR;

        public Epson()
        {
        }     

        public virtual string BuildHeader(DocumentPrint doc)
        {
            //Clave Numérica: 506060618003101741788001//
            //00001010000003502196437526              //   
            //----------------------------------------//
            //                    FACTURA ELECTRONICA //
            //              Nro: 00100001010000000080 //            
            //----------------------------------------//
            //TENANT NOMBRE COMERCIAL                 //
            //Razon Social: XXXXXXXXXXXXX             //
            //Cedula <tipo>: XXXXXXXXXXXXX            //
            //Tel: 123-00000000 - Fax: 123-000000000  //           
            //Dirección:                              // 
            //Sucursal: Principal                     //
            //Dirección Sucursal: XXXX                //
            //               Fecha emision: 02/11/2017//
            //                           Hora 10:00 am//  
            //----------------------------------------// --> si es una nota se muetra esta seccion
            //Doc. Ref: FACTURA ELECTRONICA
            //Nro Doc.: 00100001010000000075


            string str = string.Empty;           
            try
            {

                str += CommonPrinter.StrTruc( string.Format("Clave: {0}", doc.VoucherKey), LineLength) + CommonPrinter.NEW_LINE;
                str += BR;
                str += CommonPrinter.TXT_STYLE_NORMAL;
                str += CommonPrinter.NEW_LINE;
                str += (CommonPrinter.normalizeStringChars(TicoPay.Application.Helpers.EnumHelper.GetDescription(doc.TypeDocument).ToUpper())).PadLeft(LineLength, ' ')+ CommonPrinter.NEW_LINE;
                str += ("Nro: " + doc.ConsecutiveNumber).PadLeft(LineLength, ' ') + CommonPrinter.NEW_LINE;
                str += CommonPrinter.TXT_STYLE_NORMAL;
                str += BR;
                str += CommonPrinter.TXT_STYLE_NORMAL;
                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(doc.Tenant.BussinesName.ToUpper()), LineLength);
                str += CommonPrinter.StrSplit("Razon social: " + CommonPrinter.normalizeStringChars(doc.Tenant.Name), LineLength);
                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(string.Format("{0}: {1}", doc.Tenant.IdentificationTypeToString(), doc.Tenant.IdentificationNumber )), LineLength);
                var fax = string.IsNullOrWhiteSpace(doc.Tenant.Fax) ? " " : doc.Tenant.Fax;
                var phone= string.IsNullOrWhiteSpace(doc.Tenant.PhoneNumber) ? " " : doc.Tenant.PhoneNumber;
                str += string.Format("Tel: {0} - Fax: {1}", phone.PadRight(12,' '), fax.PadRight(12, ' ')).PadRight(LineLength,' ') + CommonPrinter.NEW_LINE;

                var address = string.Empty;
                if (doc.Tenant.IsAddressShort)
                    address = doc.Tenant.AddressShort;
                else
                    address = doc.Tenant.Barrio.NombreBarrio + " " + doc.Tenant.Barrio.Distrito.NombreDistrito + " " + doc.Tenant.Barrio.Distrito.Canton.NombreCanton + " " + doc.Tenant.Barrio.Distrito.Canton.Provincia.NombreProvincia + " " + doc.Tenant.Address;

                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(string.Format("Direccion: {0}", address)),LineLength) ;                 

                if (doc.BranchOffice != null)
                {
                    str += CommonPrinter.StrSplit("Sucursal: " + CommonPrinter.normalizeStringChars(doc.BranchOffice.Name), LineLength) + CommonPrinter.NEW_LINE;
                    if (!string.IsNullOrEmpty(doc.BranchOffice.Location))
                    {
                        str += CommonPrinter.StrSplit("Direccion Sucursal: " + CommonPrinter.normalizeStringChars(doc.BranchOffice.Location), LineLength) + CommonPrinter.NEW_LINE; 
                    }
                }

                str += ("Fecha: " + doc.DueDate.ToString("dd-MM-yyyy")).PadLeft(LineLength, ' ') + CommonPrinter.NEW_LINE;
                str += ("Hora: " + doc.DueDate.ToShortTimeString()).PadLeft(LineLength, ' ') + CommonPrinter.NEW_LINE;

                if (doc.DocumentRef != null)
                {
                    str += BR;
                    str += (string.Format("Doc. Ref: {0}", CommonPrinter.normalizeStringChars(doc.DocumentRef.TypeDocument))) + CommonPrinter.NEW_LINE;
                    str += (string.Format("Nro Doc.: {0}",  doc.DocumentRef.ConsecutiveNumber)) + CommonPrinter.NEW_LINE;

                }

            }
            catch (Exception ex)
            {
                throw;
            }
            
            return str;
        }

        public virtual string BuildClientInfo(DocumentPrint doc)
        {
            //----------------------------------------//                       
            //Cliente: xxxxxx xxxxxx xxxxxxxxx        //
            //Cedula: XXXXXXXXXXXXX                   //
            //Codigo: XXXXXXXXXXXXX                   //
            //Movil: 123-00000000 - Tel: 123-00000000 //
            //Direccion:                              //            
            //----------------------------------------//
            //Moneda: USD                             //
            //Condicion de venta: Credito             //
            //Fecha de vencimiento: 02-11-2017        //
            //----------------------------------------//                       


            string tipoidentificacion =string.Empty, identificacion = string.Empty;
            string clientName= string.Empty, phoneClient= string.Empty, address = string.Empty, Mobilphone= string.Empty, codigo=string.Empty;
            string str = string.Empty;
            try
            {
                if (doc.Client!=null)
                {
                    tipoidentificacion =  doc.Client.IdentificationTypeToString();
                    clientName = doc.Client.Name + ((doc.Client.IdentificationType!= Invoices.XSD.IdentificacionTypeTipo.Cedula_Juridica && doc.Client.LastName!=null)? " "+ doc.Client.LastName: string.Empty );
                    identificacion = (doc.Client.IdentificationType == Invoices.XSD.IdentificacionTypeTipo.NoAsiganda ? doc.Client.IdentificacionExtranjero : doc.Client.Identification);
                    phoneClient = string.IsNullOrWhiteSpace(doc.Client.PhoneNumber) ? string.Empty : doc.Client.PhoneNumber;
                    codigo = doc.Client.Code.ToString();
                    Mobilphone = (!string.IsNullOrWhiteSpace(doc.Client.MobilNumber)) ? doc.Client.MobilNumber : string.Empty;
                    if (doc.Client.Barrio==null)
                        address = "N/D";
                    else
                        address = ((doc.Client.Barrio != null) ? doc.Client.Barrio.NombreBarrio : "") + " " + ((doc.Client.Barrio.Distrito != null) ? doc.Client.Barrio.Distrito.NombreDistrito : "") + " " + ((doc.Client.Barrio.Distrito.Canton != null) ? doc.Client.Barrio.Distrito.Canton.NombreCanton : "") + " " + ((doc.Client.Barrio.Distrito.Canton.Provincia) != null ? doc.Client.Barrio.Distrito.Canton.Provincia.NombreProvincia : "") + " " + doc.Client.Address;

                }
                str += CommonPrinter.TXT_STYLE_NORMAL;
                str += BR;
                str += CommonPrinter.StrSplit("Cliente: " + CommonPrinter.normalizeStringChars(clientName),LineLength);
                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(tipoidentificacion + ": " + identificacion),LineLength);
                str += CommonPrinter.StrSplit("Codigo: " + codigo,LineLength);
                if (LineLength >= 40)
                {
                    str += string.Format("Movil: {0} - Tel: {1}", Mobilphone.PadRight(12, ' '), phoneClient.PadRight(12, ' ')).PadRight(LineLength, ' ') + CommonPrinter.NEW_LINE;
                }
                else
                {
                    str += string.Format("Movil: {0}", Mobilphone.PadRight(12, ' ')).PadRight(LineLength, ' ') + CommonPrinter.NEW_LINE;
                    str += string.Format("Tel: {0}", phoneClient.PadRight(12, ' ')).PadRight(LineLength, ' ') + CommonPrinter.NEW_LINE;
                }
                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(string.Format("Direccion: {0}", address)),LineLength);               
                str += BR;
                str += string.Format("Moneda: {0} ", doc.CodigoMoneda).PadRight(LineLength, ' ') + CommonPrinter.NEW_LINE;
                str += CommonPrinter.normalizeStringChars(string.Format("Condicion de venta: {0} ", TicoPay.Application.Helpers.EnumHelper.GetDescription(doc.ConditionSaleType)).PadRight(LineLength, ' ')) + CommonPrinter.NEW_LINE;

                if (doc.ConditionSaleType == FacturaElectronicaCondicionVenta.Credito)
                    str += ("Fecha de vencimiento: " + doc.ExpirationDate.Value.ToString("dd-MM-yyyy")).PadRight(LineLength, ' ') + CommonPrinter.NEW_LINE;

                str += CommonPrinter.NEW_LINE;
            }
            catch (Exception ex)
            {

                throw;
            }

            return str;

        }

        public virtual string BuildDetails(DocumentPrint doc)
        {
            string str = string.Empty;

            //========================================//
            //DESCRIPCION                        TOTAL//
            //==============================  ========//
            //Servcio de prueba                       //
            //(1.00x4,596.33)  -10 prc Desc   00000.00//                        
            //Servcio de prueba con                   //
            //descripcion larga                       //
            //(2.00x4,596.33)                 00000.00//    
            //----------------------------------------//    
            //Sub-Total:                      00000.00//

            //Exento:                        00000.00//
            //Gravado:                        00000.00//
            //Impuesto:                       00000.00//            
            //TOTAL:                          00000.00//


            str = HeaderDetails(str);
            //  decimal subtotal = 0, totalGravada=0, totalExento=0, neto=0;
            foreach (var item in doc.Details.OrderBy(x => x.LineNumber))
            {
                var Discount = string.Empty;
                if (item.DiscountPercentage > 0)
                {
                    Discount = string.Format("-{0} prc Desc", item.DiscountPercentage.ToString("N2"));
                }
                //subtotal += item.SubTotal;
                //neto += item.Total;
                //totalGravada += item.Tax.Rate > 0 ? item.SubTotal : 0;
                //totalExento+= item.Tax.Rate==0? item.SubTotal : 0;

                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(item.Title), LineLength);
                if (item.Note != null)
                    str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(item.Note), LineLength);
                var unidad = TicoPay.Application.Helpers.EnumHelper.GetXmlEnumAttributeValueFromEnum(typeof(UnidadMedidaType), item.UnitMeasurement);
                unidad = unidad == null ? string.Empty : CommonPrinter.normalizeStringChars(unidad).Replace("/", "").Replace("(", "").Replace(")", "");
                str += CommonPrinter.StrSplit(string.Format("({0}{1} x {2}) {3}", item.Quantity.ToString("N2"), unidad, item.PricePerUnit.ToString("N2"), Discount), StrLength, true) + CommonPrinter.StrSplit(item.LineTotal.ToString("N2"), IntLength, false) + CommonPrinter.NEW_LINE;
                str += CommonPrinter.StrSplit((item.Tax.Rate > 0 ? (string.Format("Impuesto: {0} prc", item.Tax.Rate.ToString("N2"))) : "Exento"), StrLength, true) + CommonPrinter.NEW_LINE;
            }
            str += BR;
            //str += CommonPrinter.StrSplit(string.Format("Neto:"), StrLength, true) + CommonPrinter.StrSplit(neto.ToString("N2"), IntLength, false) + CommonPrinter.NEW_LINE;
            //var totaldesc = doc.DiscountAmount > 0 ? "-" + doc.DiscountAmount.ToString("N2") : "0.00";
            //str += CommonPrinter.StrSplit(string.Format("Total Descuento: "), StrLength, true) + CommonPrinter.StrSplit(totaldesc, IntLength, false) + CommonPrinter.NEW_LINE;
            //str += CommonPrinter.StrSplit(string.Format("Sub-total:"), StrLength, true) + CommonPrinter.StrSplit(subtotal.ToString("N2"), IntLength, false) + CommonPrinter.NEW_LINE;
            //str += BR;

            str += CommonPrinter.NEW_LINE;
            str += CommonPrinter.StrSplit(string.Format("Total Gravado:"), StrLength, true) + CommonPrinter.StrSplit(doc.TotalGravado.ToString("N2"), IntLength, false) + CommonPrinter.NEW_LINE;
            str += CommonPrinter.StrSplit(string.Format("Total Exento:"), StrLength, true) + CommonPrinter.StrSplit(doc.TotalExento.ToString("N2"), IntLength, false) + CommonPrinter.NEW_LINE;
            var totaldesc = doc.DiscountAmount > 0 ? "-" + doc.DiscountAmount.ToString("N2") : "0.00";
            str += CommonPrinter.StrSplit(string.Format("Total Descuento: "), StrLength, true) + CommonPrinter.StrSplit(totaldesc, IntLength, false) + CommonPrinter.NEW_LINE;
            str += CommonPrinter.StrSplit(string.Format("Total Impuesto:"), StrLength, true) + CommonPrinter.StrSplit(doc.TotalTax.ToString("N2"), IntLength, false) + CommonPrinter.NEW_LINE;
            str += CommonPrinter.StrSplit(string.Format("TOTAL:"), StrLength, true) + CommonPrinter.StrSplit(doc.Total.ToString("N2"), IntLength, false) + CommonPrinter.NEW_LINE;

            return str;
        }

        public virtual string HeaderDetails(string str)
        {
            str += "========================================" + CommonPrinter.NEW_LINE;
            str += "DESCRIPCION                        TOTAL" + CommonPrinter.NEW_LINE;
            str += "==============================  ========" + CommonPrinter.NEW_LINE;
            return str;
        }

        public virtual string BuildFooter(DocumentPrint doc)
        {
            //----------------------------------------//
            //     Datos Comprobante  provisional     //
            //            de  contingencia            //
            //Nro: 00100001010000000081               //
            //Fecha: 29-08-2018                       //
            //Motivo:  Lorem ipsum dolor sit          //
            //amet, consectetur adipiscing elit.      //
            //Maecenas dui erat, auctor non cursus    //
            //----------------------------------------//
            //Observaciones: Lorem ipsum dolor sit    //
            //amet, consectetur adipiscing elit.      //
            //Maecenas dui erat, auctor non cursus    //
            //sit amet, imperdiet nec orci.Maecenas   //
            //tincidunt varius imperdiet. In          //
            //scelerisque mi at auctor cursus. Pel    //
            //----------------------------------------//
            //Estado Factura Electronica:XXXXX        //
            //Medios de pago                          //
            //Deposito Transferencia: XX.XX           //
            //Cheque: XX.XX                           //
            //Efectivo: XX.XX                         //
            //Tarjeta: XX.XX                          //
            //----------------------------------------//    
            // Incluido en el Registro de Facturacion //
            //       Electronica, segun normativa     //
            //              DGT-R-48-2016             //

            string str = string.Empty;

            str += BR;
            if (doc.IsContingency)
            {
                str += "     Datos Comprobante  provisional     " + CommonPrinter.NEW_LINE;
                str += "            de  contingencia            " + CommonPrinter.NEW_LINE;
                str += "Nro: " + doc.ConsecutiveNumberContingency + CommonPrinter.NEW_LINE;
                str += "Fecha: " + doc.DateContingency.Value.ToString("dd-MM-yyyy") + CommonPrinter.NEW_LINE;
                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(string.Format("Motivos: {0}", doc.ReasonContingency)), LineLength) + CommonPrinter.NEW_LINE;
                str += BR;
            }
            if (doc.GeneralObservation != "")
            {
                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(string.Format("Observaciones: {0}", doc.GeneralObservation)), LineLength) + CommonPrinter.NEW_LINE;
                str += BR;
            }
            str += CommonPrinter.normalizeStringChars("Estado " + TicoPay.Application.Helpers.EnumHelper.GetDescription(doc.TypeDocument) + ":" + TicoPay.Application.Helpers.EnumHelper.GetDescription(doc.Status)).PadRight(LineLength, ' ') + CommonPrinter.NEW_LINE;
            if ((doc.Payments != null) && (doc.Payments.Count > 0))
            {
                str += "Medios de pago" + CommonPrinter.NEW_LINE;

                foreach (var item in doc.Payments)
                {
                    var pstr = TicoPay.Application.Helpers.EnumHelper.GetDescription(item.Payment.PaymetnMethodType) + ": " + item.Amount.ToString("N2");
                    str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(pstr), LineLength);
                }

            }

            str = IncludeResolucionMessage(str);

            str += CommonPrinter.NEW_LINE;
            str += CommonPrinter.NEW_LINE;
            str += CommonPrinter.CMD_CUT;
            str += CommonPrinter.INITIALIZE_PRINTER;

            //if (invoice.Status == Status.Completed && mediopago != null)
            //{
            //    string medioPago = "";
            //    int cont = 0;
            //    foreach (FacturaElectronicaMedioPago pago in mediopago)
            //    {
            //        if (cont != 0 && cont < mediopago.Count())
            //            medioPago = medioPago + " / ";

            //        if (pago == FacturaElectronicaMedioPago.Transferencia_Deposito_Bancario)
            //            medioPago = medioPago + "Depósito";
            //        else
            //            medioPago = medioPago + pago.ToString();

            //        cont = cont + 1;
            //    }

            //    header.AddCell(CreateLabeledTextPdfPCell("Medio de pago:", medioPago, SubTitleFont, BodyFont, 2, 2));
            //}
            //else if (invoice.Status == Status.Completed && mediopago == null)
            //{
            //    StringBuilder medioPago = new StringBuilder();
            //    int y = 0;
            //    foreach (var payment in invoice.InvoicePaymentTypes)
            //    {
            //        medioPago.Append(payment.Payment.PaymetnMethodType.GetDescription() + ((y < invoice.InvoicePaymentTypes.Count - 1) ? "/" : ""));
            //        y++;
            //    }

            //    header.AddCell(CreateLabeledTextPdfPCell("Medio de pago:", medioPago.ToString(), SubTitleFont, BodyFont, 2, 2));
            //}



            return str;
        }

        public virtual string IncludeResolucionMessage(string str)
        {
            str += BR;
            str += " Incluido en el Registro de Facturacion " + CommonPrinter.NEW_LINE;
            str += "      Electronica, segun normativa      " + CommonPrinter.NEW_LINE;
            str += "              DGT-R-48-2016             " + CommonPrinter.NEW_LINE;
            str += BR;
            return str;
        }

        public string print(DocumentPrint docuemnt)
        {
            var doc = string.Empty;// ESC + "@";
            doc += CommonPrinter.INITIALIZE_PRINTER;
            doc += CommonPrinter.NEW_LINE;
            doc += BuildHeader(docuemnt);
            doc += BuildClientInfo(docuemnt);
            doc += BuildDetails(docuemnt);
            doc += BuildFooter(docuemnt);
            return doc;
        }
    }
}
