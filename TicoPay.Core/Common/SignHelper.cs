using Microsoft.Xades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TicoPay.Core
{
    public class SignHelper
    {
        const string URI_XMLNS_DIGSIG_RSA_SHA256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        const string URI_XMLNS_DIGSIG_SHA256 = "http://www.w3.org/2001/04/xmlenc#sha256";
        const string URI_XML_EXE_C14 = "http://www.w3.org/2001/10/xml-exc-c14n#";
        const string URI_POLICY_IDENTIFIER = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4.1/Resolucion_Comprobantes_Electronicos_DGT-R-48-2016.pdf";
        const string POLICY_HASH = "NmI5Njk1ZThkNzI0MmIzMGJmZDAyNDc4YjUwNzkzODM2NTBiOWUxNTBkMmI2YjgzYzZjM2I5NTZlNDQ4OWQzMQ==";
        const string MIME_TYPE = "application/octet-stream"; 
        const string ENCODING = "UTF-8";

        public static XmlDocument SignDocument(XmlDocument document, X509Certificate2 cert)
        {
            XadesSignedXml xadesSignedXml = new XadesSignedXml(document);
            xadesSignedXml.SignedInfo.CanonicalizationMethod = URI_XML_EXE_C14;
            xadesSignedXml.SignedInfo.SignatureMethod = URI_XMLNS_DIGSIG_RSA_SHA256;

            Reference reference = new Reference
            {
                Uri = "",
                DigestMethod = URI_XMLNS_DIGSIG_SHA256
            };
            reference.Id = "r-id-1";
            reference.AddTransform(CreateXPathTransform("not(ancestor-or-self::Signature)"));
            reference.AddTransform(new XmlDsigExcC14NTransform());
            //reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            xadesSignedXml.AddReference(reference);

            RSACryptoServiceProvider rsaKey = SetSigningKey(cert);
            xadesSignedXml.SigningKey = rsaKey;

            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data((X509Certificate)cert));
            keyInfo.AddClause(new RSAKeyValue(rsaKey));
            xadesSignedXml.KeyInfo = keyInfo;

            string id = "id-" + Guid.NewGuid().ToString("N");
            xadesSignedXml.Signature.Id = id;

            XadesObject xadesObject = new XadesObject();
            xadesObject.QualifyingProperties.Target = "#" + id;
            xadesObject.QualifyingProperties.SignedProperties.Id = "xades-" + id;
            AddSignedSignatureProperties(document,
                reference,
                xadesObject.QualifyingProperties.SignedProperties.SignedSignatureProperties,
                xadesObject.QualifyingProperties.SignedProperties.SignedDataObjectProperties, cert);
            xadesSignedXml.AddXadesObject(xadesObject);

            var refe = xadesSignedXml.SignedInfo.References[1] as Reference;
            refe.AddTransform(new XmlDsigExcC14NTransform());
            refe.DigestMethod = URI_XMLNS_DIGSIG_SHA256;

            xadesSignedXml.ComputeSignature();
            xadesSignedXml.SignatureValueId = "value-" + xadesSignedXml.Signature.Id;

            document.DocumentElement.AppendChild(document.ImportNode(xadesSignedXml.GetXml(), true));

            return document;
        }

        private static XmlDsigXPathTransform CreateXPathTransform(string xpath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement xPathElem = doc.CreateElement("XPath");
            xPathElem.InnerText = xpath;
            XmlDsigXPathTransform xForm = new XmlDsigXPathTransform();
            xForm.LoadInnerXml(xPathElem.SelectNodes("."));
            return xForm;
        }

        private static RSACryptoServiceProvider SetSigningKey(X509Certificate2 certificate)
        {
            var key = (RSACryptoServiceProvider)certificate.PrivateKey;

            Type CspKeyContainerInfo_Type = typeof(CspKeyContainerInfo);

            FieldInfo CspKeyContainerInfo_m_parameters = CspKeyContainerInfo_Type.GetField("m_parameters", BindingFlags.NonPublic | BindingFlags.Instance);
            CspParameters parameters = (CspParameters)CspKeyContainerInfo_m_parameters.GetValue(key.CspKeyContainerInfo);

            var cspparams = new CspParameters(24, "Microsoft Enhanced RSA and AES Cryptographic Provider", key.CspKeyContainerInfo.KeyContainerName);
            cspparams.KeyNumber = parameters.KeyNumber;
            cspparams.Flags = parameters.Flags;
            var signingKey = new RSACryptoServiceProvider(cspparams);
            return signingKey;
        }

        public static XmlDocument LoadDocument(string docPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            if (File.Exists(docPath))
            {
                xmlDocument.PreserveWhitespace = true;
                xmlDocument.Load(docPath);
            }
            return xmlDocument;
        }

        private static void AddSignedSignatureProperties(XmlDocument document, Reference reference, SignedSignatureProperties signedSignatureProperties, SignedDataObjectProperties signedDataObjectProperties, X509Certificate2 selectedCertificate)
        {
            Cert cert = new Cert();
            cert.IssuerSerial.X509IssuerName = selectedCertificate.IssuerName.Name;
            cert.IssuerSerial.X509SerialNumber = GetSerialNumberAsDecimalString(selectedCertificate);
            cert.CertDigest.DigestMethod.Algorithm = SignedXml.XmlDsigSHA1Url;
            cert.CertDigest.DigestValue = selectedCertificate.GetCertHash();
            signedSignatureProperties.SigningCertificate.CertCollection.Add(cert);

            signedSignatureProperties.SignaturePolicyIdentifier.SignaturePolicyImplied = false;
            signedSignatureProperties.SignaturePolicyIdentifier.SignaturePolicyId.SigPolicyId.Identifier.IdentifierUri = URI_POLICY_IDENTIFIER;
            signedSignatureProperties.SignaturePolicyIdentifier.SignaturePolicyId.SigPolicyHash.DigestMethod.Algorithm = URI_XMLNS_DIGSIG_SHA256;
            signedSignatureProperties.SignaturePolicyIdentifier.SignaturePolicyId.SigPolicyHash.DigestValue = Convert.FromBase64String(POLICY_HASH);

            DataObjectFormat newDataObjectFormat = new DataObjectFormat();
            newDataObjectFormat.MimeType = MIME_TYPE;
            //newDataObjectFormat.Encoding = ENCODING;
            newDataObjectFormat.ObjectReferenceAttribute = "#" + reference.Id;

            signedDataObjectProperties.DataObjectFormatCollection.Add(newDataObjectFormat);
            signedSignatureProperties.SigningTime = DateTime.UtcNow;
        }

        private static string GetSerialNumberAsDecimalString(X509Certificate2 certificate)
        {
            List<int> dec = new List<int> { 0 };
            foreach (char c in certificate.SerialNumber)
            {
                int carry = Convert.ToInt32(c.ToString(), 16);
                for (int i = 0; i < dec.Count; ++i)
                {
                    int val = dec[i] * 16 + carry;
                    dec[i] = val % 10;
                    carry = val / 10;
                }
                while (carry > 0)
                {
                    dec.Add(carry % 10);
                    carry /= 10;
                }
            }
            var chars = dec.Select(d => (char)('0' + d));
            var cArr = chars.Reverse().ToArray();
            return new string(cArr);
        }
    }
}
