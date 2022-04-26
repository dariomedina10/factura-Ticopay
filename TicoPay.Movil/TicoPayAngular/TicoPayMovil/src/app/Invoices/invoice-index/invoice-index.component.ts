import { Component, OnInit, Injector, ElementRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { IClient } from '../../Models/ClientModel';
import { ClientsService } from '../../shared/dataServices/clients.service';
import { InvoiceService } from '../../shared/dataServices/invoice.service';
import { InvoiceSearchConfiguration, ICompleteInvoice, InvoiceStatus , Moneda, DocumentType} from '../../Models/InvoiceModel';
import { NotificationsService } from '../../shared/notifications/notifications.service';
import { DataTableConfig, DataTableColumn, ColumnType, ColumnFilterType } from '../../Models/dataTableModel';
import { scheduleMicroTask } from '@angular/core/src/util';
import { InvoicesModule } from '../invoices.module';
import { AuthService } from '../../shared/dataServices/auth-service.service';
import { DataTablesModule } from 'angular-datatables/src/angular-datatables.module';
import { LoaderService } from '../../shared/loader/loader.service';
import { ITenant } from '../../Models/Authentificatiomodel';
import { CurrencyService } from '../../shared/currency.service';

@Component({
  templateUrl: './invoice-index.component.html',
  styleUrls: ['./invoice-index.component.css']
})
export class InvoiceIndexComponent implements OnInit {
  /*
  * Variables para funcionamiento de controles
  */
  private _tenant: ITenant;
  private _loading: boolean;
  private _currencyCode: string;
  public invoiceStatusList = InvoiceStatus;
  private _filtroFacturas: boolean;
  private _pantallaMobile: boolean;
  // private dtOptions: DataTables.Settings;
  private dtOptions: any;
  public tipoDocumento = DocumentType;


  /*
  * Variables para el Control y Filtrado de Clientes
  */
  private _filteredClients: IClient[];
  private _Clients: IClient[];
  private _clientFilter: string;
  private _clientErrorMessage: string;
  private _clientId: string;

  /*
  * Variables para el Control y Filtrado de fechas
  */
  private _invoiceFromDate: Date;
  private _invoiceToDate: Date;
  private _invoiceStatus: InvoiceStatus;

  /*
  * Variables para el Control de las Facturas
  */
  private _invoices: ICompleteInvoice[];
  private _currentInvoice: ICompleteInvoice;
  /*
  * Constructor de la clase
  */
  constructor(private clientService: ClientsService, private invoiceService: InvoiceService
    , private router: Router, private notificationsService: NotificationsService, private authService: AuthService,
     private loader: LoaderService, private _currency: CurrencyService) {
    this._clientFilter = '';
    this._filteredClients = this._Clients;
    this.opcionesLenguajeTabla();
  }

  /*
  * Enviá al formulario de creación de factura
  */
  private newInvoice() {
    this.router.navigate(['home/createInvoice']);
  }

  /*
  * Maneja los cambios en el filtro de clientes
  */
  private clientFilterChange() {
    this._filteredClients = this._clientFilter ? this.performeClientFilter(this._clientFilter) : this._Clients;
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
  * Consulta las facturas en la Base de datos
  */
  private searchInvoices() {
    let searchRequest = new InvoiceSearchConfiguration();
    if (this._filtroFacturas) {
      if (this._clientId) {
        searchRequest.ClientId = this._clientId;
      } else {
        searchRequest.ClientId = null;
      }
      if (this._invoiceFromDate) {
        searchRequest.StartDueDate = this._invoiceFromDate.toString();
      } else {
        searchRequest.StartDueDate = null;
      }
      if (this._invoiceToDate) {
        searchRequest.EndDueDate = this._invoiceToDate.toString();
      } else {
        searchRequest.EndDueDate = null;
      }
      if (this._invoiceStatus) {
        searchRequest.Status = this._invoiceStatus;
      } else {
        searchRequest.Status = null;
      }
    } else {
      searchRequest.ClientId = null;
      searchRequest.StartDueDate = null;
      searchRequest.EndDueDate = null;
      searchRequest.Status = null;
    }
    searchRequest.InvoiceId = null;
    this._invoices = null;
    sessionStorage.setItem('invoiceSearch', JSON.stringify(searchRequest));
    this.loader.show();
    this.authService.RefreshToken()
      .subscribe(tokenActive => {
        if (tokenActive) {
          // Request al servidor
          this.invoiceService.getInvoices(searchRequest)
            .subscribe(invoices => {
              this._invoices = invoices;
              for (let entry of this._invoices) {
                entry.simboloMoneda = Moneda [entry.codigoMoneda];
              }
              this.loader.hide();
            },
            serviceError => this.notificationsService.addError(<any>serviceError));
          // Fin de los Request
        } else {
          const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
          this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
        }
      },
      serviceError => console.log(serviceError));
  }

  /*
  * Obtiene el detalle de la factura
  */
  private invoiceDetails(invoice: ICompleteInvoice) {
    this.router.navigate(['/home/invoiceDetail', invoice.id]);
    /*
    if (this._currentInvoice === invoice) {
      return;
    }
    this._currentInvoice = invoice;*/
  }

  /*
  * Método para setear el lenguaje del datatable
  */
  private opcionesLenguajeTabla() {
    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 10,
      language: {
        processing: 'Procesando...',
        lengthMenu: 'Mostrar _MENU_ registros',
        zeroRecords: 'No se encontraron resultados',
        emptyTable: 'Ningún dato disponible en esta tabla',
        info: 'Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros',
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
      responsive: true,
    };
  }

  private onFiltroFacturasChange() {
    if (!this._filtroFacturas) {
      this._filteredClients = this._Clients;
      this._clientFilter = null;
      this._clientId = null;
      this._invoiceFromDate = null;
      this._invoiceToDate = null;
      this._invoiceStatus = null;
    }
  }

  ngOnInit() {
    this._invoiceStatus = null;
    this._loading = true;
    let currentSearch = null;
    currentSearch = JSON.parse(sessionStorage.getItem('invoiceSearch'));
    if (window.screen.width < 768) {
      this._pantallaMobile = true;
    } else {
      this._pantallaMobile = false;
    }
    if (localStorage.getItem('tenant')) {
      // Obtiene la configuración del Tenant
      this._tenant = JSON.parse(localStorage.getItem('tenant'));
      // this._currencyCode = this._currency.getCurrencyName(this._tenant.codigoMoneda);
      this._currencyCode = this._tenant.currencyCode;
    }
    /*
    * Carga de data inicial a los controles desde el Webapi
    */
    this.authService.RefreshToken()
      .subscribe(tokenActive => {
        if (tokenActive) {
          // Request al servidor
          // Cargar busqueda por defecto
          if ((currentSearch !== undefined) && (currentSearch !== null)) {
            this.invoiceService.getInvoices(currentSearch)
            .subscribe(invoices => {
              this._invoices = invoices;
            for (let entry of this._invoices) {
              entry.simboloMoneda = Moneda [entry.codigoMoneda];
            }
              this._loading = false;
            },
            serviceError => this.notificationsService.addError(<any>serviceError));
            sessionStorage.removeItem('invoiceSearch');
          }
          // Llenar el combo de clientes
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
              this._loading = false;
            },
            serviceError => this._clientErrorMessage = <any>serviceError);
          // Fin de los Request
        } else {
          const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
          this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
        }
      },
      serviceError => this._clientErrorMessage = <any>serviceError);
    /*
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
          serviceError => console.log(serviceError));*/
  }

}
