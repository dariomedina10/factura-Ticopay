import { Component, OnInit, Input } from '@angular/core';
import { InvoiceService } from '../../shared/dataServices/invoice.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ICompleteInvoice, InvoiceSearchConfiguration, InvoiceStatus,
  PaymentType, ListPaymentType, Moneda, DocumentType, ConditionSaleType } from '../../Models/InvoiceModel';
import { NotificationsService } from '../../shared/notifications/notifications.service';
import { AuthService } from '../../shared/dataServices/auth-service.service';
import { LoaderService } from '../../shared/loader/loader.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';
import { CurrencyService } from '../../shared/currency.service';
import { ITenant } from '../../Models/Authentificatiomodel';
import { TicketsService } from '../../shared/dataServices/tickets.service';
import * as FileSaver from 'file-saver';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { IdentificationType } from '../../Models/ClientModel';

@Component({
  selector: 'app-invoice-detail',
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.css']
})
export class InvoiceDetailComponent implements OnInit {

  /*
  * Variables para el Control del Form
  */
  private _loading: boolean;
  private _invoice: ICompleteInvoice;
  private _ticketFormat: SafeUrl;
  private _facturaPendiente: boolean;
  private _procesando: boolean;
  private _currencyCode: string;
  private _tenant: ITenant;
  private _dataImpresion: string;
  private dtOptions: any;
  public tipoDocumento = DocumentType;


  /*
 * Variables para el Control del Invoice
 */
  public paymentType = PaymentType;
  private _invoicePaymentCash: boolean;
  private _invoicePaymentCreditCard: boolean;
  private _invoicePaymentCheck: boolean;
  private _invoicePaymentDeposit: boolean;
  private _invoiceAmountCash: number;
  private _invoiceamountCreditCard: number;
  private _invoiceAmountCheck: number;
  private _invoiceAmountDeposit: number;
  private _invoiceTransNumberCreditCard: string;
  private _invoiceTransNumberCheck: string;
  private _invoiceTransNumberDeposit: string;
  private _invoiceMoneyType: Moneda;

  constructor(private router: Router, private invoiceService: InvoiceService, private route: ActivatedRoute
    , protected notificationsService: NotificationsService, private authService: AuthService,
    private loader: LoaderService, private _currency: CurrencyService, private ticketService: TicketsService
    ,private sanitizer: DomSanitizer) {
    this._invoice = null;
    this._dataImpresion = null;
    this.opcionesLenguajeTabla();
  }

  /*
  * Método para realizar el reenvió de la factura por email
  */
  private resendInvoice() {
    if ( this._invoice.ClientEmail !== undefined) {
      let arrayfacturas = new Array<string>();
      arrayfacturas.push(this._invoice.id);
      this.loader.show();
      this.authService.RefreshToken()
        .subscribe(tokenActive => {
          if (tokenActive) {
            // Request al servidor
            this.invoiceService.resendEmail(arrayfacturas)
              .subscribe((response: boolean) => {
                const enviado = response;
                this.loader.hide();
                if (enviado) {
                  this.notificationsService.addInfo('Factura reenviada al correo ' + this._invoice.ClientEmail);
                } else {
                  this.notificationsService.addError('La factura no puedo ser enviada por correo,'
                  + ' cheque la dirección de correo electrónico');
                }
              }, error => console.log(error)
              );
            // Fin de los Request
          } else {
            const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
            this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
          }
        },
        serviceError => console.log(serviceError));
    }
  }

  /*
  * Método para pagar las facturas pendientes
  */
  payInvoice(payInvoiceForm: NgForm) {
    if (payInvoiceForm && payInvoiceForm.valid) {
      let totalPaid = 0;
      if (this._invoicePaymentCash) {
        totalPaid = totalPaid + this._invoiceAmountCash;
      }
      if (this._invoicePaymentCreditCard) {
        totalPaid = totalPaid + this._invoiceamountCreditCard;
      }
      if (this._invoicePaymentCheck) {
        totalPaid = totalPaid + this._invoiceAmountCheck;
      }
      if (this._invoicePaymentDeposit) {
        totalPaid = totalPaid + this._invoiceAmountDeposit;
      }
      if ((Math.round(this._invoice.total * 100) / 100) === totalPaid) {
        this._procesando = true;
        this.loader.show();
        this.authService.RefreshToken()
          .subscribe(tokenActive => {
            if (tokenActive) {
              // Request al servidor

              // Fin de los Request
            } else {
              const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
              this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
            }
          },
          serviceError => console.log(serviceError));
      } else {
        this.notificationsService.addWarning('Debe pagar el monto exacto de la factura para poder continuar');
      }
    }
  }

  /*
  * Método que recopila los tipos de pago y los agrupa para agregarlos a la entidad de Invoice
  */
  private ListPayment(): ListPaymentType[] {
    const listPayment = new Array<ListPaymentType>();
    if (this._invoicePaymentCash) {
      const type = new ListPaymentType();
      type.TypePayment = Math.round(PaymentType.Cash * 100) / 100;
      type.Balance = this._invoiceAmountCash;
      type.Trans = null;
      listPayment.push(type);
    }
    if (this._invoicePaymentCreditCard) {
      const type = new ListPaymentType();
      type.TypePayment = Math.round(PaymentType.CreditCard * 100) / 100;
      type.Balance = this._invoiceamountCreditCard;
      type.Trans = this._invoiceTransNumberCreditCard;
      listPayment.push(type);
    }
    if (this._invoicePaymentCheck) {
      const type = new ListPaymentType();
      type.TypePayment = PaymentType.Check;
      type.Balance = Math.round(this._invoiceAmountCheck * 100) / 100;
      type.Trans = this._invoiceTransNumberCheck;
      listPayment.push(type);
    }
    if (this._invoicePaymentDeposit) {
      const type = new ListPaymentType();
      type.TypePayment = PaymentType.Deposit;
      type.Balance = Math.round(this._invoiceAmountDeposit * 100) / 100;
      type.Trans = this._invoiceTransNumberDeposit;
      listPayment.push(type);
    }
    return listPayment;
  }

  /*
  * Método para asignar el monto a pagar a efectivo
  */
  private invoicePaymentCashChange() {
    if (this._invoicePaymentCash === true) {
      if ((this._invoicePaymentCheck === false) && (this._invoicePaymentCreditCard === false)
        && (this._invoicePaymentDeposit === false)) {
        this._invoiceAmountCash = Math.round(this._invoice.total * 100) / 100;
      }
    } else {
      this._invoiceAmountCash = null;
    }
  }

  /*
  * Método para asignar el monto a pagar a tarjeta
  */
  private invoicePaymentCreditCardChange() {
    if (this._invoicePaymentCreditCard === true) {
      if ((this._invoicePaymentCheck === false) && (this._invoicePaymentCash === false)
        && (this._invoicePaymentDeposit === false)) {
        this._invoiceamountCreditCard = Math.round(this._invoice.total * 100) / 100;
      }
    } else {
      this._invoiceamountCreditCard = null;
    }
  }

  /*
  * Método para asignar el monto a pagar a cheque
  */
  private invoicePaymentCheckChange() {
    if (this._invoicePaymentCheck === true) {
      if ((this._invoicePaymentCreditCard === false) && (this._invoicePaymentCash === false)
        && (this._invoicePaymentDeposit === false)) {
        this._invoiceAmountCheck = Math.round(this._invoice.total * 100) / 100;
      }
    } else {
      this._invoiceAmountCheck = null;
    }
  }

  /*
  * Método para asignar el monto a pagar a deposito
  */
  private invoicePaymentDepositChange() {
    if (this._invoicePaymentDeposit === true) {
      if ((this._invoicePaymentCreditCard === false) && (this._invoicePaymentCash === false)
        && (this._invoicePaymentCheck === false)) {
        this._invoiceAmountDeposit = Math.round(this._invoice.total * 100) / 100;
      }
    } else {
      this._invoiceAmountDeposit = null;
    }
  }

  /*
  * Método para solicitar la reimpresión de la factura
  */
  private printTicket() {
   this.authService.RefreshToken()
   .subscribe(tokenActive => {
     if (tokenActive) {
       // Request al servidor
         this.invoiceService.GetPrinterData(this._invoice.id)
         .subscribe(invoiceData => {          
           if(invoiceData != undefined && invoiceData != null) {
            this._dataImpresion = "com.fidelier.printfromweb://" + invoiceData;            
            this._ticketFormat = this.sanitizer.bypassSecurityTrustUrl(this._dataImpresion);
            // console.log(this._ticketFormat);
           } else {
            this._dataImpresion = null;
           }            
         },
         serviceError => this.notificationsService.addError(<any>serviceError));
       // Fin de los Request
     } else {
       const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
       this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
     }
   }); 
    //console.log(textoTicket);
    //this._ticketFormat = this.sanitizer.bypassSecurityTrustUrl(textoTicket);
  }

  /*
  * Metodo para solicitar el pdf de la factura
  */
  private downloadPdf() {
    this.ticketService.getInvoicePDF(this._invoice.id, this._invoice.consecutiveNumber)
                    .subscribe((responsePdf: Blob) => {
                    }, errori => console.log(errori)
                    );
  }

   /*
   * Método para enviar de regreso a la búsqueda de facturas
   */
  private returnToIndex() {
    this.router.navigate(['/home/invoiceIndex']);
  }

  /*
  * Método para setear el lenguaje del datatable
  */
  private opcionesLenguajeTabla() {
    this.dtOptions = {
      searching: false,
      paging: false,
      language: {
        processing: 'Procesando...',
        lengthMenu: 'Mostrar _MENU_ registros',
        zeroRecords: 'No se encontraron resultados',
        emptyTable: 'Ningún dato disponible en esta tabla',
        info: 'Mostrando un total de _TOTAL_ registros',
        infoEmpty: 'Mostrando un total de 0 registros',
        infoFiltered: '(filtrado de un total de _MAX_ registros)',
        infoPostFix: '',
        search: 'Buscar:',
        url: '',
        thousands: ',',
        loadingRecords: 'Cargando...',
        paginate: {
          first: 'Primero',
          last: 'Último',
          next: 'Siguiente',
          previous: 'Anterior'
        },
        aria: {
          sortAscending: ': Activar para ordenar la columna de manera ascendente',
          sortDescending: ': Activar para ordenar la columna de manera descendente'
        }
      },
      responsive: true
    };
  }

  ngOnInit() {
    this._invoicePaymentCash = false;
    this._invoicePaymentCreditCard = false;
    this._invoicePaymentCheck = false;
    this._invoicePaymentDeposit = false;
    if (localStorage.getItem('tenant')) {
      // Obtiene la configuración del Tenant
      this._tenant = JSON.parse(localStorage.getItem('tenant'));
      // this._currencyCode = this._currency.getCurrencyName(this._tenant.codigoMoneda);
      this._currencyCode = this._tenant.currencyCode;
      this._invoiceMoneyType = Moneda[this._tenant.currencyCode];
    }
    this._loading = true;
    if (!this._invoice) {
      this.route.params.subscribe(params => {
        const id = params.id;
        let searchRequest = new InvoiceSearchConfiguration();
        searchRequest.EndDueDate = null;
        searchRequest.ClientId = null;
        searchRequest.StartDueDate = null;
        searchRequest.Status = null;
        searchRequest.InvoiceId = null;
        if (id) {
          // Buscar la Factura para asignarla a la vista
          searchRequest.InvoiceId = id;
          this.authService.RefreshToken()
            .subscribe(tokenActive => {
              if (tokenActive) {
                // Cargar los clientes a la lista
                this.invoiceService.getInvoices(searchRequest)
                  .subscribe(invoices => {
                    this._facturaPendiente = false;
                    this._invoice = invoices[0];
                    this._invoice.simboloMoneda = Moneda [this._invoice.codigoMoneda];                    
                    if(this._tenant.isPos){
                      this.printTicket();
                    }
                    // console.log(this._invoice);
                    this._loading = false;
                    if (this._invoice.status === InvoiceStatus.Pendiente) {
                      // this._facturaPendiente = true;
                    }
                  },
                  serviceError => this.notificationsService.addError(<any>serviceError));
              } else {
                const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
                this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
              }
            },
            serviceError => this.notificationsService.addError(<any>serviceError));
        }
      });
    }
  }

}
