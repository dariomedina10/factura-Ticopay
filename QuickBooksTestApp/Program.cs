using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicopayUniversalConnectorService.ConexionesExternas.QuickBooks;
using TicopayUniversalConnectorService.ConexionesExternas.QuickBooks.Dto;

namespace QuickBooksTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string Opcion = "";
            string Config = "";
            bool Ejecutando = true;
            //Console.WriteLine(" Token asignado: ");
            //Console.WriteLine("Este Token tiene una duracion de 20 min, para extenderlo utilize la funcion RefreshToken");
            //Console.ReadKey();
            Console.Clear();
            Console.WriteLine("Introduzca la dirección del archivo de compañía, Ejemplo C:\\Users\\Public\\Documents\\Intuit\\QuickBooks\\Company Files\\Ticopay Quickbooks.qbw");
            Config = Console.ReadLine();
            FileLog errorLog = new FileLog();
            while (Ejecutando)
            {                
                Console.Clear();
                Console.WriteLine("Archivo de compañía seleccionado : " + Config);
                Console.WriteLine("Seleccione Operación a Realizar: ");
                Console.WriteLine("1: Consultar Clientes ");
                Console.WriteLine("2: Consultar Invoices ");
                Console.WriteLine("3: Consultar CreditMemos ");
                Console.WriteLine("S: Salir ");
                Opcion = Console.ReadKey().KeyChar.ToString();
                if (Opcion.ToUpper().Contains("1"))
                {
                    try
                    {
                        Console.Clear();
                        QuickbooksEnterpriseDesktop Conexion = new QuickbooksEnterpriseDesktop("QuickBooksTestApp", Config, "Quick125");
                        List<QuickbooksClient> clientes = Conexion.BuscarClientes();
                        if (clientes != null)
                        {
                            foreach (QuickbooksClient item in clientes)
                            {
                                Console.WriteLine("Cliente : " + item.Name + " " + item.IdentificationType.ToString() + " " + item.Identification + " " +
                                    item.PhoneNumber + " " + item.Email + " " + item.CreditDays.ToString() + " " + item.Address);
                            }
                        }                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error " + ex.Message);
                        errorLog.NuevaLinea("Consultar Clientes : " + ex.Message);
                    } 
                    Console.ReadKey();
                }
                if (Opcion.ToUpper().Contains("2"))
                {
                    try
                    {
                        QuickbooksEnterpriseDesktop Conexion = new QuickbooksEnterpriseDesktop("QuickBooksTestApp", Config, "Quick125");
                        List<QuickbooksInvoice> facturasCredito = Conexion.BuscarfacturasCredito(DateTime.Now.AddDays(-2));
                        if (facturasCredito != null)
                        {
                            foreach (QuickbooksInvoice item in facturasCredito)
                            {
                                //Console.WriteLine("Cliente : " + item.Name + " " + item.IdentificationType.ToString() + " " + item.Identification + " " +
                                //    item.PhoneNumber + " " + item.Email + " " + item.CreditDays.ToString() + " " + item.Address);
                            }
                        }
                        List<QuickbooksInvoice> facturasContado = Conexion.BuscarfacturasContado(DateTime.Now.AddDays(-2));
                        Console.WriteLine("Exito");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error " + ex.Message);
                        errorLog.NuevaLinea("Consultar Facturas : " + ex.Message);
                    }
                    Console.ReadKey();
                }
                if (Opcion.ToUpper().Contains("3"))
                {
                    try
                    {
                        QuickbooksEnterpriseDesktop Conexion = new QuickbooksEnterpriseDesktop("QuickBooksTestApp", Config, "Quick125");
                        List<QuickbooksNote> notas = Conexion.BuscarNotas(DateTime.Now.AddDays(-2));
                        if (notas != null)
                        {
                            foreach (QuickbooksNote item in notas)
                            {
                                //Console.WriteLine("Cliente : " + item.Name + " " + item.IdentificationType.ToString() + " " + item.Identification + " " +
                                //    item.PhoneNumber + " " + item.Email + " " + item.CreditDays.ToString() + " " + item.Address);
                            }
                        }
                        Console.WriteLine("Exito");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error " + ex.Message);
                        errorLog.NuevaLinea("Consultar CreditMemos : " + ex.Message);
                    }
                    Console.ReadKey();
                }
                if (Opcion.ToUpper().Contains("S"))
                {
                    Ejecutando = false;
                }
            }
            Console.Clear();
            Console.WriteLine("Hasta luego ");
            Console.ReadKey();
        }
    }
}
