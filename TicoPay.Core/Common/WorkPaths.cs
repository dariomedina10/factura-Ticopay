using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace TicoPay.Core.Common
{
    public static class WorkPaths
    {
        private static string _WorkPathRoot;
        static WorkPaths()
        {
            InitializeWorkPathRoot();
        }

        private static void InitializeWorkPathRoot()
        {
            string workPath = ConfigurationManager.AppSettings["WorkPath"];
            if (!string.IsNullOrWhiteSpace(workPath))
            {
                _WorkPathRoot = workPath;
            }
            else
            {
                _WorkPathRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "TicoPay");
            }

            if (!Directory.Exists(_WorkPathRoot))
            {
                try
                {
                    Directory.CreateDirectory(_WorkPathRoot);
                }
                catch (Exception)
                {
                    _WorkPathRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "TicoPay");
                    Directory.CreateDirectory(_WorkPathRoot);
                }
            }
        }

        private static string GetOrCreateDirectory(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            string path = Path.Combine(_WorkPathRoot, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetXmlPath()
        {
            return GetOrCreateDirectory("XML");
        }

        public static string GetCertifiedPath()
        {
            return GetOrCreateDirectory("Uploads");
        }

        public static string GetXmlSignedPath()
        {
            return GetOrCreateDirectory("XMLSigned");
        }

       
        public static string GetXmlReceivedPath()
        {
            return GetOrCreateDirectory("XMLReceived");
        }

        public static string GetPdfPath()
        {
            return GetOrCreateDirectory("PDF");
        }

        public static string GetQRPath()
        {
            return GetOrCreateDirectory("QR");
        }

        public static string GetXmlSignedClientPath()
        {
            return GetOrCreateDirectory("XMLSignedClient");
        }

        public static string GetXmlClientPath()
        {
            return GetOrCreateDirectory("XMLClient");
        }

    }
}