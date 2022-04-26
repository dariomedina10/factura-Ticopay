using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using TicoPay.Socket.BL;
using TicoPay.Invoices;
using TicoPay.Clients;
using TicoPay.MultiTenancy;
using Abp.Domain.Repositories;
using TicoPay.Services;
using Abp.Domain.Uow;
using Abp;
using Abp.Dependency;


namespace TicoPay.Socket
{
    class ListenPorts
    {
        private static ManualResetEvent allDone = new ManualResetEvent(false);
       // private static readonly IKernel Kernel = new StandardKernel(new NinjectDependencyResolver());  
        private static ILog _loggerForWeService;

        System.Net.Sockets.Socket[] scon;
        IPEndPoint[] ipPoints;
        internal ListenPorts(IPEndPoint[] ipPoints)
        {
            this.ipPoints = ipPoints;
            scon = new System.Net.Sockets.Socket[ipPoints.Length];
        }

        public void BeginListen()
        {
            _loggerForWeService = SocketLogManager.GetLogger();

            //IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); ; //new

            for (int i = 0; i < ipPoints.Length; i++)
            {
                scon[i] = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //scon[i] = new System.Net.Sockets.Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //new
                scon[i].Bind(ipPoints[i]);
            }
            ThreadListen(scon);
        }

        public void ThreadListen(System.Net.Sockets.Socket[] objs)
        {
            try
            {
                int x = 0;
                    while (true)
                    {
                        try
                        {
                            // Set the event to nonsignaled state.
                            allDone.Reset();
                            // Start an asynchronous socket to listen for connections.
                            foreach (var listener in objs)
                            {
                                listener.Listen(100);
                                Console.WriteLine("Esperando una conexion o request al puerto {0}...", ((IPEndPoint)listener.LocalEndPoint).Port);
                                _loggerForWeService.Debug("Esperando una conexion o request al puerto " + ((IPEndPoint)listener.LocalEndPoint).Port + "...");
                                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                                // Wait until a connection is made before continuing.
                            }
                            _loggerForWeService.Debug("Esperando por una conexión..");

                            allDone.WaitOne();
                        }
                        catch (Exception ex)
                        {
                            _loggerForWeService.Debug("Esperando por una conexión.. " + ex);
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            if (x > 60)
                            {
                                long memory = GC.GetTotalMemory(false);
                                if (memory > 52428800)
                                {
                                    _loggerForWeService.Debug("Liberando Memoria.. " + memory);
                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                    GC.Collect();
                                    _loggerForWeService.Debug("Memoria Resultante.. " + GC.GetTotalMemory(false));
                                }
                                x = 0; 
                            }
                            else
                            {
                                x++;
                            }
                        }
                    }
               // }
            }
            catch (SocketException ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Signal the main thread to continue.
                allDone.Set();

                // Get the socket that handles the client request.
                System.Net.Sockets.Socket listener = (System.Net.Sockets.Socket)ar.AsyncState;
                System.Net.Sockets.Socket handler = listener.EndAccept(ar);

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.ReceiveTimeout = 35000;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public static void ReadCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                System.Net.Sockets.Socket handler = state.workSocket;
                int port = ((IPEndPoint)handler.LocalEndPoint).Port;

                using (var bootstrapper = AbpBootstrapper.Create<SocketModule>())
                {

                    var serviceTenant = IocManager.Instance.Resolve<tenantService>();
                    var serviceFactura = IocManager.Instance.Resolve<invoiceService>();
                    var serviceClient = IocManager.Instance.Resolve<clientService>();
               
                    SocketError errorCode;
                    int bytesRead = handler.EndReceive(ar, out errorCode);
                    if (errorCode != SocketError.Success)
                    {
                        bytesRead = 0;
                    }

                    if (bytesRead > 0)
                    {
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                        content = state.sb.ToString();
                        Console.WriteLine("--------------------------------------------------------------------------------------------");
                        Console.WriteLine("Nuevo Request, Fecha: {0}", DateTimeZone.Now().ToString(CultureInfo.InvariantCulture));
                        Console.WriteLine("{0} Bytes Leídos desde Socket. Request Entrante: \n Data : {1}", content.Length, content);

                        _loggerForWeService.Debug("--------------------------------------------------------------------------------------------");
                        _loggerForWeService.Debug(String.Format("Nuevo Request, Fecha: {0}", DateTimeZone.Now().ToString(CultureInfo.InvariantCulture)));
                        _loggerForWeService.Debug(String.Format("{0} Bytes Leídos desde Socket. Request Entrante: \n Data : {1}", content.Length, content));

                        Send(handler, ProcessRequest.Request(content, serviceFactura, serviceClient, serviceTenant, port));
                    }
                    else
                        Send(handler, string.Empty);

                    state.Dispose();

                    bootstrapper.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _loggerForWeService.Debug(e.ToString());
                _loggerForWeService.Debug(e.StackTrace);
            }

        }

        private static void Send(System.Net.Sockets.Socket handler, String data)
        {
            try
            {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                handler.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), handler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                System.Net.Sockets.Socket handler = (System.Net.Sockets.Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine(string.Format("{0} bytes Envíados al Cliente. Resquest de Salida: ", bytesSent));
                Console.WriteLine("--------------------------------------------------------------------------------------------");

                _loggerForWeService.Debug(String.Format("{0} bytes Envíados al Cliente. Resquest de Salida: ", bytesSent));
                _loggerForWeService.Debug(String.Format("--------------------------------------------------------------------------------------------"));

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
