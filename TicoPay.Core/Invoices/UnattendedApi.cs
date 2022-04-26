using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.UI;
using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature.Parameters;
using iTextSharp.text.pdf.qrcode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Core.Common;
using TicoPay.General;
using TicoPay.Invoices.XSD;
using TicoPay.MultiTenancy;
using TicoPay.Taxes;
using ZXing;

namespace TicoPay.Invoices
{
    public class UnattendedApi : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited, IComprobanteRecepcion
    {
        public const string facturaelectronica = "01";
        public virtual Tenant Tenant { get; set; }
        public int TenantId { get; set; }
        /// <summary>
        /// Clave del comprobante
        /// </summary>
        [MaxLength(50)]
        public string VoucherKey { get; set; }
        /// <summary>
        /// Numero consecutivo
        /// </summary>
        /// 
        [MaxLength(20)]
        [Display(Name = "No. Factura")]
        public string ConsecutiveNumber { get; set; }
        /// <summary>
        /// Ruta XML Firmado
        /// </summary>
        /// 
        public string ElectronicBill { get; set; }
        ///// <summary>
        ///// Codigo QR
        ///// </summary>
        //public string QRCodeGenerator { get; set; }
        public byte[] QRCode { get; set; }
        /// <summary>
        /// Indica si la factura fue enviada a Hacienda
        /// </summary>
        public bool SendInvoice { get; set; }

        public DateTime DueDate { get; set; }
        /// <summary>
        /// Estatus de la factura en hacienda
        /// </summary>
        public StatusTaxAdministration StatusTribunet { get; set; }

        /// <summary>
        /// Estatus del comprobante electronico
        /// </summary>
        public VoucherSituation StatusVoucher { get; set; }
        /// <summary>
        /// Mensaje de respuesta de la administracion triutaria
        /// </summary>
        public string MessageTaxAdministration { get; set; }

        [Index("IX_Number")]
        public int Number { get; set; }

        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }

        public static UnattendedApi Create(int tenantId, DateTime dueDate)
        {
            var entity = new UnattendedApi
            {
                Id = Guid.NewGuid(),
                DueDate = dueDate
            };
            return entity;
        }

        public void CreateXML(Tenant tenant, Certificate certificate, string voucherKey, XmlDocument xmlDoc)
        {
            string path = Path.Combine(WorkPaths.GetXmlClientPath(), voucherKey + ".xml");

            using (XmlTextWriter writer = new XmlTextWriter(path, null))
            {
                writer.Formatting = Formatting.Indented;
                xmlDoc.Save(writer);
            }
            path = Path.Combine(WorkPaths.GetXmlSignedClientPath(), voucherKey + ".xml");

            if (tenant.ValidateHacienda && certificate != null)
            {
                //Guarda el certificado temporalmente
                string archivo = (DateTimeZone.Now().ToString("yyyyMMddHHmmss") + "-" + certificate.FileName).ToLower();
                string certPath = Path.Combine(WorkPaths.GetCertifiedPath(), archivo);
                try
                {
                    File.WriteAllBytes(certPath, certificate.CertifiedRoute);
                }
                catch (IOException)
                {
                }
                SignedXMLXADES(Path.Combine(WorkPaths.GetXmlClientPath(), voucherKey + ".xml"), Path.Combine(WorkPaths.GetXmlSignedClientPath(), voucherKey + ".xml"), Path.Combine(certPath), certificate.Password);
            }
        }

        public void SignedXMLXADES(string RutaXML, string RutaXMLSigned, string archivo, string password)
        {
            XadesService xadesService = new XadesService();
            SignatureParameters parametros = new SignatureParameters();

            //parametros.SignatureMethod = SignatureMethod.RSAwithSHA256;
            parametros.SigningDate = DateTimeZone.Now();

            // Política de firma de factura-e 3.1
            parametros.SignaturePolicyInfo = new SignaturePolicyInfo();
            parametros.SignaturePolicyInfo.PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4.1/Resolucion_Comprobantes_Electronicos_DGT-R-48-2016.pdf";
            parametros.SignaturePolicyInfo.PolicyHash = "NmI5Njk1ZThkNzI0MmIzMGJmZDAyNDc4YjUwNzkzODM2NTBiOWUxNTBkMmI2YjgzYzZjM2I5NTZlNDQ4OWQzMQ==";
            parametros.SignaturePackaging = SignaturePackaging.ENVELOPED;
            parametros.InputMimeType = "text/xml";

            X509Certificate2 selectedCertificate = new X509Certificate2(@archivo, password, X509KeyStorageFlags.Exportable);

            //using (parametros.Signer = new Signer(CertsHelper.Load("TicoPay.Common." + archivo, password)))
            using (parametros.Signer = new Signer(selectedCertificate))
            {
                if (File.Exists(RutaXML))
                {
                    using (FileStream fs = new FileStream(@RutaXML, FileMode.Open))
                    {
                        var docFirmado = xadesService.Sign(fs, parametros);
                        docFirmado.Save(@RutaXMLSigned);
                    }
                }
            }
            // borra el certificado del directorio temporal
            DeleteFile(archivo);
        }

        public void DeleteFile(string path)
        {
            FileInfo filecert = new FileInfo(path);
            filecert.Delete();
        }

        public void AddAtribbuteNodoXML(XmlDocument xmlDoc, UnattendedApi useConsecutive, TipoDocumento tipoDocumento)
        {
            XmlNode nodoHijo = xmlDoc.SelectSingleNode(tipoDocumento.ToString());

            if (nodoHijo.Attributes.Count == 0)
            {
                AddAtributte(xmlDoc, tipoDocumento.ToString(), "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                AddAtributte(xmlDoc, tipoDocumento.ToString(), "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            }

            AddAtributte(xmlDoc, tipoDocumento.ToString(), "xmlns", AddEsquema(tipoDocumento));
            VerificarNodoXML(xmlDoc, NodoXml.NumeroConsecutivo, useConsecutive, tipoDocumento);
            VerificarNodoXML(xmlDoc, NodoXml.Clave, useConsecutive, tipoDocumento);
            ValidateFax(xmlDoc);
            AddResolution(xmlDoc, tipoDocumento);
            XmlTextWriter writer = new XmlTextWriter(Console.Out)
            {
                Formatting = Formatting.Indented
            };
            xmlDoc.WriteTo(writer);
        }

        public void VerificarNodoXML(XmlDocument xmlDoc, NodoXml nodo, UnattendedApi useConsecutive, TipoDocumento tipoDocumento)
        {
            switch ((int)nodo)
            {
                case 0:
                    {
                        if (xmlDoc.SelectSingleNode("//" + nodo.ToString()) == null)
                        {
                            XmlNode doc = xmlDoc.SelectSingleNode("//" + tipoDocumento.ToString());
                            XmlNode numeroConsecutivo = xmlDoc.SelectSingleNode("//" + NodoXml.NumeroConsecutivo.ToString());
                            XmlNode clave = xmlDoc.CreateNode(XmlNodeType.Element, NodoXml.Clave.ToString(), null);
                            clave.InnerText = useConsecutive.VoucherKey;
                            doc.InsertBefore(clave, numeroConsecutivo);
                        }
                        break;
                    }
                case 1:
                    {
                        if (xmlDoc.SelectSingleNode("//" + nodo.ToString()) == null)
                        {
                            XmlNode doc = xmlDoc.SelectSingleNode("//" + tipoDocumento.ToString());
                            XmlNode fecha = xmlDoc.SelectSingleNode("//" + NodoXml.FechaEmision.ToString());
                            XmlNode consecutive = xmlDoc.CreateNode(XmlNodeType.Element, NodoXml.NumeroConsecutivo.ToString(), null);
                            consecutive.InnerText = useConsecutive.ConsecutiveNumber;
                            doc.InsertBefore(consecutive, fecha);
                        }
                        break;
                    }
            }
        }

        public void AddResolution(XmlDocument xmlDoc, TipoDocumento tipoDocumento)
        {
            XmlNode doc = xmlDoc.SelectSingleNode("//" + tipoDocumento.ToString());
            XmlNode normativa = xmlDoc.CreateNode(XmlNodeType.Element, "Normativa", null);
            XmlNode numeroResolucion = xmlDoc.CreateNode(XmlNodeType.Element, "NumeroResolucion", null);
            numeroResolucion.InnerText = ConfigurationManager.AppSettings["XML.NumeroResolucion"];
            XmlNode fechaResolucion = xmlDoc.CreateNode(XmlNodeType.Element, "FechaResolucion", null);
            fechaResolucion.InnerText = ConfigurationManager.AppSettings["XML.FechaResolucion"];
            normativa.AppendChild(numeroResolucion);
            normativa.AppendChild(fechaResolucion);
            doc.AppendChild(normativa);
        }

        private void AddAtributte(XmlDocument xmlDoc, string nodo, string attribute, string item)
        {
            XmlNode nodoHijo = xmlDoc.SelectSingleNode(nodo);
            
            //agregarle el atributo
            XmlAttribute atr = xmlDoc.CreateAttribute(attribute);
            atr.Value = item;
            nodoHijo.Attributes.SetNamedItem(atr);
        }

        private string AddEsquema(TipoDocumento tipoDocumento)
        {
            string esquema = "";
            switch ((int)tipoDocumento)
            {
                case 0:
                    {
                        esquema = "https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/facturaElectronica";
                        break;
                    }
                case 1:
                    {
                        esquema = "https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/notaCreditoElectronica";
                        break;
                    }
                case 2:
                    {
                        esquema = "https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/notaDebitoElectronica";
                        break;
                    }
                case 3:
                    {
                        esquema = "https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/tiqueteElectronico";
                        break;
                    }
            }
            return esquema;
        }

        public TipoDocumento DeterminarTipoDocumento(XmlDocument xmlDoc)
        {
            List<string> listErrores = new List<string>();
            XmlElement nav = xmlDoc.DocumentElement;
            XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);
            ns.AddNamespace("msbld", nav.NamespaceURI);

            TipoDocumento tipoDocumento = new TipoDocumento();
            if (nav.SelectSingleNode("//msbld:" + TipoDocumento.FacturaElectronica.ToString(), ns) != null)
            {
                //if (xmlDoc.GetElementsByTagName("Receptor").Count > 0)
                tipoDocumento = TipoDocumento.FacturaElectronica;
                //else
                //    listErrores.Add("La etiqueta Receptor par

            }
            else if (nav.SelectSingleNode("//msbld:" + TipoDocumento.NotaCreditoElectronica.ToString(), ns) != null)
            {
                listErrores.Add("No esta implementado generar " + TipoDocumento.NotaCreditoElectronica);
                //tipoDocumento = TipoDocumento.NotaCreditoElectronica;
            }
            else if (nav.SelectSingleNode("//msbld:" + TipoDocumento.NotaDebitElectronica.ToString(), ns) != null)
            {
                listErrores.Add("No esta implementado generar " + TipoDocumento.NotaDebitElectronica);
                //tipoDocumento = TipoDocumento.NotaDebitElectronica;
            }
            else if (nav.SelectSingleNode("//msbld:" + TipoDocumento.TiqueteElectronico.ToString(), ns) != null)
            {
                tipoDocumento = TipoDocumento.TiqueteElectronico;
            }
            else
            {
                listErrores.Add("Etiqueta de comprobante electrónico Inválido");
            }
            ExceptionXML(listErrores);
            return tipoDocumento;
        }

        public void SetInvoiceNumber(int number)
        {
            Number = number;
        }

        public void UnattendedConsecutivo(XmlDocument xmlDoc)
        {
            List<string> listErrores = new List<string>();
            if ((xmlDoc.SelectSingleNode("//" + NodoXml.NumeroConsecutivo.ToString()) != null) &&
                (xmlDoc.SelectSingleNode("//" + NodoXml.Clave.ToString()) != null))
            {
                listErrores.Add("Se debe enviar el XML sin el nodo Clave y NumeroConsecutivo, esto se genera desde el Api");
                //throw new UserFriendlyException("Se debe enviar el XML sin el nodo Clave y NumeroConsecutivo, esto se genera desde el Api");
            }

            if (xmlDoc.SelectSingleNode("//" + NodoXml.Clave.ToString()) != null)
            {
                listErrores.Add("Se debe enviar el XML sin el nodo Clave, esto se genera desde el Api");
                //throw new UserFriendlyException("Se debe enviar el XML sin el nodo Clave, esto se genera desde el Api");
            }

            if (xmlDoc.SelectSingleNode("//" + NodoXml.NumeroConsecutivo.ToString()) != null)
            {
                listErrores.Add("Se debe enviar el XML sin el nodo NumeroConsecutivo, esto se genera desde el Api");
                //throw new UserFriendlyException("Se debe enviar el XML sin el nodo NumeroConsecutivo, esto se genera desde el Api");
            }

            if (xmlDoc.SelectSingleNode("//" + NodoXml.FechaEmision.ToString()) == null)
            {
                listErrores.Add("Falta el nodo FechaEmision en el XML");
                //throw new UserFriendlyException("Falta el nodo FechaEmision en el XML");
            }
            else
            {
                string fecha = xmlDoc.SelectSingleNode("//" + NodoXml.FechaEmision.ToString()).InnerText;
                if (fecha != "")
                {
                    DateTime result;
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string format = "yyyy-MM-ddTHH:mm:ss.fff";
                    provider = new CultureInfo("es-CR");
                    try
                    {
                        result = DateTime.ParseExact(fecha, format, provider);
                    }
                    catch (UserFriendlyException ex)
                    {
                        listErrores.Add("Formato Fecha en el nodo FechaEmision es Inválido");
                        //throw new UserFriendlyException("Formato Fecha en el nodo FechaEmision es Inválido");
                    }
                }
                else
                {
                    listErrores.Add("El nodo FechaEmision esta vacio");
                    //throw new UserFriendlyException("El nodo FechaEmision esta vacio");
                }


            }
            ExceptionXML(listErrores);
        }

        public void ExceptionXML(List<string> listErrores)
        {
            if (listErrores.Count > 0)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string resultat_Json = jss.Serialize(listErrores);
                throw new UserFriendlyException(resultat_Json);
            }
        }

        public void ValidarDueDate(DateTime dueDate)
        {
            string fecha = dueDate.ToString("dd-MM-yyyy");
            bool valor = false;
            DateTime result;
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "dd-MM-yyyy";
            try
            {
                result = DateTime.ParseExact(fecha, format, provider);
            }
            catch (Exception)
            {
                throw new UserFriendlyException("Parametro dueDate con formato inválido verifique!!");
            }
        }

        public void SetInvoiceConsecutivo(Register tenantRegisters, Tenant tenant, TipoDocumento tipoDocumento, bool incrementLastInvoiceNumber = true)
        {
            string consecutivo = null;
            if (incrementLastInvoiceNumber)
            {
                switch ((int)tipoDocumento)
                {
                    case 0:
                        {
                            if (tenantRegisters.LastInvoiceNumber == 9999999999)
                                tenantRegisters.LastInvoiceNumber = 0;

                            tenantRegisters.LastInvoiceNumber = tenantRegisters.LastInvoiceNumber + 1;
                            consecutivo = tenantRegisters.LastInvoiceNumber.ToString();
                            break;
                        }
                    case 1:
                        {
                            if (tenantRegisters.LastNoteCreditNumber == 9999999999)
                                tenantRegisters.LastNoteCreditNumber = 0;

                            tenantRegisters.LastNoteCreditNumber = tenantRegisters.LastNoteCreditNumber + 1;
                            consecutivo = tenantRegisters.LastNoteCreditNumber.ToString();
                            break;
                        }
                    case 2:
                        {
                            if (tenantRegisters.LastNoteDebitNumber == 9999999999)
                                tenantRegisters.LastNoteDebitNumber = 0;

                            tenantRegisters.LastNoteDebitNumber = tenantRegisters.LastNoteDebitNumber + 1;
                            consecutivo = tenantRegisters.LastNoteDebitNumber.ToString();
                            break;
                        }
                    case 3:
                        {
                            if (tenantRegisters.LastTicketNumber == 9999999999)
                                tenantRegisters.LastTicketNumber = 0;

                            tenantRegisters.LastTicketNumber = tenantRegisters.LastTicketNumber + 1;
                            consecutivo = tenantRegisters.LastTicketNumber.ToString();
                            break;
                        }
                }

            }
            else
            {
                switch ((int)tipoDocumento)
                {
                    case 0:
                        {
                            consecutivo = ((tenantRegisters.LastInvoiceNumber == 9999999999) ? 0 : tenantRegisters.LastInvoiceNumber + 1).ToString();
                            break;
                        }
                    case 1:
                        {
                            consecutivo = ((tenantRegisters.LastNoteCreditNumber == 9999999999) ? 0 : tenantRegisters.LastNoteCreditNumber + 1).ToString();
                            break;
                        }
                    case 2:
                        {
                            consecutivo = ((tenantRegisters.LastNoteDebitNumber == 9999999999) ? 0 : tenantRegisters.LastNoteDebitNumber + 1).ToString();
                            break;
                        }
                    case 3:
                        {
                            consecutivo = ((tenantRegisters.LastTicketNumber == 9999999999) ? 0 : tenantRegisters.LastTicketNumber + 1).ToString();
                            break;
                        }
                }
            }
            ConsecutiveNumber = tenant.local.ToString() + tenantRegisters.RegisterCode.ToString() + facturaelectronica + consecutivo.PadLeft(10, '0');
        }


        public void CheckNodo(XmlDocument xmlDoc, UnattendedApi unattended, Tenant tenant)
        {
            if (xmlDoc.SelectSingleNode("//" + NodoXml.NumeroConsecutivo.ToString()) != null)
            {
                XmlNode numeroConsecutivo = xmlDoc.SelectSingleNode("//" + NodoXml.NumeroConsecutivo.ToString());
                unattended.ConsecutiveNumber = numeroConsecutivo.InnerText;
            }
            else
            {
                throw new UserFriendlyException("No se encontro el nodo NumeroConsecutivo");
            }

            if (xmlDoc.SelectSingleNode("//" + NodoXml.Clave.ToString()) != null)
            {
                XmlNode clave = xmlDoc.SelectSingleNode("//" + NodoXml.Clave.ToString());
                unattended.VoucherKey = clave.InnerText;
            }
            else
            {

                SetInvoiceNumberKey(unattended.DueDate, unattended.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);
            }
            var tipoDocumento = DeterminarTipoDocumento(xmlDoc);
            AddAtribbuteNodoXML(xmlDoc, unattended, tipoDocumento);
        }

        /// <summary>
        /// Genera un codigo de seguridad a la FE
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string SecurityCode(int size)
        {
            System.Random randomGenerate = new System.Random();
            System.String sPassword = "";
            sPassword = System.Convert.ToString(randomGenerate.Next(00000001, 99999999));
            sPassword = sPassword.PadLeft(size, '0');
            return sPassword.Substring(sPassword.Length - size, size);
        }

        /// <summary>
        /// Genera la clave numerica exigida para la FE
        /// </summary>
        /// <param name="tenantRegisters"></param>
        public void SetInvoiceNumberKey(DateTime fechafactura, string consecutive, Tenant tenant, int vouchersituation)
        {
            string dia = fechafactura.Day.ToString();
            string anio = fechafactura.Year.ToString().Substring(2, 2);
            string mes = fechafactura.Month.ToString();
            string securityCode = SecurityCode(8);

            VoucherKey = tenant.Country.CountryCode.ToString() + dia.PadLeft(2, '0') + mes.PadLeft(2, '0') + anio + tenant.IdentificationNumber.PadLeft(12, '0') + consecutive + vouchersituation.ToString() + securityCode;
        }

        public void ReGenerateQrFromVoucherKey()
        {
            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                barcodeWriter.Write(VoucherKey).Save(memoryStream, ImageFormat.Png);
                QRCode = memoryStream.ToArray();
            }
        }

        public void DataNodoParents(XmlNamespaceManager ns, XmlElement nav, Invoice @invoice)
        {
            ns.AddNamespace("ms", nav.NamespaceURI);

            @invoice.VoucherKey = nav.SelectSingleNode("//ms:Clave", ns).InnerText;
            VoucherKey = nav.SelectSingleNode("//ms:Clave", ns).InnerText;
            @invoice.ConsecutiveNumber = nav.SelectSingleNode("//ms:NumeroConsecutivo", ns).InnerText;
            @invoice.DueDate = DateTime.Parse(nav.SelectSingleNode("//ms:FechaEmision", ns).InnerText);

            string valor = nav.SelectSingleNode("//ms:CondicionVenta", ns).InnerText;

            @invoice.ConditionSaleType = ValorEnumCondictionSale(valor);
            if (@invoice.ConditionSaleType.Equals(FacturaElectronicaCondicionVenta.Credito))
            {
                @invoice.CreditTerm = int.Parse(nav.SelectSingleNode("//ms:PlazoCredito", ns).InnerText);
                @invoice.ExpirationDate = @invoice.DueDate.AddDays(@invoice.CreditTerm);
            }
            
        }

        public void ValidateFax(XmlDocument xmlDoc)
        {
            if (xmlDoc.SelectSingleNode("//Emisor//Fax")==null)
            {
                XmlNode doc = xmlDoc.SelectSingleNode("//Emisor");
                string xsi = "http://www.w3.org/2001/XMLSchema-instance";
                XmlNode fax = xmlDoc.CreateElement("Fax");
                XmlAttribute xsiNil = xmlDoc.CreateAttribute("nil", xsi);
                XmlNode telefono = xmlDoc.SelectSingleNode("//Telefono");
                xsiNil.Value = "true";
                fax.Attributes.Append(xsiNil);
                doc.InsertAfter(fax,telefono);
            }
            
        }

        public void DataReceptor(XmlDocument xmlDoc, Client @client)
        {
            XmlElement nav = xmlDoc.DocumentElement;
            XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);
            ns.AddNamespace("ms", nav.NamespaceURI);
            
            @client.NameComercial = nav.SelectSingleNode("//ms:Receptor//ms:Nombre", ns) != null ?
                                     nav.SelectSingleNode("//ms:Receptor//ms:Nombre", ns).InnerText : "";

            @client.Name = nav.SelectSingleNode("//ms:Receptor//ms:Nombre", ns).InnerText;

            @client.IdentificationType = (IdentificacionTypeTipo)int.Parse(nav.SelectSingleNode("//ms:Receptor//ms:Identificacion//ms:Tipo", ns).InnerText);

            @client.Identification = nav.SelectSingleNode("//ms:Receptor//ms:Identificacion//ms:Numero", ns).InnerText;

            @client.IdentificacionExtranjero = (IdentificacionTypeTipo)int.Parse(nav.SelectSingleNode("//ms:Receptor//ms:Identificacion//ms:Tipo", ns).InnerText) == IdentificacionTypeTipo.NoAsiganda ?
                                                nav.SelectSingleNode("//ms:Receptor//ms:Identificacion//ms:Numero", ns).InnerText : "";

            @client.PhoneNumber = nav.SelectSingleNode("//ms:Receptor//ms:Telefono", ns) != null ?
                                  nav.SelectSingleNode("//ms:Receptor//ms:Telefono//ms:CodigoPais", ns).InnerText +
                                  "-" + nav.SelectSingleNode("//ms:Receptor//ms:Telefono//ms:NumTelefono", ns).InnerText : "";
            if(nav.SelectSingleNode("//ms:Receptor//ms:Fax", ns) != null)
            {
                @client.MobilNumber = nav.SelectSingleNode("//ms:Receptor//ms:Fax//ms:CodigoPais", ns).InnerText +
                                      "-" + nav.SelectSingleNode("//ms:Receptor//ms:Fax//ms:NumTelefono", ns).InnerText;
            }            

            @client.Email = nav.SelectSingleNode("//ms:Receptor//ms:CorreoElectronico", ns) != null ? 
                            nav.SelectSingleNode("//ms:Receptor//ms:CorreoElectronico", ns).InnerText : "";

            if (nav.SelectSingleNode("//ms:Receptor//ms:Ubicacion", ns) != null)
            {
                if (nav.SelectSingleNode("//ms:Receptor//ms:Ubicacion//ms:Barrio", ns) != null &&
                    nav.SelectSingleNode("//ms:Receptor//ms:Ubicacion//ms:Distrito", ns) != null)
                {
                    Barrio barrio = new Barrio
                    {
                        Id = int.Parse(nav.SelectSingleNode("//ms:Receptor//ms:Ubicacion//ms:Barrio", ns).InnerText),
                        DistritoID = int.Parse(nav.SelectSingleNode("//ms:Receptor//ms:Ubicacion//ms:Distrito", ns).InnerText)
                    };
                    @client.BarrioId = barrio.Id;
                    @client.Barrio = barrio;
                    @client.Address = nav.SelectSingleNode("//ms:Receptor//ms:Ubicacion//ms:OtrasSenas", ns) != null ?
                                      nav.SelectSingleNode("//ms:Receptor//ms:Ubicacion//ms:OtrasSenas", ns).InnerText : "";
                }
            }
        }

        public void ArmarEstructuraPDF(string path, Tenant tenant, Invoice @invoice, Client @client, List<FacturaElectronicaMedioPago> listMedioPago)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(path);

            TipoDocumento documento = DeterminarTipoDocumento(xmlDoc);
            @invoice.TypeDocument = TypeDocument(documento);
            XmlElement nav = xmlDoc.DocumentElement;
            XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);
            ns.AddNamespace("msbld", nav.NamespaceURI);

            DataNodoParents(ns, nav, @invoice);
            
            if (xmlDoc.GetElementsByTagName("Receptor").Count > 0)
            {
                DataReceptor(xmlDoc, @client);
            }

            ListadoTipoPago(xmlDoc, ns, nav, listMedioPago);
            ObtenerDetalleInvoice(xmlDoc, @invoice, tenant);
            ObtenerResumen(ns,nav, @invoice);  
            ReGenerateQrFromVoucherKey();
            @invoice.QRCode = QRCode;
        }

        private bool ValidateItemChildrenXml(DataRow row,string children)
        {
            foreach (var ta in row.Table.Columns)
            {
                if (ta.ToString().Equals(children))
                    return true;
            }
            return false;
        }

        private bool ValidateItemParentsXml(DataSet ds, string parents)
        {
            foreach (DataTable tabla in ds.Tables)
            {
                if (tabla.TableName.Equals(parents))
                    return true;
            }
            return false;
        }

        public void ListadoTipoPago(XmlDocument xmlDoc, XmlNamespaceManager ns, XmlElement nav, List<FacturaElectronicaMedioPago> listMedioPago)
        {
            ns.AddNamespace("msbld", nav.NamespaceURI);

            //Seleccionar todos los nodos que coinciden
            XmlElement elementoPadre = xmlDoc.DocumentElement;
            XmlNodeList nodeList = elementoPadre.SelectNodes("//msbld:MedioPago", ns);

            //bucle en cada uno
            foreach (XmlNode nodo in nodeList)
            {
                FacturaElectronicaMedioPago medioPago = ValorEnumMedioPago(nodo.InnerText);
                listMedioPago.Add(medioPago);
            }
        }

        public void ObtenerResumen(XmlNamespaceManager ns, XmlElement nav, Invoice @invoice)
        {
            ns.AddNamespace("msbld", nav.NamespaceURI);

            @invoice.CodigoMoneda = (FacturaElectronicaResumenFacturaCodigoMoneda)Enum
                               .Parse(typeof(FacturaElectronicaResumenFacturaCodigoMoneda),
                               nav.SelectSingleNode("//msbld:CodigoMoneda", ns).InnerText, true);

            @invoice.TotalGravado = nav.SelectSingleNode("//msbld:ResumenFactura//msbld:TotalGravado", ns) != null ?
                                    Decimal.Parse(nav.SelectSingleNode("//msbld:ResumenFactura//msbld:TotalGravado", ns).InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture) : 0;

            @invoice.TotalExento = Decimal.Parse(nav.SelectSingleNode("//msbld:ResumenFactura//msbld:TotalExento", ns).InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            @invoice.DiscountAmount = Decimal.Parse(nav.SelectSingleNode("//msbld:ResumenFactura//msbld:TotalDescuentos", ns).InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

            @invoice.TotalTax = nav.SelectSingleNode("//msbld:ResumenFactura//msbld:TotalImpuesto", ns) != null ?
                                Decimal.Parse(nav.SelectSingleNode("//msbld:ResumenFactura//msbld:TotalImpuesto", ns).InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture) : 0;

            @invoice.Total = Decimal.Parse(nav.SelectSingleNode("//msbld:ResumenFactura//msbld:TotalComprobante", ns).InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        }


        public void ObtenerDetalleInvoice(XmlDocument xmlDoc, Invoice @invoice, Tenant tenant)
        {
            XmlNodeList detalleServicio = xmlDoc.GetElementsByTagName("DetalleServicio");
            XmlNodeList lineaDetalle = ((XmlElement)detalleServicio[0]).GetElementsByTagName("LineaDetalle");
            int line = 1;
            decimal rate = 0;
            decimal impuestoMonto = 0;
            foreach (XmlElement l in lineaDetalle)
            {
                XmlNodeList cantidadNodo = l.GetElementsByTagName("Cantidad");
                XmlNodeList unidMedidaNodo = l.GetElementsByTagName("UnidadMedida");
                XmlNodeList detalleNodo = l.GetElementsByTagName("Detalle");
                XmlNodeList precioUnitarioNodo = l.GetElementsByTagName("PrecioUnitario");
                XmlNodeList montoTotalNodo = l.GetElementsByTagName("MontoTotal");

                XmlNodeList impuestoMontoNodo;
                XmlNodeList impuestoTarifaNodo;
                XmlNodeList montoDescuentoNodo;
                XmlNodeList naturalezaDescuentoNodo;

                XmlNodeList subTotal = l.GetElementsByTagName("SubTotal");

                XmlNodeList impuestoNodo = l.GetElementsByTagName("Impuesto");
                if (impuestoNodo.Count > 0)
                {
                    impuestoMontoNodo = ((XmlElement)impuestoNodo[0]).GetElementsByTagName("Monto");
                    impuestoMonto = Decimal.Parse(impuestoMontoNodo[0].InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                    impuestoTarifaNodo = ((XmlElement)impuestoNodo[0]).GetElementsByTagName("Tarifa");
                    rate = Decimal.Parse(impuestoTarifaNodo[0].InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                }

                XmlNodeList montoTotalLineanodo = l.GetElementsByTagName("MontoTotalLinea");

                decimal precio = Decimal.Parse(precioUnitarioNodo[0].InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                decimal montoDescuento = 0;
                if (l.GetElementsByTagName("MontoDescuento").Count > 0)
                {
                    montoDescuentoNodo = l.GetElementsByTagName("MontoDescuento");
                    naturalezaDescuentoNodo = l.GetElementsByTagName("NaturalezaDescuento");
                    decimal descuento = Decimal.Parse(montoDescuentoNodo[0].InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                    montoDescuento = ( descuento / precio) * 100;
                }

                string detalle = detalleNodo[0].InnerText;
                decimal cantidad = Decimal.Parse(cantidadNodo[0].InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                Tax tax = Tax.Create(rate);

                @invoice.AssignInvoiceLine(TenantId, precio, impuestoMonto, montoDescuento, "", "", detalle, cantidad, LineType.Service, null, null, @invoice, line++, tax, null, UnidadMedidaType.Unid, "");
            }
        }

        public static FacturaElectronicaCondicionVenta ValorEnumCondictionSale(string xml)
        {
            FacturaElectronicaCondicionVenta condicionVenta = FacturaElectronicaCondicionVenta.Otros;
            switch (xml)
            {
                case "01":
                    condicionVenta = FacturaElectronicaCondicionVenta.Contado;
                    break;
                case "02":
                    condicionVenta = FacturaElectronicaCondicionVenta.Credito;
                    break;
                case "03":
                    condicionVenta = FacturaElectronicaCondicionVenta.Consignacion;
                    break;
                case "04":
                    condicionVenta = FacturaElectronicaCondicionVenta.Apartado;
                    break;
                case "05":
                    condicionVenta = FacturaElectronicaCondicionVenta.Arrendamiento_Opcion_de_Compra;
                    break;
                case "06":
                    condicionVenta = FacturaElectronicaCondicionVenta.Arrendamiento_Funcion_Financiera;
                    break;
            }

            return condicionVenta;
        }

        public static FacturaElectronicaMedioPago ValorEnumMedioPago(string xml)
        {
            FacturaElectronicaMedioPago medioPago = FacturaElectronicaMedioPago.Otros;
            switch (xml)
            {
                case "01":
                    medioPago = FacturaElectronicaMedioPago.Cheque;
                    break;
                case "02":
                    medioPago = FacturaElectronicaMedioPago.Tarjeta;
                    break;
                case "03":
                    medioPago = FacturaElectronicaMedioPago.Cheque;
                    break;
                case "04":
                    medioPago = FacturaElectronicaMedioPago.Transferencia_Deposito_Bancario;
                    break;
                case "05":
                    medioPago = FacturaElectronicaMedioPago.Recaudado_Terceros;
                    break;
            }

            return medioPago;
        }

        public void AssignBarrioDistrict(Barrio barrio, Distrito distrito, Client @client)
        {
            if (barrio.NombreBarrio != null)
            {
                barrio.Distrito = distrito;
                barrio.DistritoID = distrito.Id;
                @client.Barrio = barrio;
                @client.BarrioId = barrio.Id;
            }
            else
            {
                barrio = null;
                @client.Barrio = barrio;
            }

        }

        public void SetInvoiceXML(Uri XML)
        {
            if (XML != null)
            {
                ElectronicBill = XML.ToString();
            }
            //if (PDF != null)
            //{
            //    ElectronicBillPDF = PDF.ToString();
            //}
        }

        public void ChangeVoucherKit(string path, string voucherKey, Tenant tenant, Certificate certificate)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlElement nav = xmlDoc.DocumentElement;

            XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);
            ns.AddNamespace("msbld", nav.NamespaceURI);
            XmlNode clave = nav.SelectSingleNode("//msbld:Clave", ns);
            clave.InnerText = voucherKey;

            DeleteFile(path);

            CreateXML(tenant, certificate, VoucherKey, xmlDoc);
        }

        public TypeDocumentInvoice TypeDocument(TipoDocumento tipoDocumento)
        {
            TypeDocumentInvoice typeDocumentInvoice = new TypeDocumentInvoice();

            switch (tipoDocumento)
            {
                case TipoDocumento.FacturaElectronica:
                    {
                        typeDocumentInvoice = TypeDocumentInvoice.Invoice;
                        break;
                    }
                case TipoDocumento.NotaDebitElectronica:
                    {
                        typeDocumentInvoice = TypeDocumentInvoice.NotaDebito;
                        break;
                    }
                case TipoDocumento.NotaCreditoElectronica:
                    {
                        typeDocumentInvoice = TypeDocumentInvoice.NotaCredito;
                        break;
                    }
                case TipoDocumento.TiqueteElectronico:
                    {
                        typeDocumentInvoice = TypeDocumentInvoice.Ticket;
                        break;
                    }
            }

            return typeDocumentInvoice;
        }

        public enum NodoXml
        {
            [Description("Clave")]
            Clave,
            [Description("NumeroConsecutivo")]
            NumeroConsecutivo,
            [Description("FechaEmision")]
            FechaEmision,
            [Description("Normativa")]
            Normativa
        }

        public enum TipoDocumento
        {
            [Description("Factura Electrónica")]
            FacturaElectronica,
            [Description("Nota de débito electrónica")]
            NotaDebitElectronica,
            [Description("Nota de crédito electrónica")]
            NotaCreditoElectronica,
            [Description("Tiquete electrónico")]
            TiqueteElectronico,
            [Description("Comprobante emitido en contingencia")]
            ComprobanteEmitidoContingencia,
        }

        public enum TypeDocumentElectronic
        {
            [Description("Factura Electrónica")]
            FacturaElectronica,
            [Description("Nota de débito electrónica")]
            NotaDebitElectronica,
            [Description("Nota de crédito electrónica")]
            NotaCreditoElectronica,
            [Description("Tiquete electrónico")]
            TiqueteElectronico,
            [Description("Emisor")]
            Emisor,
            [Description("Identificacion")]
            Identificacion,
            [Description("Ubicacion")]
            Ubicacion,
            [Description("Telefono")]
            Telefono,
            [Description("Receptor")]
            Receptor,
            [Description("Fax")]
            Fax,
            [Description("MedioPago")]
            MedioPago,
            [Description("DetalleServicio")]
            DetalleServicio,
            [Description("LineaDetalle")]
            LineaDetalle,
            [Description("Codigo")]
            Codigo,
            [Description("Impuesto")]
            Impuesto,
            [Description("ResumenFactura")]
            ResumenFactura,
            [Description("Normativa")]
            Normativa
        }
    }

}
