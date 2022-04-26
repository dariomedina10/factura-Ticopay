using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices.XSD;

namespace TicoPay.Printers
{
    

    public class Matriz : IModelPrinter
    {
        public const string BR = @"-----------------------------------------------------------------------------------------------\r\n";

        public const int LineLength = 98;
        public const int IntLength = 20;
        public const int maxRow = 12;
        public const int StrLengthCol01 = 57;
        public const int StrLengthCol02 = 20;
        public const int MaxLineDetails = 10;

        public Matriz()
        { }

        public string BuildHeader(DocumentPrint doc)
        {
            //HNG CARMENTA GLOBALGROUP SOCIEDAD ANONIMA                                                      //
            //Razon Social: XXXXXXXXXXXXX                                                                    //
            //Cedula<tipo>: XXXXXXXXXXXXX                                                                    //
            //Tel: 123-00000000 - Fax: 123-000000000                                                         //
            //Dirección: xxxxxxxxxxx xxxxx xxxxxx xxxxxxxx xxxxxxxxx xxxxxxxxxxxxxx                          //
            //Sucursal: PRINCIPAL                                                                            //
            //Direccion Sucursal: xxxxx xxxxx xxxx xxxxxxxxxx  xxxxxxxxxxxx                                  //
            //                                                                                               //
            //Clave: 50606061800310174178800100001010000003502196437526                                      //
            //FACTURA ELECTRONICA Nro: 00100001010000000080        Fecha Emision: 02/11/2017 – Hora: 10:00 am//  
            //Documento Ref: FACTURA ELECTRONICA Nro 00100001010000000080                  
            //-----------------------------------------------------------------------------------------------//

            string str = string.Empty;
            try
            {  
                str += CommonPrinter.TXT_STYLE_NORMAL;
                str += CommonPrinter.StrPad(CommonPrinter.normalizeStringChars(doc.Tenant.BussinesName.ToUpper()), LineLength,' ') + CommonPrinter.NEW_LINE;
                str += CommonPrinter.TXT_STYLE_NORMAL;
                str += CommonPrinter.StrPad("Razon social: " + CommonPrinter.normalizeStringChars(doc.Tenant.Name), LineLength,' ') + CommonPrinter.NEW_LINE; 
                str += CommonPrinter.StrPad(CommonPrinter.normalizeStringChars(string.Format("{0}: {1}", doc.Tenant.IdentificationTypeToString(), doc.Tenant.IdentificationNumber)), LineLength,' ') + CommonPrinter.NEW_LINE; 
                var fax = string.IsNullOrWhiteSpace(doc.Tenant.Fax) ? " " : doc.Tenant.Fax;
                var phone = string.IsNullOrWhiteSpace(doc.Tenant.PhoneNumber) ? " " : doc.Tenant.PhoneNumber;
                str += CommonPrinter.StrPad(string.Format("Tel: {0} - Fax: {1}", phone, fax),LineLength, ' ') + CommonPrinter.NEW_LINE;
                
                var address = string.Empty;
                if (doc.Tenant.IsAddressShort)
                    address = doc.Tenant.AddressShort;
                else
                    address = doc.Tenant.Barrio.NombreBarrio + " " + doc.Tenant.Barrio.Distrito.NombreDistrito + " " +
                        doc.Tenant.Barrio.Distrito.Canton.NombreCanton + " " + doc.Tenant.Barrio.Distrito.Canton.Provincia.NombreProvincia + " " + doc.Tenant.Address;

                str += CommonPrinter.StrPad(CommonPrinter.normalizeStringChars(string.Format("Direccion: {0}", address)), LineLength, ' ') + CommonPrinter.NEW_LINE;

                if (doc.BranchOffice != null)
                {
                    str += CommonPrinter.StrSplit("Sucursal: " + CommonPrinter.normalizeStringChars(doc.BranchOffice.Name), LineLength) + CommonPrinter.NEW_LINE;
                    if (!string.IsNullOrWhiteSpace(doc.BranchOffice.Location))
                    {
                        str += CommonPrinter.StrSplit("Direccion Sucursal: " + CommonPrinter.normalizeStringChars(doc.BranchOffice.Location), LineLength) + CommonPrinter.NEW_LINE; 
                    }
                }

                str += CommonPrinter.NEW_LINE;
                str += CommonPrinter.TXT_STYLE_NORMAL;
                str += string.Format("Clave: {0}", doc.VoucherKey).PadRight(LineLength, ' ') + CommonPrinter.NEW_LINE;
                str += (CommonPrinter.normalizeStringChars(TicoPay.Application.Helpers.EnumHelper.GetDescription(doc.TypeDocument).ToUpper()) 
                + " Nro: " + doc.ConsecutiveNumber.ToString()).PadRight(57, ' ') ;       
                str += string.Format("Fecha : {0}  Hora: {1}", doc.DueDate.ToString("dd-MM-yyyy"), doc.DueDate.ToShortTimeString()).PadLeft(LineLength-57, ' ') + CommonPrinter.NEW_LINE;
                if (doc.DocumentRef != null)
                {                    
                    str += (string.Format("Documento Ref: {0} Nro {1}", CommonPrinter.normalizeStringChars(doc.DocumentRef.TypeDocument), doc.DocumentRef.ConsecutiveNumber)) + CommonPrinter.NEW_LINE;
                  
                }
                str += CommonPrinter.TXT_STYLE_NORMAL;
                str += CommonPrinter.BR(LineLength);

            }
            catch (Exception ex)
            {
                throw;
            }

            return str;
        }

        public string BuildClientInfo(DocumentPrint doc)
        {
                              
            //Cliente: xxxxxx xxxxxx xxxxxxxxx                                                               //
            //Cedula: XXXXXXXXXXXXX                           Codigo: XXXXXXXXXXXXX                          //           
            //Direccion:                                                                                     //            
            //-----------------------------------------------------------------------------------------------//
                              


            string tipoidentificacion = string.Empty, identificacion = string.Empty;
            string clientName = string.Empty, phoneClient = string.Empty, address = string.Empty, Mobilphone = string.Empty, codigo = string.Empty;
            string str = string.Empty;
            try
            {
                if (doc.Client != null)
                {
                    tipoidentificacion = doc.Client.IdentificationTypeToString();
                    clientName = doc.Client.Name + ((doc.Client.IdentificationType != Invoices.XSD.IdentificacionTypeTipo.Cedula_Juridica && doc.Client.LastName != null) ? " " + doc.Client.LastName : string.Empty);
                    identificacion = (doc.Client.IdentificationType == Invoices.XSD.IdentificacionTypeTipo.NoAsiganda ? doc.Client.IdentificacionExtranjero : doc.Client.Identification);
                    phoneClient = string.IsNullOrWhiteSpace(doc.Client.PhoneNumber) ? string.Empty : doc.Client.PhoneNumber;
                    codigo = doc.Client.Code.ToString();                   
                    Mobilphone = (!string.IsNullOrWhiteSpace(doc.Client.MobilNumber)) ? doc.Client.MobilNumber : string.Empty;
                    if (doc.Client.Barrio == null)
                        address = "N/D";
                    else
                        address = doc.Client.Barrio.NombreBarrio + " " + doc.Client.Barrio.Distrito.NombreDistrito + " " + doc.Client.Barrio.Distrito.Canton.NombreCanton + " " + doc.Client.Barrio.Distrito.Canton.Provincia.NombreProvincia + " " + doc.Client.Address;

                }                         
                str += CommonPrinter.StrPad("Cliente: " + CommonPrinter.normalizeStringChars(clientName), LineLength,' ') + CommonPrinter.NEW_LINE;
                str += (CommonPrinter.normalizeStringChars(tipoidentificacion + ": " + identificacion)).PadRight(StrLengthCol01, ' ');
                str += ("Codigo: " + codigo).PadRight(LineLength - StrLengthCol01, ' ') + CommonPrinter.NEW_LINE;

                str += CommonPrinter.StrPad(CommonPrinter.normalizeStringChars(string.Format("Direccion: {0}", address)), LineLength,' ') + CommonPrinter.NEW_LINE;
                str += CommonPrinter.BR(LineLength); ;

              
               
            }
            catch (Exception ex)
            {

                throw;
            }

            return str;

        }

        public string BuildDetails(DocumentPrint doc)
        {
            string str = string.Empty;

            //DESCRIPCION                               CANT               PRECIO   DESC  IMPUESTO        MONTO//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //Servicio con descripción xxxxxxxxxxxxxxx 0000001.00 Sp 000000000.00  000.00  000.00  000000000.00//
            //--------------------------------------------------------------------------------------------------//
            //                                                           Subtotal:    CRC 0000000000000000.00
            //                                                     Total Impuesto:    CRC 0000000000000000.00
            //                                                              TOTAL:    CRC 0000000000000000.00
            //-----------------------------------------------------------------------------------------------//



            str += "DESCRIPCION                               CANT               PRECIO    DESC  IMPUESTO       MONTO" + CommonPrinter.NEW_LINE;
           
            decimal subtotal = 0, totalGravada = 0, totalExento = 0, neto = 0;
            var i = 0;
            foreach (var item in doc.Details.OrderBy(x => x.LineNumber).Take(MaxLineDetails))
            {
                var unidad = TicoPay.Application.Helpers.EnumHelper.GetXmlEnumAttributeValueFromEnum(typeof(UnidadMedidaType), item.UnitMeasurement);
                unidad = unidad == null ? CommonPrinter.normalizeStringChars(item.UnitMeasurement.ToString()) : CommonPrinter.normalizeStringChars(unidad).Replace("/","").Replace("(","").Replace(")","");
                var cant = CommonPrinter.StrPad(item.Quantity.ToString("N2").Replace(",", "") + unidad, 12, ' ');
                 i++;
                str += string.Format(
                        "{0} {1} {2}  {3}  {4}  {5}", CommonPrinter.StrPad(CommonPrinter.normalizeStringChars(item.Title),40,' '), cant,
                        (item.PricePerUnit.ToString("N2").Replace(",", "").PadLeft(12, ' ')), (item.DiscountPercentage.ToString("N2").Replace(",", "").PadLeft(6, ' '))
                        , (item.Tax.Rate.ToString("N2").Replace(",", "").PadLeft(6, ' ')), (item.LineTotal.ToString("N2").Replace(",", "").PadLeft(13, ' '))
                    ) 
                    + CommonPrinter.NEW_LINE;
               
                subtotal += item.SubTotal;


            }
            while (i<MaxLineDetails)
            {              
                str += CommonPrinter.NEW_LINE;
                i++;
            }
            str += CommonPrinter.BR(LineLength);
            str += ("Sub-total:").PadLeft(StrLengthCol01, ' ') + string.Format("{0} {1}", doc.CodigoMoneda.ToString() ,subtotal.ToString("N2")).PadLeft(LineLength- StrLengthCol01, ' ')+ CommonPrinter.NEW_LINE;
            str += ("Total Impuesto:").PadLeft(StrLengthCol01, ' ') + string.Format("{0} {1}", doc.CodigoMoneda.ToString(), doc.TotalTax.ToString("N2")).PadLeft(LineLength - StrLengthCol01, ' ') + CommonPrinter.NEW_LINE;
            str += ("TOTAL:").PadLeft(StrLengthCol01, ' ') + string.Format("{0} {1}", doc.CodigoMoneda.ToString(), doc.Total.ToString("N2")).PadLeft(LineLength - StrLengthCol01, ' ') + CommonPrinter.NEW_LINE;
            str += CommonPrinter.BR(LineLength); 

           
            return str;
        }

        public string BuildFooter(DocumentPrint doc)
        {
            //--------------------------------------------------------------------------------------------------
            //Datos Comprobante  provisional de  contingencia     
            //Nro: 00100001010000000081               
            //Fecha: 29-08-2018                       
            //Motivo:  Lorem ipsum dolor sit          
            //amet, consectetur adipiscing elit.      
            //Maecenas dui erat, auctor non cursus    
            //--------------------------------------------------------------------------------------------------
            //Observaciones: Lorem ipsum dolor sit amet, consectetur adipiscing elit.Maecenas dui erat, auctor
            //non cursus sit amet, imperdiet nec orci.Maecenas tincidunt varius imperdiet. In scelerisque mi
            //at auctor cursus. Pel
            //--------------------------------------------------------------------------------------------------
            //Condicion de venta: Credito - Fecha de vencimiento: 02/11/2017
            //Estado factura: Pagada
            //Medio de pago: Efectivo / Cheque
            //-----------------------------------------------------------------------------------------------//
            //      Incluido en el Registro de Facturacion Electrónica, segun normativa DGT-R - 48 - 2016



            string str = string.Empty;
            string vence = string.Empty;
            str += CommonPrinter.BR(LineLength);
            if (doc.IsContingency)
            {
                str += "Datos Comprobante provisional de contingencia" + CommonPrinter.NEW_LINE;
                str += "Nro: " + doc.ConsecutiveNumberContingency + CommonPrinter.NEW_LINE;
                str += "Fecha: " + doc.DateContingency.Value.ToString("dd-MM-yyyy") + CommonPrinter.NEW_LINE;
                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(string.Format("Motivos: {0}", doc.ReasonContingency)), LineLength) + CommonPrinter.NEW_LINE;
                str += CommonPrinter.BR(LineLength);
            }
            if (doc.GeneralObservation != "")
            {
                str += CommonPrinter.StrSplit(CommonPrinter.normalizeStringChars(string.Format("Observaciones: {0}", doc.GeneralObservation)), LineLength) + CommonPrinter.NEW_LINE;
                str += CommonPrinter.BR(LineLength);
            }

            if (doc.ConditionSaleType == FacturaElectronicaCondicionVenta.Credito)
                vence = " - Fecha de vencimiento: " + doc.ExpirationDate.Value.ToString("dd-MM-yyyy");           

            str += CommonPrinter.StrPad(CommonPrinter.normalizeStringChars(string.Format("Condicion de venta: {0} {1}", TicoPay.Application.Helpers.EnumHelper.GetDescription(doc.ConditionSaleType),vence)),LineLength,' ')+ CommonPrinter.NEW_LINE;            
            str += CommonPrinter.normalizeStringChars("Estado " + TicoPay.Application.Helpers.EnumHelper.GetDescription(doc.TypeDocument) + ":" + TicoPay.Application.Helpers.EnumHelper.GetDescription(doc.Status)).PadRight(LineLength, ' ') + CommonPrinter.NEW_LINE;
            if ((doc.Payments != null) && (doc.Payments.Count > 0))
            {
                str += "Medios de pago: " ;
                var pstr = string.Empty;
                foreach (var item in doc.Payments)
                {
                     pstr = TicoPay.Application.Helpers.EnumHelper.GetDescription(item.Payment.PaymetnMethodType) + " - ";
                  
                }

                str += pstr.Substring(0,pstr.Length-3) + CommonPrinter.NEW_LINE;

            }
            str += CommonPrinter.BR(LineLength);
            //-----------------------------------------------------------------------------------------------//
            str += "       Incluido en el Registro de Facturacion Electronica, segun normativa DGT-R-48-2016       " + CommonPrinter.NEW_LINE;
            str += CommonPrinter.INITIALIZE_PRINTER;
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

        public string HeaderDetails(string str)
        {
            throw new NotImplementedException();
        }
    }

   
}
