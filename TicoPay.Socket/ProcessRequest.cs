using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ticopay.BNModel;
using log4net;
using TicoPay.MultiTenancy;
using TicoPay.Invoices;
using TicoPay.Clients;
using TicoPay.Invoices.Dto;
using TicoPay.Clients.Dto;


namespace TicoPay.Socket
{
    /// <summary>
    /// Procesa las tramas enviadas por el banco y devuelve la trama con la respuesta esperada
    /// </summary>
    class ProcessRequest
    {
        private const int PageSize = 10;
        private static ILog _loggerForWeService;
        /// <summary>
        /// Procesa las tramas enviadas por el banco segun el tipo de mensaje
        /// </summary>
        /// <param name="trama">Trama envidada por el banco</param>
        /// <param name="facturaService">Servicios de facturas consultas a la base de datos</param>
        /// <param name="previstaService">Servicios de previstas consultas a la base datos</param>
        /// /// <param name="asadaService">Servicios de asadas consultas a la base datos</param>
        /// <param name="puerto">Purto de la Asada a la que el banco quiere hacer la peticion</param>
        /// <returns>Trama con la respuesta acorde al tipo de request</returns>
        public static string Request(string trama, invoiceService facturaService, clientService clientService, tenantService tenantService, int puerto)
        {
            var agreementConectivity = tenantService.GetTenantsByPort(puerto).FirstOrDefault(); // **consulta los tenant q estan en el convenio

            var listTenant = (new List<int> { agreementConectivity.Id });
            //int asadasid = asada.AsadaId; //**aqui deberia ser una lista 

            _loggerForWeService = SocketLogManager.GetLogger();


            Console.WriteLine("Procesando trama del Banco Nacional....... Para la empresa: {0}, en el Puerto: {1}", listTenant[0], puerto);
            Console.WriteLine("Procesando trama del Banco Nacional....... Para los Tenant asociados en el Puerto: {0}", puerto);
            _loggerForWeService.Debug(String.Format("Procesando trama del Banco Nacional....... Para los Tenant asociados en el Puerto: {0}", puerto));
            if (!string.IsNullOrEmpty(trama))
            {
                int mensaje = BNConectividad.GetMensaje(trama);
                Console.WriteLine("Tipo del Mensaje en Trama: {0}", mensaje);

                switch (mensaje)
                {
                    case 200: //Trama N° 1:  Consulta de recibos ***** Trama N°3:  Aplicación del pago.
                        return ConsultaAplicacionPago(trama, facturaService, clientService, tenantService, listTenant);
                    case 420: //Trama N°5:  Reversión de pagos.
                        return ReversionPagoBN(trama, facturaService, clientService, tenantService, listTenant);
                    case 800: //Trama N° 11: Consulta de disponibilidad de servicio ( “Echo”)
                        return ConsultaDisponibilidadServicio(trama);
                    case 500: //Trama N°7:  Aplicación de notas de crédito (depósito) y débito (comisión).
                        return AplicacionNotaCreditoDebitoBN(trama);// dviquez: no se esta usando

                    //*****TRAMAS PARA EL ESQUEMA PAGO AUTOMÁTICO DE RECIBOS (PAR)*****
                    case 100: //Trama # 1 : Afiliación – Desafiliación de servicios  para Pago Automático de Recibos
                        return AfiliacionDesafiliacionBN(trama, clientService, tenantService, listTenant);
                    case 140: //Trama # 5 : Calendarización Por Vencimiento De Recibos
                        return CalendarizacionVencimientos(trama, tenantService, facturaService, listTenant);
                }
            }
            return string.Empty; //En caso de que el mensaje no corresponda a ninguno de los anteriores se devuelve un string vacio
        }

        /****************** Codigo para soporte de Trama N° 1:  Consulta de recibos ***** Trama N°3:  Aplicación del pago *******************************/
        /// <summary>
        /// Procesa las tramas de consulta y aplicacion de pago
        /// </summary>
        /// <param name="trama">Trama envidada por el banco</param>
        /// <param name="facturaService">Servicios de facturas consultas a la base de datos</param>
        /// <param name="previstaService"> </param>
        /// <param name="asadaService"> </param>
        /// <param name="asada"> </param>
        /// <returns>Trama con la respuesta segun el request del Banco</returns>
        static string ConsultaAplicacionPago(string trama, invoiceService facturaService, clientService clientService, tenantService tenantService, List<int> listTenant)
        {
            int codigoTransaccion = -1;
            codigoTransaccion = BNConectividad.GetCodigoTransaccion(trama); // Obtiene el codigo de transaccion el cual indica cual es el tipo de consulta, 800000 = Aplicacion de Pago o 310000 = Consulta de Recibo 

            Console.WriteLine("Codigo de transaccion: {0}", codigoTransaccion);
            switch (codigoTransaccion)
            {
                case 310000:
                    return ConsultaReciboTrama(trama, facturaService, clientService, tenantService, listTenant);
                case 800000:
                    return AplicacionPagoBN(trama, facturaService, clientService, tenantService, listTenant);       //<----------------------------------------
            }
            return string.Empty; // Si el codigo de transaccion no corresponde ninguno de los anteriores se retorna una cadena vacia
        }

        /// <summary>
        /// Procesa la consulta de Recibo (Trama N° 1:  Consulta de recibos.)
        /// </summary>
        /// <param name="trama">Trama enviada por el Banco</param>
        /// <param name="facturaService">Servicios de Factura, Business Layer</param>
        /// <param name="previstaService"> </param>
        /// <param name="asadaService"> </param>
        /// <param name="asada"> </param>
        /// <returns>Trama con la consulta del servicio</returns>
        static string ConsultaReciboTrama(string trama, invoiceService facturaService, clientService clientService, tenantService tenantService, List<int> listTenant)
        {
            try
            {
                Console.WriteLine("Consulta Recibo de Pago");
                _loggerForWeService.Debug("Consulta Recibo de Pago");
                var consultaRecibo = Ticopay.BNModel.ConsultaRecibo.ParserConsultaRecibo(trama); // Serializa la trama al objeto ConsultaRecibo
                return ConsultaRecibo(facturaService, clientService, tenantService, consultaRecibo, listTenant);//Para conectividad por numero de Cuenta  <----------------------------------------
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Consula los recibos pendientes de la prevista (por numero de cuenta)
        /// </summary>
        /// <param name="facturaService">Servicios de facturas consultas a la base de datos</param>
        /// <param name="previstaService"> </param>
        /// <param name="asadaService"> </param>
        /// <param name="consultaRecibo">Trama enviada por el banco serializada</param>
        /// <param name="asada"> </param>
        /// <returns>Respuesta con los recibos pendientes segun el numero de cuenta</returns>
        static string ConsultaRecibo(invoiceService facturaService, clientService clientService, tenantService tenantService, ConsultaRecibo consultaRecibo, List<int> listTenant)
        {
            var respuestaConsultaRecibo = new RespuestaConsultaRecibo(consultaRecibo);
            ClientBN client;
            try
            {
                TipoLLaveAcceso tipo = tenantService.GetTenantTipoAcceso(listTenant);
                client = clientService.GetExistClientByCode((MultiTenancy.TipoLLaveAcceso)tipo,long.Parse(consultaRecibo.LlaveAcceso));
                if (client == null)
                {
                    Console.WriteLine("Error, El Código del Cliente no existe");
                    respuestaConsultaRecibo.CodigoRespuesta = 3; //Por favor llamar a la asada
                    return respuestaConsultaRecibo.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error, procesando la consulta de facturas pendientes por favor llamar a TicoPay\n Exception: {0}", ex);
                respuestaConsultaRecibo.CodigoRespuesta = 1; //Por favor llamar a la asada
                return respuestaConsultaRecibo.ToString();
            }

            try
            {
                if (client!=null) 
                {
                    var facturas = facturaService.GetInvoicesPendingPay(client); //obtiene las facturas del cliente pendientes por pago
                    if (facturas != null && facturas.Count > 0)
                    {
                        Console.WriteLine("Consultando cliente con Codigo: {0}", client.Code);
                        
                        string str = client.Name + ((!string.IsNullOrEmpty(client.LastName)) ? " " + client.LastName : "");
                        if (str.Length > 48)
                            str = str.Substring(0, 48);

                        respuestaConsultaRecibo.NombreCliente = str;
                        respuestaConsultaRecibo.CantidadServicios = facturas.Count(); //???

                        var servicioPendiente = new ServicioPendiente { LlaveAcceso = consultaRecibo.LlaveAcceso };
                        foreach (var factura in facturas)
                        {
                            var recibo = new Recibo
                                             {
                                                 Monto = (double)factura.Balance,
                                                 NumeroFactura = factura.Number, 
                                                 Verificador = 0,
                                                 Vencimiento = factura.DueDate,
                                                 Periodo = factura.DueDate
                                             };
                            servicioPendiente.Recibos.Add(recibo);
                        }
                        respuestaConsultaRecibo.Servicios.Add(servicioPendiente);
                    }
                    else
                    {
                        Console.WriteLine("El cliente con codigo {0} no tiene facturas pendientes", client.Code);
                        respuestaConsultaRecibo.CodigoRespuesta = 2; // El servicio no tiene recibos pendientes
                    }
                }
                else
                {
                    Console.WriteLine("El cliente con codigo {0} no existe", client.Code);
                    respuestaConsultaRecibo.CodigoRespuesta = 3; // Si el numero de cuenta no existe, por efecto la respuesta a retornar es 3 (El servicio no existe)
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ufff hubo un error procesando la consulta de facturas pendientes por favor llamar a TicoPay\n Exception: {0}", ex.ToString());
                respuestaConsultaRecibo.CodigoRespuesta = 1; // Si algo sale mal, un error no controlado, Por favor llamar a la ASADA

            }
            return respuestaConsultaRecibo.ToString();
        }


        /// <summary>
        /// Procesa la Trama N°3:  Aplicación del pago. Realiza el pago de la factura
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <param name="facturaService">Servicios de factura, como BD</param>
        /// <param name="previstaService"> </param>
        /// <param name="asadaService"> </param>
        /// <param name="asada"> </param>
        /// <returns>Trama de respuesta</returns>
        static string AplicacionPagoBN(string trama, invoiceService facturaService, clientService clientService, tenantService tenantService, List<int> listTenant)
        {
            Console.WriteLine("Procesando Aplicacion de Pago......");
            _loggerForWeService.Debug("Consulta Aplicacion de Pago");
            var aplicacionPago = AplicacionPago.ParserAplicacionPago(trama);
            var respuestaAplicacionPago = new RespuestaPago(aplicacionPago);

            ClientBN client;
            try
            {
                TipoLLaveAcceso tipo = (TipoLLaveAcceso)tenantService.GetTenantTipoAcceso(listTenant);
                client = clientService.GetExistClientByCode((MultiTenancy.TipoLLaveAcceso)tipo, long.Parse(aplicacionPago.ValorServicio));
                if (client == null)
                {
                    Console.WriteLine("El número de codigo del Cliente no existe!");
                    respuestaAplicacionPago.CodigoRespuesta = 3; //Por favor llamar a la asada
                    return respuestaAplicacionPago.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error procesando el pago de la factura por favor llamar a TicoPay \n Exception: {0}", ex.ToString());
                respuestaAplicacionPago.CodigoRespuesta = 1; //Por favor llamar a la asada
                return respuestaAplicacionPago.ToString();
            }

            try
            {
                Console.WriteLine("Procesando pago de la factura #{0} con numero de cuenta# {1}", aplicacionPago.NumeroFactura, client.Code);
                var factura = facturaService.GetInvoicesByNumber(client, aplicacionPago.NumeroFactura); //Obtiene la factura segun el numero de cuenta y el numero de factura

                if (factura != null)
                {
                    string str = client.Name + ((!string.IsNullOrEmpty(client.LastName)) ? " " + client.LastName : "");
                    if (str.Length > 48)
                        str = str.Substring(0, 48);

                    respuestaAplicacionPago.NombreCliente = str;

                    var facturasPendientes = facturaService.GetInvoicesPendingOld(client, factura.DueDate); 
                    if (facturasPendientes.Count() > 0)
                    {
                        if (facturasPendientes.First().Number != aplicacionPago.NumeroFactura)
                        {
                            Console.WriteLine("Error al procesar la factura #{0} con número de cuenta #{1} , existen facturas de meses anteriores.", factura.Number, client.Code);
                            _loggerForWeService.Debug(string.Format("Error al procesar la factura #{0} con número de cuenta #{1} , existen facturas de meses anteriores.", factura.Number, client.Code));
                            respuestaAplicacionPago.CodigoRespuesta = 1;
                            return respuestaAplicacionPago.ToString();
                        }
                    }

                    //if (facturasPendientes.First() == null)// si es null esta mal porque se supone que esta pagando una factura que existe
                    //{
                    //    Console.WriteLine("Error al procesar la factura #{0} con número de cuenta #{1}", factura.Number, client.Code);
                    //    _loggerForWeService.Debug(string.Format("Error al procesar la factura #{0} con número de cuenta #{1}", factura.Number, client.Code));
                    //    respuestaAplicacionPago.CodigoRespuesta = 1;
                    //    return respuestaAplicacionPago.ToString();
                    //}

                    if (!EstaPagada(factura)) //Se debe validar que la factura no haya sido pagada
                    {
                        _loggerForWeService.Debug(string.Format("Pagando Factura No {0}, Cuenta {1}, del tenant {2}", factura.Number, client.Code, client.TenantId));
                        int result = facturaService.PayInvoiceBn(factura,aplicacionPago.CodigoAgencia,trama); // Paga la factura

                        if (result == 0) // Si el resultado es 0 significa que la factura fue pagada todo salio bien
                        {
                            Console.WriteLine("La factura #{0} con numero de cuenta #{1} fue pagada exitosamente", aplicacionPago.NumeroFactura, client.Code);
                            _loggerForWeService.Debug(string.Format("Factura No {0} Pagada Exitosamente de la empresa {1}", factura.Number, client.TenantId));
                            /*Rubros de Pago*/
                            respuestaAplicacionPago.Monto = (double)factura.Balance;
                            if (factura.Balance > 0)
                                respuestaAplicacionPago.Rubros.Add(new Rubro(1,
                                                                             RespuestaBNConectivdad.GetMontoConDecimales((double)factura.Balance, 18).ToString(
                                                                                 CultureInfo.InvariantCulture)));
                        }
                        else if (result == -1)
                        {
                            Console.WriteLine("La factura #{0} con numero de cuenta #{1}, error al pagar la factura", aplicacionPago.NumeroFactura, client.Code);
                            _loggerForWeService.Debug(string.Format("Error -1 al pagar la factura {0} del tenant {1}", factura.Number, client.TenantId));
                            respuestaAplicacionPago.CodigoRespuesta = 1; // Por favor llamar a la asada
                        }
                        //else if (result == -2)
                        //{
                        //    Console.WriteLine("La factura #{0} con numero de cuenta #{1} ya esta pago, no se puede pagar 2 veces", aplicacionPago.NumeroFactura, client.Code);
                        //    _loggerForWeService.Debug(string.Format("Error -2 al pagar la factura {0} de la asada {1}", factura.Number, client.TenantId));
                        //    respuestaAplicacionPago.CodigoRespuesta = 5; //El recibo ya esta pago
                        //}
                    }
                    else
                    {
                        Console.WriteLine("La factura #{0} con numero de cuenta #{1} ya esta pago, no se puede pagar 2 veces", aplicacionPago.NumeroFactura, client.Code);
                        _loggerForWeService.Debug(string.Format("Error al pagar la factura {0} del tenant {1}, la factura esta pagada", factura.Number, client.TenantId));
                        respuestaAplicacionPago.CodigoRespuesta = 5; //El recibo ya fue pagada
                    }
                }
                else
                {
                    respuestaAplicacionPago.CodigoRespuesta = 6; // Si el recibo no existe, posiblemente es por que el recibo ya esta pago
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error procesando el pago de un recibo por favor llamar a TicoPay \n Exception: {0}", ex.ToString());
                _loggerForWeService.Debug(string.Format("Error General al pagar la factura {0} de la empresa {1}", aplicacionPago.NumeroFactura, client.TenantId));
                respuestaAplicacionPago.CodigoRespuesta = 1; // Por favor llamar a la asada
            }
            return respuestaAplicacionPago.ToString();
        }

        static private bool EstaPagada(InvoicePendingPayBN factura)
        {
            if (factura.Status == Status.Completed)
                return true;
            return false;
        }

        /**************************************************** Fin *************************************************************/


        /****************** Codigo para soporte de Trama N°5:  Reversión de pagos. ***** Trama N°6:  Respuesta a reversión de pagos. *******************************/

        /// <summary>
        /// Procesa la solicitud del banco de revertir algun recibo en especifico
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <param name="facturaService">Servicios de factura, como base de datos</param>
        /// <param name="previstaService"> </param>
        /// <param name="asadaService"> </param>
        /// <param name="asadaid"> </param>
        /// <returns>Trama con la respuesta para el Banco Nacional</returns>
        static string ReversionPagoBN(string trama, invoiceService facturaService, clientService clientService, tenantService tenantService, List<int> listTenant)
        {
            Console.WriteLine("Procesando reversion de pago......");
            _loggerForWeService.Debug("Procesando reversion de pago......");
            //TODO: Negociar los codigos de respuesta con el BN
            var reversion = ReversionPago.ParserReversionPago(trama); // Convierte la trama en un objeto de tipo ReversionPago
            var respuestaReversion = new RespuestaReversionPago(reversion);
            try
            {
                //int cuenta = -1;
                ClientBN client;
                try
                {
                    TipoLLaveAcceso tipo = (TipoLLaveAcceso)tenantService.GetTenantTipoAcceso(listTenant);
                    //cuenta = clientService.ReturnCuentaDeacuerdoTipo(tipo, reversion.ValorServicio, listTenant);
                    client = clientService.GetExistClientByCode((MultiTenancy.TipoLLaveAcceso)tipo, long.Parse(reversion.ValorServicio));
                    if (client == null)
                    {
                        Console.WriteLine("El número de codigo del Cliente no existe!");
                        respuestaReversion.CodigoRespuesta = 3; //Por favor llamar a la asada
                        return respuestaReversion.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("uff hubo un error consultando al Cliente en la la reversion de pago por favor llamar a TicoPay \n Exception: {0}", ex.ToString());
                    respuestaReversion.CodigoRespuesta = 1; //Por favor llamar a la asada
                    return respuestaReversion.ToString();
                }

                var factura = facturaService.GetInvoicesByNumber(client,reversion.NumeroFactura);
                _loggerForWeService.Debug("-->NoFactura:" + reversion.NumeroFactura);
                _loggerForWeService.Debug("-->Codigo Cliente:" + client.Code);

                if (factura != null && (EstaPagada(factura))) //Verificar que la factura este pagada
                {
                    var fechapago = new DateTime(factura.PaymentDate.Value.Year, factura.PaymentDate.Value.Month, factura.PaymentDate.Value.Day);

                    var fechareversion = new DateTime(reversion.FechaTransaccion.Year, reversion.FechaTransaccion.Month, reversion.FechaTransaccion.Day);

                    if (factura.PaymentDate!= null && fechapago.Equals(fechareversion))
                    {
                        facturaService.ReverseInvoice(factura, "Reversado por el Banco Nacional");//ojo esto mandar el numero de cuenta y la asada
                        Console.WriteLine("La factura #{0} del Cliente con Id #{1} fue revertido exitosamente", factura.Number, client.Code);
                    }
                    else
                    {
                        Console.WriteLine("La factura #{0} no fue cancelada en la misma fecha que se intenta hacer la reversion", factura.Number);
                        respuestaReversion.CodigoRespuesta = 7; // El recibo no fue cancelado en la misma fecha de la reversion
                    }
                }
                else
                {
                    if ((factura != null) && (!EstaPagada(factura)))
                    { 
                        Console.WriteLine("La factura #{0} del Cliente con Id #{1} no esta pagado para ser revertido", factura.Number, client.Code);
                        respuestaReversion.CodigoRespuesta = 4; //El recibo no esta pagado
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("uff hubo un error procesando la reversion de pago por favor llamar a TicoPay \n Exception: {0}", ex.ToString());
                respuestaReversion.CodigoRespuesta = 1; //Por favor llamar a la asada
            }
            return respuestaReversion.ToString();
        }

        /**************************************************** Fin *************************************************************/

        /****************** Codigo para soporte de Trama N°7:  Aplicación de notas de crédito (depósito) y débito (comisión). Trama N°8:  Respuesta a aplicación de notas de crédito y débito.  *******************************/
        /// <summary>
        /// Aplicacion de notas de credito y debito
        /// </summary>
        /// <param name="trama">Trama enviada por el banco nacional</param>
        /// <returns>Trama con la respuesta al banco nacional</returns>
        static string AplicacionNotaCreditoDebitoBN(string trama)
        {
            Console.WriteLine("Procesando Aplicacion de notas de debito y credito......");
            _loggerForWeService.Debug("Procesando Aplicacion de notas de debito y credito......");
            //TODO:ngonzalez --> Esta trama se debe guardar en la tramas para ser procesada despues, actualmente no se esta haciendo nada
            var aplicacionNotaCD = AplicacionNotaCreditoDebito.ParserAplicacionNotaCD(trama);
            var respuesta = new RespuestaAplicacionNotaCreditoDebito(aplicacionNotaCD);
            return respuesta.ToString();
        }

        /**************************************************** Fin *************************************************************/

        /****************** Codigo para soporte de Trama N° 11: Consulta de disponibilidad de servicio ( “Echo”). Trama N° 12: Respuesta a consulta de disponibilidad de servicio (Echo)  *******************************/
        /// <summary>
        /// le responde al banco estoy vivo y esperando
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>respuesta al banco</returns>
        static string ConsultaDisponibilidadServicio(string trama)
        {
            Console.WriteLine("Procesando Disponibilidad de servicio......");
            _loggerForWeService.Debug("Procesando Disponibilidad de servicio......");
            var disponibilidad = DisponibilidadServicio.ParserDisponibilidadServicio(trama);
            var respuestaDisponibilidad = new RespuestaDisponibilidadServicio(disponibilidad);
            return respuestaDisponibilidad.ToString();
        }

        /**************************************************** Fin *************************************************************/

        /****************** Codigo para soporte de Trama # 1 : Afiliación – Desafiliación de servicios  para Pago Automático de Recibos. Trama # 2 : Respuesta: Afiliación - Desafiliación  *******************************/

        /// <summary>
        /// Afilia un numero de cuenta al pago automatico del banco nacional
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <param name="previstaService">Servicios de prevista</param>
        /// <param name="asadaService"> </param>
        /// <param name="asadaid"> </param>
        /// <returns>Respuesta al BN</returns>
        static string AfiliacionDesafiliacionBN(string trama, clientService clientService, tenantService tenantService, List<int> listTenant)
        {
            var afiliacion = AfiliacionDesafiliacion.ParserAfiliacionDesafiliacion(trama);
            var respuestaAfiliacion = new RespuestaAfiliacionDesafiliacion(afiliacion);
            ClientBN client;
            try
            {
                TipoLLaveAcceso tipo = (TipoLLaveAcceso)tenantService.GetTenantTipoAcceso(listTenant);
                client = clientService.GetExistClientByCode((MultiTenancy.TipoLLaveAcceso)tipo, long.Parse(afiliacion.LlaveServicio));

                if (client == null)
                {
                    Console.WriteLine("El número de codigo del cliente no existe!");
                    respuestaAfiliacion.CodigoRespuesta = 3; //Por favor llamar a la asada
                    return respuestaAfiliacion.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error procesando la afiliacion desafiliacion de pagos automaticos por favor llamar a TicoPay \n Exception: {0}", ex);
                respuestaAfiliacion.CodigoRespuesta = 3; //Por favor llamar a la asada
                return respuestaAfiliacion.ToString();
            }

            try
            {
                //var prevista = clientService.GetPrevistaByContrato(cuenta, "Abonado, Sector");

                if (client != null)
                {
                    string str = client.Name + ((!string.IsNullOrEmpty(client.LastName)) ? " " + client.LastName : "");
                    if (str.Length > 48)
                        str = str.Substring(0, 48);

                    respuestaAfiliacion.NombreCliente = str;


                    if (afiliacion.CodigoTransaccion == 100000) //Afiliacion
                    {
                        Console.WriteLine("Procesando Afiliacion de pago automatico de recibos......");
                        _loggerForWeService.Debug(string.Format("Afiliando servicio: Cliente - {0} - Codigo {1}", client.Name, client.Code));
                        respuestaAfiliacion.CodigoTransaccion = 100100;

                        //TODO: Segun conversé con oscar alfaro del BN el dia 12 diciembre, el dia de pago el banco no lo esta usando, esta usando siempre al vencimiento, por ende siempre vamos a retornar 0 en dia de pago, por eso comento las siguientes 2 lineas, en caso de cambiar esto se deben descomentar las siguientes 2 lineas y eliminar la siguiente
                        //if (afiliacion.FormaPago == 0) //Fecha Fija 
                        //    respuestaAfiliacion.DiaPagoCiclo = CalcularDiaVencimiento(prevista.Sector.DiaInicio, prevista.Sector.MargenDiasCobro);
                        //else if (afiliacion.FormaPago == 1)// Al vencimiento
                        //    respuestaAfiliacion.DiaPagoCiclo = afiliacion.DiaPago;

                        respuestaAfiliacion.DiaPagoCiclo = afiliacion.DiaPago;
                        respuestaAfiliacion.MontoPromedio = 0;
                        client.PagoAutomaticoBn = true;
                        client.DiaPagoBn = respuestaAfiliacion.DiaPagoCiclo;
                        client.MontoMaximoBn = afiliacion.MontoMaximo;
                        client.FormaPagoBn = (TicoPay.Clients.FormaPago)afiliacion.FormaPago; //1 = Vencimiento, 0 = Dia Fijo
                        clientService.UpdateClientBn(client);
                    }
                    else if (afiliacion.CodigoTransaccion == 100001) // Desafiliacion
                    {
                        Console.WriteLine("Procesando Desafiliacion de pago automatico de recibos......");
                        _loggerForWeService.Debug(string.Format("Desafiliando servicio: Cliente - {0} - Codigo {1}", client.Name, client.Code));
                        respuestaAfiliacion.CodigoTransaccion = 100101;
                        respuestaAfiliacion.MontoPromedio = 0;
                        client.PagoAutomaticoBn = false;
                        client.DiaPagoBn = 0;
                        client.MontoMaximoBn = 0;
                        client.FormaPagoBn = 0;
                        clientService.UpdateClientBn(client);
                    }
                    else if (afiliacion.CodigoTransaccion == 100002) // Consulta
                    {
                        Console.WriteLine("Procesando consulta de pago automatico de recibos......");
                        respuestaAfiliacion.CodigoTransaccion = 100102;
                        respuestaAfiliacion.DiaPagoCiclo = 0;
                        respuestaAfiliacion.MontoPromedio = 0;
                    }
                    Console.WriteLine("La afiliacion, desfialiacion o consulta con numero de cuenta #{0} fue realizada con exito", afiliacion.LlaveServicio);
                }
                else
                {
                    Console.WriteLine("Error el numero de cuenta #{0} no existe", afiliacion.LlaveServicio);
                    respuestaAfiliacion.CodigoRespuesta = 3; //El numero de cuenta no existe
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error procesando la afiliacion desafiliacion de pagos automaticos por favor llamar a la asada \n Exception: {0}", ex.ToString());
                respuestaAfiliacion.CodigoRespuesta = 1; //Por favor llamar a la ASADA
            }
            return respuestaAfiliacion.ToString();
        }


        /**************************************************** Fin *************************************************************/

        /****************** Codigo para soporte de Trama # 5 : Calendarización Por Vencimiento De Recibos  ----   Trama # 6 : Respuesta Fechas Vencimiento Recibos   *******************************/

        /// <summary>
        /// Forma la trama de respuesta al BN con los datos de facturas que deben ser cobradas en la fecha actual (hoy) por concepto de pago automatico de recibos (PAR)
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <param name="asadaService"> </param>
        /// <param name="facturaService">Servicios de factura Base de Datos</param>
        /// <param name="asada">Asada a la que el banco quiere hacer la peticion</param>
        /// <returns></returns>
        static string CalendarizacionVencimientos(string trama, tenantService tenantService, invoiceService facturaService, List<int> listTenant)
        {
            Console.WriteLine("Procesando Calendarizacion de vencimientos...");
            _loggerForWeService.Debug("Procesando Calendarizacion de vencimientos...");
            var calendarizacion = CalendarizacionVencimientoRecibo.ParserCalendarizacionVencimientos(trama); // Serializa la trama de calendarizacion en un objeto de tipo CalendarizacionVencimientoRecibo
            var respuestaCalendarizacion = new RespuestaFechaVencimientoRecibos(calendarizacion); // Prepara la base de calendarizacion para la respuesta al BN
            try
            {
                // aqui se toma el primer tenant xq pueden ser varios
                int index = CacheHelper.Exists(listTenant[0].ToString(CultureInfo.InvariantCulture)) ? CacheHelper.Get(listTenant[0].ToString(CultureInfo.InvariantCulture)) : 0;
                bool indicador = false;
                var facturas = facturaService.GetInvoicesPendingPayPAR(index, listTenant, PageSize, out indicador); //Obtiene todas las facturas de las previstas con pago automatico activado segun la fecha actual (hoy)

                if (indicador)
                {
                    if (CacheHelper.Exists(listTenant[0].ToString(CultureInfo.InvariantCulture)))
                        CacheHelper.Update(CacheHelper.Get(listTenant[0].ToString(CultureInfo.InvariantCulture)) + 1, listTenant[0].ToString(CultureInfo.InvariantCulture));//actualiza página
                    else
                        CacheHelper.Add(1, listTenant[0].ToString(CultureInfo.InvariantCulture));// si no existe la página es uno
                }
                else
                {
                    if (CacheHelper.Exists(listTenant[0].ToString(CultureInfo.InvariantCulture)))
                        CacheHelper.Clear(listTenant[0].ToString(CultureInfo.InvariantCulture));
                }

                if (facturas.Count > 0)
                {
                    TipoLLaveAcceso tipo = (TipoLLaveAcceso)tenantService.GetTenantTipoAcceso(listTenant);

                    respuestaCalendarizacion.Indicador = indicador ? 1 : 0; // Si hay mas de 10 facturas indicar que existen mas facturas que deben ser procesadas se indica con un 1, de lo contrario indicar con un 0 que no hay mas facturas a ser procesadas
                    int count = 0;
                    foreach (var factura in facturas) //Enviar solo las primeras 10 facturas de la lista, segun las reglas del banco solo se pueden enviar 10 facturas por turno
                    {
                        if (count == 10) // Verificar que solo se envien 10 facturas como maximo
                            break;

                        var registro = new Registro();

                        switch (tipo)
                        {
                            case TipoLLaveAcceso.Codigo_Cliente:
                                registro.LlaveServicio = factura.Code.Value.ToString();
                                break;
                            //case TipoLlaveAcceso.Medidor:
                            //    registro.LlaveServicio = factura.Lectura.Prevista.Medidor.Codigo;
                            //    break;
                            //case TipoLlaveAcceso.CodigoExtra:
                            //    registro.LlaveServicio = factura.Lectura.Prevista.CodigoIdExtra;
                            //    break;
                        }

                        registro.Periodo = factura.DueDate;
                        registro.Monto = (double)factura.Balance;
                        registro.FechaVencimiento = factura.DueDate;
                        registro.NumeroFactura = factura.Number;
                        registro.Verificador = 0;
                        respuestaCalendarizacion.Registros.Add(registro); //Agrega el registro de factura a la lista
                        count++;
                    }
                }
                else
                {
                    Console.WriteLine("No hay mas recibos a ser cobrados el dia de hoy......");
                    respuestaCalendarizacion.Indicador = 0; //Indica que no hay mas facturas relacionadas a previstas con pago automatico de recibos PAR.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("uff hubo un error procesando la calendarizacion de vencimientos \n Exception: {0}", ex);
                respuestaCalendarizacion.CodigoRespuesta = 1; //TODO:ngonzalez preguntar por los codigos de respuesta en caso de que haya un error
            }
            return respuestaCalendarizacion.ToString();
        }
    }

    public enum TipoLLaveAcceso
    {
        Numero_Identificacion,
        Codigo_Cliente
    }
}
