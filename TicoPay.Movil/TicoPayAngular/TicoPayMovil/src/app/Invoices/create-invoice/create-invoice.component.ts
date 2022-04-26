import { Component, OnInit, Injector, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { IClient, Client, IdentificationType } from '../../Models/ClientModel';
import { IInvoiceLines, InvoiceLines, Invoice, PaymentType, ListPaymentType, IInvoice,
  ICompleteInvoice, DiscountType, FirmType, Moneda, LineType, DocumentType} from '../../Models/InvoiceModel';
import { ClientsService } from '../../shared/dataServices/clients.service';
import { error } from 'util';
import { IService, UnitMeasurement } from '../../Models/ServiceModel';
import { ServicesService } from '../../shared/dataServices/services.service';
import { InvoiceService } from '../../shared/dataServices/invoice.service';
import { NotificationsService } from '../../shared/notifications/notifications.service';
import { LoaderService } from '../../shared/loader/loader.service';
import { AuthService } from '../../shared/dataServices/auth-service.service';
import { forEach } from '@angular/router/src/utils/collection';
import { DataTableDirective } from 'angular-datatables';
import { Input } from '@angular/core/src/metadata/directives';
import { isNumeric } from 'rxjs/util/isNumeric';
import { ITenant } from '../../Models/Authentificatiomodel';
import { CurrencyService } from '../../shared/currency.service';
import { ITaxes } from '../../Models/TaxesModel';
import { TaxesService } from '../../shared/dataServices/taxes.service';
import { IProduct } from '../../Models/ProductModel';
import { ProductsService } from '../../shared/dataServices/products.service';
import { IItem, Item, ItemType } from '../../Models/ItemModel';
import { TicketsService } from '../../shared/dataServices/tickets.service';
import * as FileSaver from 'file-saver';

@Component({
  templateUrl: './create-invoice.component.html',
  styleUrls: ['./create-invoice.component.css']
})
export class CreateInvoiceComponent implements OnInit {

  //#region Declaración de Variables
  /*
  * Variables para el control del formulario
  */
  private _procesando: boolean;
  private _filtroCliente: boolean;
  private _filtroItem: boolean;
  private _descuentoGlobal: boolean;
  private _currencyCode: string;
  private _tenant: ITenant;
  public firmType = FirmType;
  public moneda = Moneda;
  public unitMeasurement = UnitMeasurement;
  public identificationType = IdentificationType;
  public tipoDocumento = DocumentType;
  private _TenantFirmType: FirmType;
  private _invoiceFirmType: FirmType;
  private _invoiceMoneyType: Moneda;
  private _comprobanteType: DocumentType;

  /*
  * Variables para el Control y Filtrado de Clientes
  */
  private _filteredClients: IClient[];
  private _Clients: IClient[];
  private _clientFilter: string;
  private _clientErrorMessage: string;
  private _clientId: string;
  private _newClient: boolean;
  get clientFilter(): string {
    return this._clientFilter;
  }
  set clientFilter(value: string) {
    this._clientFilter = value;
  }
  /*
  * Variables para el Control y Filtrado de Servicios y productos
  */
  private _userSelectedService: boolean;
  // private dtOptions: DataTables.Settings;
  private dtOptions: any;
  private _taxes: ITaxes[];
  private _taxErrorMessage: string;
  // Items
  private _filteredItems: Item[];
  private _items: Item[];
  private _itemId: string;
  private _itemFilter: string;
  // Services
  private _serviceLock: boolean;
  // private _filteredServices: IService[];
  private _services: IService[];
  // private _serviceFilter: string;
  private _serviceNote: string;
  private _serviceErrorMessage: string;
  // private _serviceId: string;
  private _newService: boolean;
  // Products
  private _products: IProduct[];
  private _newProduct: boolean;
  // Lines
  private _selectedServices: InvoiceLines[];
  private _gdSelectedServices: InvoiceLines[];
  private _serviceQuantity: number;
  private _serviceDiscount: number;
  private _servicePrice: number;
  private _serviceTitle: string;
  private _serviceTax: string;
  private _serviceMeasurementUnit: UnitMeasurement;
  private _serviceMeasurementUnitOther: string;
  get itemFilter(): string {
    return this._itemFilter;
  }
  set itemFilter(value: string) {
    this._itemFilter = value;
  }
  /*
  * Variables para el Control del Invoice
  */
  public paymentType = PaymentType;
  // Condiciones de pago Contado o Credito
  private _invoicePaymentConditionsCredit: boolean;
  private _invoiceCreditDays: number;
  // Tipos de pago Efectivo , Tarjeta , Cheque o Deposito / Transferencia
  private _invoicePaymentCash: boolean;
  private _invoicePaymentCreditCard: boolean;
  private _invoicePaymentCheck: boolean;
  private _invoicePaymentDeposit: boolean;
  // Monto de cada tipo de pago y Referencia
  private _invoiceAmountCash: number;
  private _invoiceamountCreditCard: number;
  private _invoiceAmountCheck: number;
  private _invoiceAmountDeposit: number;
  private _invoiceTransNumberCreditCard: string;
  private _invoiceTransNumberCheck: string;
  private _invoiceTransNumberDeposit: string;
  // Totalización de factura sin descuento global
  private _invoiceSubTotal: number;
  private _invoiceTotal: number;
  private _invoiceTaxTotal: number;
  private _invoiceDiscountTotal: number;
  // Totalización de factura con descuento global
  private _gdInvoiceSubTotal: number;
  private _gdInvoiceTotal: number;
  private _gdInvoiceTaxTotal: number;
  private _gdInvoiceDiscountTotal: number;
  // Manejo de la entidad de factura
  private _invoiceErrorText: string;
  private _invoiceError: boolean;
  private _invoice = new Invoice();
  private _globalDiscount: number;
  // Id para el PDF
  private _sendInvoice: ICompleteInvoice;

  /*
  * Constructor de la clase
  */
  constructor(private router: Router, private clientService: ClientsService, private serviceService: ServicesService,
    private invoiceService: InvoiceService, protected notificationsService: NotificationsService,
    private loader: LoaderService, private authService: AuthService, private _currency: CurrencyService,
    private taxesService: TaxesService, private productService: ProductsService, private ticketService: TicketsService) {
    this._clientFilter = '';
    this._filteredClients = this._Clients;
    this._itemFilter = '';
    this._filteredItems = this._items;
    this._selectedServices = new Array<InvoiceLines>();
    this.opcionesLenguajeTabla();
    this._invoiceFirmType = FirmType.Llave;
    this._comprobanteType = DocumentType.Invoice;
    this._invoice.ClientIdentificationType = IdentificationType.CedulaFisica;
    this._sendInvoice = null;
    // this._invoiceMoneyType = Moneda.CRC;
  }

  //#endregion

  /*
  * Enviá la entidad Invoice al servicio de invoice para ser agregada a la bd
  */
  submitInvoiceInformation(createInvoiceForm: NgForm) {
    if (createInvoiceForm && createInvoiceForm.valid) {
      if (((this._invoiceTotal > 0) && (!this._globalDiscount)) ||
        ((this._gdInvoiceTotal > 0) && (this._globalDiscount))) {
        if (this._invoicePaymentConditionsCredit) {
          // Pago Crédito
          if (this._invoiceCreditDays > 0) {
            this._invoice.CreditTerm = this._invoiceCreditDays;
          } else {
            return this.notificationsService.addWarning('Debe ingresar la cantidad de dias de crédito para poder continuar');
          }
        } else {
          // Pago Contado
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
          if (((Math.round(this._invoiceTotal * 100) / 100) !== totalPaid) && (!this._descuentoGlobal)) {
            return this.notificationsService.addWarning('Debe pagar el monto exacto del comprobante para poder continuar');
          }
          if (((Math.round(this._gdInvoiceTotal * 100) / 100) !== totalPaid) && (this._descuentoGlobal)) {
            return this.notificationsService.addWarning('Debe pagar el monto exacto del comprobante para poder continuar');
          }
        }
        // Bloquear el botón y mostrar mensaje de procesando
        if(this._clientId == undefined && this._comprobanteType === DocumentType.Invoice) {
          return this.notificationsService.addWarning('Debe seleccionar un cliente para poder continuar');
        }
        this._invoice.ClientId = this._clientId;
        if (this._descuentoGlobal) {this._invoice
          // this._invoice.InvoiceLines = this._gdSelectedServices;
          this._invoice.InvoiceLines = this._selectedServices;
          this._invoice.DiscountGeneral = this._globalDiscount;
          this._invoice.TypeDiscountGeneral = DiscountType.Percentage;
        } else {
          this._invoice.InvoiceLines = this._selectedServices;
          this._invoice.DiscountGeneral = null;
          this._invoice.TypeDiscountGeneral = null;
        }
        if (this._invoicePaymentConditionsCredit) {
          this._invoice.ListPaymentType = null;
        } else {
          this._invoice.ListPaymentType = this.ListPayment();
        }
        this._procesando = true;
        this.loader.show();
        this._invoice.TipoFirma = this._invoiceFirmType;
        // console.log(this._invoiceMoneyType);
        this._invoice.CodigoMoneda = this._invoiceMoneyType;
        // console.log(this._invoice.CodigoMoneda);
        this._invoice.TypeDocument = this._comprobanteType;
        this.authService.RefreshToken()
          .subscribe(tokenActive => {
            if (tokenActive) {
              // Request al servidor
              if (this._comprobanteType === DocumentType.Invoice) {
                this.invoiceService.createInvoice(this._invoice)
                .subscribe((response: ICompleteInvoice) => {
                  const newInvoice = response;
                  this.loader.hide();
                  this._procesando = false;
                  if (newInvoice) {
                    // console.log('Factura Creada');
                    this.notificationsService.addroute('Factura generada correctamente con el número de documento '
                      + newInvoice.consecutiveNumber,
                      '/home/invoiceDetail/', newInvoice.id, 'Ver detalle de factura');
                    // Reiniciar el formulario
                    this._selectedServices = new Array<InvoiceLines>();
                    this._gdSelectedServices = new Array<InvoiceLines>();
                    this._invoiceSubTotal = 0;
                    this._invoiceDiscountTotal = 0;
                    this._invoiceTaxTotal = 0;
                    this._invoiceTotal = 0;
                    this._gdInvoiceSubTotal = 0;
                    this._gdInvoiceDiscountTotal = 0;
                    this._gdInvoiceTaxTotal = 0;
                    this._gdInvoiceTotal = 0;
                    this._descuentoGlobal = false;
                    // createInvoiceForm.reset();
                    this._clientId = undefined;
                    this._serviceQuantity = 1;
                    this._invoicePaymentCash = false;
                    this._invoicePaymentCheck = false;
                    this._invoicePaymentCreditCard = false;
                    this._invoicePaymentDeposit = false;
                    this._invoicePaymentConditionsCredit = false;
                    // Devuelve el nuevo cliente mediante un evento al Parent Component
                  } else {
                    // Error en la creación , mostrar un mensaje de error de validación
                    console.log('Fallo la creación de la factura');
                    // this.notificationsService.addError('Problemas en la creación de la factura');
                  }
                }, errori => console.log(errori)
                );
              } else {
                this.ticketService.createTicket(this._invoice)
                .subscribe((response: ICompleteInvoice) => {
                  const newInvoice = response;
                  this.loader.hide();
                  this._procesando = false;
                  this._sendInvoice = null;
                  if (newInvoice) {
                    // console.log('Factura Creada');
                    this.notificationsService.addroute('Tiquete electrónico generado correctamente con el número de documento '
                      + newInvoice.consecutiveNumber,
                      '/home/invoiceDetail/', newInvoice.id, 'Ver detalle del Tiquete electrónico');
                    this._sendInvoice = newInvoice;
                    // Reiniciar el formulario
                    this._invoice.ClientEmail = null;
                    this._invoice.ClientIdentification = null;
                    this._invoice.ClientIdentificationType = null;
                    this._invoice.ClientName = null;
                    this._selectedServices = new Array<InvoiceLines>();
                    this._gdSelectedServices = new Array<InvoiceLines>();
                    this._invoiceSubTotal = 0;
                    this._invoiceDiscountTotal = 0;
                    this._invoiceTaxTotal = 0;
                    this._invoiceTotal = 0;
                    this._gdInvoiceSubTotal = 0;
                    this._gdInvoiceDiscountTotal = 0;
                    this._gdInvoiceTaxTotal = 0;
                    this._gdInvoiceTotal = 0;
                    this._descuentoGlobal = false;
                    // createInvoiceForm.reset();
                    this._clientId = undefined;
                    this._serviceQuantity = 1;
                    this._invoicePaymentCash = false;
                    this._invoicePaymentCheck = false;
                    this._invoicePaymentCreditCard = false;
                    this._invoicePaymentDeposit = false;
                    this._invoicePaymentConditionsCredit = false;
                    /* // Descarga automatica de tiquete
                    this.ticketService.getInvoicePDF(this._sendInvoice.id, this._sendInvoice.consecutiveNumber)
                    .subscribe((responsePdf: Blob) => {
                    }, errori => console.log(errori)
                    );
                    */
                    // Devuelve el nuevo cliente mediante un evento al Parent Component
                  } else {
                    // Error en la creación , mostrar un mensaje de error de validación
                    console.log('Fallo la creación del tiquete');
                    // this.notificationsService.addError('Problemas en la creación de la factura');
                  }
                }, errori => console.log(errori)
                );
                }
              // Fin de los Request
            } else {
              const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
              this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
            }
          },
            serviceError => console.log(serviceError));
      } else {
        this.notificationsService.addWarning('Debe agregar servicios al comprobante para poder continuar');
      }
    }
  }

  /*
  * Método para asignar los dias de crédito de un cliente seleccionado
  */
  private onSelectedMoneyChange() {
    if (this._invoiceMoneyType !== undefined) {
      this._currencyCode = Moneda[this._invoiceMoneyType];
    }
  }

  /*
  * Método para calcular el descuento global de la factura
  */
  private onGlobalDiscountChange() {
    if (this._descuentoGlobal) {
      if (this._globalDiscount === undefined) {
        return this.notificationsService.addWarning('Debe ingresar un monto de descuento');
      }
      if ((this._globalDiscount <= 0) || (this._globalDiscount > 99)) {
        return this.notificationsService.addWarning('El descuento global debe ser mayor que 0% y menor o igual a 99%');
      }
      // Se copia los servicios elegidos actualmente para aplicarles el descuento global
      this._gdSelectedServices = this._selectedServices.map(x => Object.assign({}, x));
      // Se procesan todos los servicios seleccionados para agregarles el descuento global
      if (this._gdSelectedServices.length > 0) {
        let service: InvoiceLines;
        this._gdInvoiceSubTotal = 0;
        this._gdInvoiceDiscountTotal = 0;
        this._gdInvoiceTaxTotal = 0;
        this._gdInvoiceTotal = 0;
        for (service of this._gdSelectedServices) {
          // Cálculos del servicio sin descuento global
          let subTotal = 0;
          let descuentoGlobal = 0;
          const currentSubTotal = Math.round((service.Total - service.Impuesto) * 100) / 100;
          let SelectedService;
          if (service.IdService != null) {
            SelectedService = this._services.find((currentService: IService) =>
              currentService.id === service.IdService);
          }
          // Cálculos aplicando el descuento global
          descuentoGlobal = Math.round(this.gdServiceDiscount(currentSubTotal, this._globalDiscount) * 100) / 100;
          const sumaDescuentos = ((this._globalDiscount / 100) + (service.Descuento / 100));
          const multiplicacionDescuentos = ((this._globalDiscount / 100) * (service.Descuento / 100));
          const calculoDescuentoTotal = (sumaDescuentos - multiplicacionDescuentos) * 100;
          service.Descuento = Math.round(calculoDescuentoTotal * 100) / 100;
          subTotal = Math.round((currentSubTotal - descuentoGlobal) * 100) / 100;
          if (service.IdService != null) {
            service.Impuesto = Math.round(this.serviceTax(subTotal, SelectedService.tax.rate) * 100) / 100;
          } else {
            const cSelectedTax = this._taxes.find((tax: ITaxes) =>
              tax.id === service.idImpuesto);
            service.Impuesto = Math.round(this.serviceTax(subTotal, cSelectedTax.rate) * 100) / 100;
          }
          service.Total = Math.round((subTotal + service.Impuesto) * 100) / 100;
          // Sumamos el monto del servicio a los totales de la factura con descuento (forma inicial)
          this._gdInvoiceSubTotal = this._gdInvoiceSubTotal + currentSubTotal;
          this._gdInvoiceDiscountTotal = this._gdInvoiceDiscountTotal + descuentoGlobal;
          this._gdInvoiceTaxTotal = this._gdInvoiceTaxTotal + service.Impuesto;
          this._gdInvoiceTotal = (Math.round(this._gdInvoiceSubTotal * 100) / 100) + (Math.round(this._gdInvoiceTaxTotal * 100) / 100)
            - (Math.round(this._gdInvoiceDiscountTotal * 100) / 100);
        }
        // Sumamos el monto del servicio a los totales de la factura con descuento (forma Carlos)
        // const discountGeneral = (this._invoiceSubTotal * this._globalDiscount) / 100;
        // this._gdInvoiceSubTotal = this._invoiceSubTotal - discountGeneral;
        // this._gdInvoiceDiscountTotal = discountGeneral;
        // this._gdInvoiceTaxTotal = this._invoiceTaxTotal;
        // this._gdInvoiceTotal = (Math.round(this._gdInvoiceSubTotal * 100) / 100) + (Math.round(this._gdInvoiceTaxTotal * 100) / 100);
      } else {
        this._gdInvoiceSubTotal = 0;
        this._gdInvoiceDiscountTotal = 0;
        this._gdInvoiceTaxTotal = 0;
        this._gdInvoiceTotal = 0;
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
  * Método que carga el precio en BD del servicio seleccionado
  */
  private onSelectedItemChange() {
    if (this._itemId === undefined) {
      return this.notificationsService.addWarning('Debe seleccionar el servicio o producto que desea agregar');
    }
    const cSelectedItem = this._items.find((item: IItem) =>
      item.Id === this._itemId);
    this._servicePrice = Math.round(cSelectedItem.Price * 100) / 100;
    this._serviceQuantity = 1;
    this._serviceTitle = cSelectedItem.Name;
    const cSelectedTax = this._taxes.find((tax: ITaxes) =>
      tax.id === cSelectedItem.TaxId);
    this._serviceTax = cSelectedTax.id;
    this._serviceLock = true;
  }

  /*
  * Método que elimina la selección de servicio y deja libre para ingresar una linea nueva
  */
  private onUnlockServiceClicked() {
    this._itemId = undefined;
    this._serviceTitle = null;
    this._serviceTax = undefined;
    this._servicePrice = 0;
    this._serviceQuantity = 1;
    this._serviceDiscount = 0;
    this._serviceLock = false;
  }

//#region Agregar items a la factura y cálculos

  /*
  * Agrega un servicio a la lista de servicios a facturar
  */
  private addService() {
    // Instancia de IInvoiceLines donde se almacenara temporalmente el servicio seleccionado antes de agregarlo a _selectedServices
    if (this._serviceLock === true) {
      if (this._itemId === undefined) {
        return this.notificationsService.addWarning('Debe seleccionar el servicio que desea agregar');
      }
    } else {
      if ((this._serviceMeasurementUnit === undefined) || (this._serviceMeasurementUnit === null)) {
        return this.notificationsService.addWarning('Debe configurar la unidad de de Medida por defecto en Configuración / Compañía');
      }
    }
    if ((this._serviceTitle === undefined) || (this._serviceTitle === null) || (this._serviceTitle.length < 3)) {
      return this.notificationsService.addWarning('Debe especificar la descripción de la linea a agregar');
    }
    if ((this._serviceTax === undefined) || (this._serviceTax === null)) {
      return this.notificationsService.addWarning('Debe seleccionar el impuesto de la linea a agregar');
    }
    if ((this._servicePrice === undefined) || (this._servicePrice <= 0)) {
      return this.notificationsService.addWarning('El precio del item debe ser mayor a 0');
    }
    if (this._serviceDiscount >= 100) {
      return this.notificationsService.addWarning('El porcentaje de descuento no puede ser mayor a 99%');
    }
    if (this._serviceDiscount < 0) {
      return this.notificationsService.addWarning('El porcentaje de descuento no puede ser menor a 0%');
    }
    if (this._serviceQuantity === undefined) {
      return this.notificationsService.addWarning('Debe especificar la cantidad del item a facturar');
    }
    if (this._serviceQuantity <= 0) {
      return this.notificationsService.addWarning('Debe especificar la cantidad del item a facturar');
    }
    let selectedService = new InvoiceLines();
    selectedService.Cantidad = this._serviceQuantity;
    if (this._serviceNote === undefined) {
      selectedService.Note = null;
    } else {
      selectedService.Note = this._serviceNote;
    }
    let taxRate: number;
    if (this._serviceLock === true) {
      // Buscamos el objecto de Services en la data y procedemos a grabar los valores
      const cSelectedItem = this._items.find((item: Item) =>
        item.Id === this._itemId);
        if (cSelectedItem.Type === ItemType.Servicio) {
          selectedService.IdService = cSelectedItem.Id;
          selectedService.Tipo = LineType.Service;
        } else {
          selectedService.IdProduct = cSelectedItem.Id;
          selectedService.Tipo = LineType.Product;
        }
      selectedService.Servicio = cSelectedItem.Name;
      taxRate = cSelectedItem.Tax.rate;
      selectedService.UnidadMedida = cSelectedItem.UnidadMedidaType;
      selectedService.UnidadMedidaOtra = cSelectedItem.UnidadMedidaOtra;
    } else {
      selectedService.IdService = null;
      selectedService.Servicio = this._serviceTitle;
      const cSelectedTax = this._taxes.find((tax: ITaxes) =>
        tax.id === this._serviceTax);
      taxRate = cSelectedTax.rate;
      selectedService.UnidadMedida = this._serviceMeasurementUnit;
      selectedService.UnidadMedidaOtra = this._serviceMeasurementUnitOther;
    }
    selectedService.idImpuesto = this._serviceTax;
    let subTotal = 0;
    selectedService.Precio = Math.round(this._servicePrice * 100) / 100;
    if (this._serviceDiscount !== undefined) {
      selectedService.Descuento = this._serviceDiscount;
      subTotal = Math.round(this.serviceSubTotal(selectedService.Precio, selectedService.Cantidad
        , this._serviceDiscount) * 100) / 100;
    } else {
      selectedService.Descuento = 0;
      subTotal = Math.round(this.serviceSubTotal(selectedService.Precio, selectedService.Cantidad
        , 0) * 100) / 100;
    }
    selectedService.Impuesto = Math.round(this.serviceTax(subTotal, taxRate) * 100) / 100;
    selectedService.Total = Math.round((subTotal + selectedService.Impuesto) * 100) / 100;
    // Si se pasa de 12 digitos mas 2 decimales evitamos el ingreso
    if ((selectedService.Total + this._invoiceTotal) >= 9999999999999.99) {
      return this.notificationsService.addWarning('El Monto del total de la factura excede lo permitido');
    }
    // Agregamos la instancia de IInvoiceLines a _selectedServices
    this._selectedServices.push(selectedService);
    this._userSelectedService = true;
    // Sumamos el monto del servicio a los totales de la factura
    this._invoiceSubTotal = this._invoiceSubTotal + subTotal + (Math.round(this.serviceDiscount(selectedService.Precio,
      selectedService.Cantidad, this._serviceDiscount) * 100) / 100);
    this._invoiceDiscountTotal = this._invoiceDiscountTotal + (Math.round(this.serviceDiscount(selectedService.Precio,
      selectedService.Cantidad, this._serviceDiscount) * 100) / 100);
    this._invoiceTaxTotal = this._invoiceTaxTotal + selectedService.Impuesto;
    this._invoiceTotal = (Math.round(this._invoiceSubTotal * 100) / 100) + (Math.round(this._invoiceTaxTotal * 100) / 100)
      - (Math.round(this._invoiceDiscountTotal * 100) / 100);
    // Se llama al método de calcular los descuentos globales en caso de que el usuario ya tenga aplicado un descuento
    this.onGlobalDiscountChange();
    this._serviceDiscount = 0;
    this._serviceQuantity = 1;
    this._serviceTitle = null;
    this._itemId = undefined;
    this.onUnlockServiceClicked();
  }

  /*
  * Calcula el monto del descuento del servicio a facturar con descuento Global
  */
  private gdServiceDiscount(subtotal: number, discount: number): number {
    const totalDiscount = (subtotal * discount) / 100;
    return (totalDiscount);
  }

  /*
  * Calcula el monto del descuento del servicio a facturar
  */
  private serviceDiscount(price: number, quantity: number, discount: number): number {
    const totalAmount = price * quantity;
    let totalDiscount;
    if (discount !== undefined) {
      totalDiscount = (totalAmount * discount) / 100;
    } else {
      totalDiscount = 0;
    }
    return (totalDiscount);
  }

  /*
  * Calcula el monto Sub Total del servicio a facturar
  */
  private serviceSubTotal(price: number, quantity: number, discount: number): number {
    const totalAmount = price * quantity;
    const totalDiscount = (totalAmount * discount) / 100;
    return (totalAmount - totalDiscount);
  }

  /*
  * Calcula el monto del impuesto del servicio a facturar
  */
  private serviceTax(price: number, tax: number): number {
    const taxTotal = (price * tax) / 100;
    return (taxTotal);
  }

  /*
  * Elimina un servicio de la lista de servicios a facturar
  */
  private removeService(service: InvoiceLines) {
    const subTotal = Math.round(this.serviceSubTotal(service.Precio, service.Cantidad
      , 0) * 100) / 100;
    this._invoiceSubTotal = Math.round((this._invoiceSubTotal - subTotal) * 100) / 100;
    this._invoiceDiscountTotal = Math.round((this._invoiceDiscountTotal - this.serviceDiscount(service.Precio,
      service.Cantidad, service.Descuento)) * 100) / 100;
    this._invoiceTaxTotal = Math.round((this._invoiceTaxTotal - service.Impuesto) * 100) / 100;
    this._invoiceTotal = (Math.round(this._invoiceSubTotal * 100) / 100) + (Math.round(this._invoiceTaxTotal * 100) / 100)
      - (Math.round(this._invoiceDiscountTotal * 100) / 100);
    this._selectedServices.splice(this._selectedServices.indexOf(service), 1);
    if (this._selectedServices.length === 0) {
      this._invoiceSubTotal = 0;
      this._invoiceDiscountTotal = 0;
      this._invoiceTaxTotal = 0;
      this._invoiceTotal = 0;
      this._userSelectedService = false;
    }
    this.onGlobalDiscountChange();
  }
//#endregion

  //#region Filtro de Items

  /*
  * Maneja los cambios en el filtro de items
  */
  private itemFilterChange() {
    this._filteredItems = this._itemFilter ? this.performeItemFilter(this._itemFilter) : this._items;
  }

  /*
    * Método para filtrar los items por nombre
    */
  private performeItemFilter(filterBy: string): Item[] {
    if (this._filtroItem) {
      filterBy = filterBy.toLocaleLowerCase();
      return this._items.filter((item: Item) =>
        item.Name.toLocaleLowerCase().indexOf(filterBy) !== -1
      ).sort((a, b) => {
        if (a.Name.toLocaleLowerCase() < b.Name.toLocaleLowerCase()) {
          return -1;
        } else if (a.Name.toLocaleLowerCase() > b.Name.toLocaleLowerCase()) {
          return 1;
        } else {
          return 0;
        }
      });
    } else {
      return this._items;
    }
  }
  //#endregion

  //#region Creación de productos y servicios

  /*
  * Oculta o muestra panel para agregar servicios
  */
  private addNewService() {
    this._newService = !this._newService;
    this._filtroItem = false;
  }

  /*
  * Oculta o muestra panel para agregar productos
  */
  private addNewProduct() {
    this._newProduct = !this._newProduct;
    this._filtroItem = false;
  }

  /*
  * Método que recibe la notification de la creación del cliente desde la app-create-quickclient Directive
  */
  private onServiceCreated(serviceCreated: IService) {
    this.authService.RefreshToken()
      .subscribe(tokenActive => {
        if (tokenActive) {
          // Request al servidor
          // recargar los servicios a la lista para poder mostrar el nuevo servicio creado
          this.serviceService.getServices()
            .subscribe(services => {
              this._services = services;
              this.VaciarItems();
              this.LlenarProductosItems();
              this.LlenarServiciosItems();
              this._filteredItems = this._items;
              // Seleccionar en el Select el nuevo servicio como seleccionado
              this._itemId = serviceCreated.id;
              // Ocular el panel de creación de nuevo servicio
              this._newService = !this._newService;
              // Cargar el precio del nuevo servicio
              this.onSelectedItemChange();
            },
              serviceError => this._clientErrorMessage = <any>serviceError);
          // Fin de los Request
        }
      },
        serviceError => console.log(serviceError));
  }

  /*
  * Método que recibe la notification de la creación del cliente desde la app-create-quickproduct Directive
  */
  private onProductCreated(productCreated: IProduct) {
    this.authService.RefreshToken()
      .subscribe(tokenActive => {
        if (tokenActive) {
          // Request al servidor
          // recargar los servicios a la lista para poder mostrar el nuevo servicio creado
          this.productService.getProducts()
            .subscribe(products => {
              this._products = products;
              this.VaciarItems();
              this.LlenarProductosItems();
              this.LlenarServiciosItems();
              // Cambiar para que el filtro tome todo
              this._filteredItems = this._items;
              // Seleccionar en el Select el nuevo servicio como seleccionado
              this._itemId = productCreated.id;
              // Ocular el panel de creación de nuevo servicio
              this._newProduct = !this._newProduct;
              // Cargar el precio del nuevo producto
              this.onSelectedItemChange();
            },
              serviceError => this._clientErrorMessage = <any>serviceError);
          // Fin de los Request
        }
      },
        serviceError => console.log(serviceError));
  }
  //#endregion

  //#region Clientes
  /*
  * Maneja los cambios en el filtro de clientes
  */
  private clientFilterChange() {
    this._filteredClients = this._clientFilter ? this.performeClientFilter(this._clientFilter) : this._Clients;
  }

  /*
  * Oculta o muestra panel para agregar clientes
  */
  private addClient() {
    this._newClient = !this._newClient;
    this._filtroCliente = false;
  }

  /*
  * Método que recibe la notification de la creación del cliente desde la app-create-quickclient Directive
  */
  private onClientCreated(clientCreated: IClient) {
    this.authService.RefreshToken()
      .subscribe(tokenActive => {
        if (tokenActive) {
          // Request al servidor
          // recargar los clientes a la lista para poder mostrar el nuevo cliente creado
          this.clientService.getClients()
            .subscribe(clients => {
              if (clients !== undefined) {
                clients.sort((a, b) => {
                  if (a.name.toLocaleLowerCase() < b.name.toLocaleLowerCase()) {
                    return -1;
                  } else if (a.name.toLocaleLowerCase() > b.name.toLocaleLowerCase()) {
                    return 1;
                  } else {
                    return 0;
                  }
                });
              } else {
                this._Clients = null;
              }
              this._Clients = clients;
              this._filteredClients = this._Clients;
              // Seleccionar en el Select el nuevo cliente como seleccionado
              this._clientId = clientCreated.id;
              if (this._comprobanteType === DocumentType.Ticket) {
                this._invoice.ClientIdentificationType = clientCreated.identificationType;
                if (clientCreated.email !== undefined) {
                  this._invoice.ClientEmail = clientCreated.email;
                }
                if (clientCreated.identificationType === IdentificationType.NoAsiganda) {
                  this._invoice.ClientIdentification = clientCreated.identificacionExtranjero;
                } else {
                  this._invoice.ClientIdentification = clientCreated.identification;
                }
                if (clientCreated.lastName !== undefined) {
                  this._invoice.ClientName = clientCreated.name + ' ' + clientCreated.lastName;
                } else {
                  this._invoice.ClientName = clientCreated.name;
                }
              }
              // Ocular el panel de creación de nuevo cliente
              this._newClient = !this._newClient;
            },
              serviceError => this._clientErrorMessage = <any>serviceError);
          // Fin de los Request
        }
      },
        serviceError => console.log(serviceError));
  }

  /*
  * Método para filtrar los clientes por nombre y identificacion
  */
  private performeClientFilter(filterBy: string): IClient[] {
    filterBy = filterBy.toLocaleLowerCase();
    return this._Clients.filter((client: IClient) =>
      // client.name.toLocaleLowerCase().indexOf(filterBy) !== -1
      ((client.name.toLocaleLowerCase().indexOf(filterBy) !== -1) ||
       ( (client.identification !== null) && (client.identification.indexOf(filterBy) !== -1)) ||
       ( (client.identificacionExtranjero !== null) && (client.identificacionExtranjero.indexOf(filterBy) !== -1)))
    ).sort((a, b) => {
      if (a.name.toLocaleLowerCase() < b.name.toLocaleLowerCase()) {
        return -1;
      } else if (a.name.toLocaleLowerCase() > b.name.toLocaleLowerCase()) {
        return 1;
      } else {
        return 0;
      }
    });
  }
  /*
  * Método para asignar los dias de crédito de un cliente seleccionado
  */
  private onSelectedClientChange() {
    if (this._clientId !== undefined) {
      const SelectedClient = this._Clients.find((client: IClient) =>
        client.id === this._clientId);
        // console.log(SelectedClient);
      if (SelectedClient.creditDays > 0) {
        this._invoiceCreditDays = SelectedClient.creditDays;
      } else {
        this._invoiceCreditDays = 0;
      }
      if (this._comprobanteType === DocumentType.Ticket) {
        this._invoice.ClientIdentificationType = SelectedClient.identificationType;
        if (SelectedClient.email !== undefined) {
          this._invoice.ClientEmail = SelectedClient.email;
        }
        if (SelectedClient.identificationType === IdentificationType.NoAsiganda) {
          this._invoice.ClientIdentification = SelectedClient.identificacionExtranjero;
        } else {
          this._invoice.ClientIdentification = SelectedClient.identification;
        }
        if (SelectedClient.lastName !== undefined) {
          this._invoice.ClientName = SelectedClient.name + ' ' + SelectedClient.lastName;
        } else {
          this._invoice.ClientName = SelectedClient.name;
        }
      }
    }
  }
  //#endregion

  //#region Medios de Pago
  /*
  * Método para asignar el monto a pagar a efectivo
  */
  private invoicePaymentCashChange() {
    if (this._invoicePaymentCash === true) {
      if ((this._invoicePaymentCheck === false) && (this._invoicePaymentCreditCard === false)
        && (this._invoicePaymentDeposit === false)) {
        if (this._descuentoGlobal) {
          this._invoiceAmountCash = Math.round(this._gdInvoiceTotal * 100) / 100;
        } else {
          this._invoiceAmountCash = Math.round(this._invoiceTotal * 100) / 100;
        }
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
        if (this._descuentoGlobal) {
          this._invoiceamountCreditCard = Math.round(this._gdInvoiceTotal * 100) / 100;
        } else {
          this._invoiceamountCreditCard = Math.round(this._invoiceTotal * 100) / 100;
        }
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
        if (this._descuentoGlobal) {
          this._invoiceAmountCheck = Math.round(this._gdInvoiceTotal * 100) / 100;
        } else {
          this._invoiceAmountCheck = Math.round(this._invoiceTotal * 100) / 100;
        }
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
        if (this._descuentoGlobal) {
          this._invoiceAmountDeposit = Math.round(this._gdInvoiceTotal * 100) / 100;
        } else {
          this._invoiceAmountDeposit = Math.round(this._invoiceTotal * 100) / 100;
        }
      }
    } else {
      this._invoiceAmountDeposit = null;
    }
  }

  /*
  * Método para prevenir ingreso de valores mayores de 10 cifras (keypress)="keyPress($event)"
  */
  private onDecimalValueChange(inputValue: any) {
    // console.log(inputValue);
    if (inputValue.target.value.length > 12) {
      event.preventDefault();
    }
    if ((inputValue.keyCode === 45) || (inputValue.keyCode === 43) || (inputValue.keyCode === 44) || (inputValue.keyCode === 101)) {
      event.preventDefault();
    }
  }

  /*
  * Método para prevenir ingreso de valores mayores de 3 cifras (keypress)="keyPress($event)"
  */
  private onPercentageValueChange(inputValue: any) {
    // console.log(inputValue);
    if (inputValue.target.value.length > 3) {
      event.preventDefault();
    }
    if ((inputValue.keyCode === 45) || (inputValue.keyCode === 43) || (inputValue.keyCode === 44) || (inputValue.keyCode === 101)) {
      event.preventDefault();
    }
  }
  //#endregion

  /*
   * Método para enviar de regreso a la búsqueda de facturas
   */
  private returnToIndex() {
    this.router.navigate(['/home/invoiceIndex']);
  }

//#region Llenado de lista de Items
  /*
  * Método para juntar los productos en una misma lista de items
  */
  private LlenarProductosItems() {
    // let producto: IProduct;
    if (this._products !== undefined && this._products.length > 0) {
      for (let producto of this._products) {
        let itemProducto = new Item();
        itemProducto.Id = producto.id;
        itemProducto.Name = producto.name;
        itemProducto.Price = producto.retailPrice;
        itemProducto.TaxId = producto.taxId;
        itemProducto.Tax = producto.tax;
        itemProducto.UnidadMedidaType = producto.unitMeasurement;
        itemProducto.Type = ItemType.Producto;
        this._items.push(itemProducto);
      }
    }
    this._items.sort((a, b) => {
      if (a.Name.toLocaleLowerCase() < b.Name.toLocaleLowerCase()) {
        return -1;
      } else if (a.Name.toLocaleLowerCase() > b.Name.toLocaleLowerCase()) {
        return 1;
      } else {
        return 0;
      }
    });
  }

  /*
    * Método para juntar los servicios en una misma lista de items
    */
  private LlenarServiciosItems() {
    let servicio: IService;
    if (this._services !== undefined) {
      for (servicio of this._services) {
        let item = new Item();
        item.Id = servicio.id;
        item.Name = servicio.name;
        item.Price = servicio.price;
        item.TaxId = servicio.taxId;
        item.Tax = servicio.tax;
        item.UnidadMedidaType = servicio.unitMeasurement;
        item.UnidadMedidaOtra = servicio.unitMeasurementOthers;
        item.Type = ItemType.Servicio;
        this._items.push(item);
      }
    }
    this._items.sort((a, b) => {
      if (a.Name.toLocaleLowerCase() < b.Name.toLocaleLowerCase()) {
        return -1;
      } else if (a.Name.toLocaleLowerCase() > b.Name.toLocaleLowerCase()) {
        return 1;
      } else {
        return 0;
      }
    });
  }

  /*
  * Vaciá los Items de la lista para volverlos a rellenar
  * */
  private VaciarItems() {
    this._items = new Array<Item>();
  }
  //#endregion

  //#region Inicializacion de Variables y Carga inicial

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
        infoEmpty: 'Mostrando registros del 0 al 0 de un total de 0 registros',
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
      responsive: false,
      retrieve: true
    };
  }

  ngOnInit() {
    /*
    * Inicializacion de variables
    */
    this._comprobanteType = DocumentType.Invoice;
    this._procesando = true;
    this._userSelectedService = false;
    this._newClient = false;
    this._newService = false;
    this._newProduct = false;
    this._invoicePaymentCash = false;
    this._invoicePaymentCreditCard = false;
    this._invoicePaymentCheck = false;
    this._invoicePaymentDeposit = false;
    this._invoiceError = false;
    this._invoiceSubTotal = 0;
    this._invoiceDiscountTotal = 0;
    this._invoiceTaxTotal = 0;
    this._invoiceTotal = 0;
    this._gdInvoiceSubTotal = 0;
    this._gdInvoiceDiscountTotal = 0;
    this._gdInvoiceTaxTotal = 0;
    this._gdInvoiceTotal = 0;
    this._invoicePaymentConditionsCredit = false;
    this._invoiceCreditDays = 0;
    this._serviceQuantity = 1;
    this._serviceTitle = null;
    this._items = new Array<Item>();
    if (localStorage.getItem('tenant')) {
      // Obtiene la configuración del Tenant
      this._tenant = JSON.parse(localStorage.getItem('tenant'));
      // this._currencyCode = this._currency.getCurrencyName(this._tenant.codigoMoneda);
      this._invoiceMoneyType = Moneda[this._tenant.currencyCode];
      this._currencyCode = Moneda[this._invoiceMoneyType];
      this._serviceMeasurementUnit = this._tenant.unitMeasurementDefault;
      this._serviceMeasurementUnitOther = this._tenant.unitMeasurementOthersDefault;
      // console.log(this._tenant.tipoFirma);
      if (this._tenant.tipoFirma !== undefined) {
        if (this._tenant.tipoFirma === 0) {
          this._TenantFirmType = FirmType.Llave;
        } else if (this._tenant.tipoFirma === 1) {
          this._TenantFirmType = FirmType.Firma;
        } else if (this._tenant.tipoFirma === 2) {
          this._TenantFirmType = FirmType.Todos;
        }
      }
    }

    /*
        * Carga de data inicial a los controles desde el Webapi
        */
    this.authService.RefreshToken()
      .subscribe(tokenActive => {
        if (tokenActive) {
          // Request al servidor
          // Cargar los clientes a la lista
          this.clientService.getClients()
            .subscribe(clients => {
              this._Clients = clients;
              this._filteredClients = this._Clients;
              this._procesando = false;
              this._filteredClients.sort((a, b) => {
                if (a.name.toLocaleLowerCase() < b.name.toLocaleLowerCase()) {
                  return -1;
                } else if (a.name.toLocaleLowerCase() > b.name.toLocaleLowerCase()) {
                  return 1;
                } else {
                  return 0;
                }
              });
            },
              serviceError => this._clientErrorMessage = <any>serviceError);
          // Cargar los servicios a la lista
          this.serviceService.getServices()
            .subscribe(services => {
              this._services = services;
              this.LlenarServiciosItems();
              this._filteredItems = this._items;
              // this._procesando = false;
            },
              serviceError => this._serviceErrorMessage = <any>serviceError);
          // Cargar los productos a la lista
          this.productService.getProducts()
            .subscribe(products => {
              this._products = products;
              this.LlenarProductosItems();
              this._filteredItems = this._items;
              // this._procesando = false;
            },
              serviceError => this._serviceErrorMessage = <any>serviceError);
          // Cargar los Impuestos a la Lista
          this.taxesService.getTaxes()
            .subscribe(taxes => {
              this._taxes = taxes;
            },
              serviceError => this._taxErrorMessage = <any>serviceError);
          // Fin de los Request
        } else {
          const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
          this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
        }
      },
        serviceError => console.log(serviceError));
  }
  //#endregion
}
