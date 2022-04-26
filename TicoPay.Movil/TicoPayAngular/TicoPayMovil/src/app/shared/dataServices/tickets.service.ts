import { Injectable } from '@angular/core';
import { IInvoice, ICompleteInvoice } from '../../Models/InvoiceModel';
import { Observable } from 'rxjs/Observable';
import { AppConsts } from '../appConst';
import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';
import { NotificationsService } from '../notifications/notifications.service';
import { Router } from '@angular/router';
import * as FileSaver from 'file-saver';

@Injectable()
export class TicketsService {

  private _direccionServicio: string;
  private _token: string;
  private _tenancyName: string;
  private _Invoices: Observable<IInvoice[]>;
  private _completeInvoices: Observable<ICompleteInvoice[]>;

  constructor(private _httpService: Http, private _router: Router,
      protected notificationsService: NotificationsService) {
      const currentUser = JSON.parse(localStorage.getItem('token'));
      const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
      this._token = currentUser.token;
      this._tenancyName = currentTenant.tenancyName;
  }

  /*
  *Realiza el Request del metodo Post /invoice/PostTicketAsync en el WebApi
  */
  public createTicket(client: IInvoice): Observable<ICompleteInvoice> {
      const currentUser = JSON.parse(localStorage.getItem('token'));
      const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
      this._token = currentUser.token;
      this._tenancyName = currentTenant.tenancyName;
      this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/Invoice/PostTicketAsync';
      if (this._token != null) {
          const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
          const options = new RequestOptions({ headers: headers });
          const body = JSON.stringify(client);
          // console.log(body);
          return this._httpService.post(this._direccionServicio, body, options)
              .map((response) => {
                  return this.processCreateTicket(response);
              }).catch((response: any, caught: any) => {
                  if (response instanceof Response) {
                      try {
                          return Observable.of(this.processCreateTicket(response));
                      } catch (e) {
                          return <Observable<ICompleteInvoice>><any>Observable.throw(e);
                      }
                  } else {
                      return <Observable<ICompleteInvoice>><any>Observable.throw(response);
                  }
              });
      } else {
          // El token del usuario no existe o es invalido
          console.log('Usuario no Logeado');
          return Observable.throw('Usuario no Logeado');
      }
  }

  /*
  * Procesa la respuesta obtenida al llamar al metodo /invoice/PostTicketAsync del WebApi
  */
  protected processCreateTicket(response: Response): ICompleteInvoice {
      const responseText = response.text();
      const status = response.status;
      // console.log(response.json());
      if (status === 200) {
          const invoice = response.json().objectResponse;
          if (invoice) {
              console.log('Ticket añadido');
              return invoice;
          }
      } else if (status === 400 || status === 500) {
          const errorType = response.json().error_msg;
          console.log(errorType);
          if (errorType === 'Por favor confirme su usuario y verifique su dirección fiscal.') {
              this.notificationsService.addError('Error: Por favor confirme su usuario y verifique su dirección fiscal.');
          } else if (errorType === 'Has alcanzado el limite mensual de facturas.' +
              ' Puedes obtener un plan con mayor número de facturas o esperar hasta el próximo mes.') {
              this.notificationsService.addError('Has alcanzado el limite mensual de facturas.' +
                  'Puedes obtener un plan con mayor número de facturas o esperar hasta el próximo mes.');
          } else if (errorType === 'Posee facturas pendientes por firma digital.' +
              ' Por favor complete el proceso de firma y envío a hacienda de estas factura para' +
              ' poder generar nuevas facturas con llave criptográfica.') {
              this.notificationsService.addError('Posee facturas pendientes por firma digital.' +
                  ' Por favor complete el proceso de firma y envío a hacienda de estas factura para' +
                  ' poder generar nuevas facturas con llave criptográfica.');
          } else {
              this.notificationsService.addError('Problemas en la creación del Tiquete');
          }
          return null;
      } else if (status !== 200 && status !== 204) {
          this.notificationsService.addError('Problemas en la creación del Tiquete');
          return null;
      }
      return null;
  }

  /*
    *Realiza el Request del método Get /invoice/GetInvoicePDF/ en el WebApi
    */
   public getInvoicePDF(idInvoice: string, invoiceNumber: string): Observable<Blob> {
    const currentUser = JSON.parse(localStorage.getItem('token'));
    const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
    this._token = currentUser.token;
    this._tenancyName = currentTenant.tenancyName;
    this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + 'invoice/GetInvoicePDFFile/' + idInvoice;
    if (this._token != null) {
        const headers = new Headers({
        'Content-Type': 'application/json',
        'Accept': 'application/pdf' ,
        'Authorization': 'Bearer ' + this._token });
        const options = new RequestOptions({ headers: headers });
        options.responseType = ResponseContentType.Blob;
        // console.log('Usando Token ' + this._token);
        return this._httpService.get(this._direccionServicio, options)
            .map((response) => {
                return this.processGetInvoicePDF(response, invoiceNumber);
            }).catch((response: any, caught: any) => {
                if (response instanceof Response) {
                    try {
                        return Observable.of(this.processGetInvoicePDF(response, invoiceNumber));
                    } catch (e) {
                        return Observable.throw(e);
                    }
                } else {
                    return Observable.throw(response); // <Observable<IClient[]>><any>Observable.throw(response);
                }
            });
    } else {
        // El token del usuario no existe
        console.log('Usuario no Logeado');
        return Observable.throw('Usuario no Logeado');
    }
}

/*
* Procesa la respuesta obtenida al llamar al metodo /invoice/GetInvoicePDF del WebApi
*/
protected processGetInvoicePDF(response: Response, invoiceNumber: string): Blob {
    const responseText = response.text();
    const status = response.status;
    if (status === 200) {
        const mediaType = 'application/pdf';
        const blob = new Blob([response.blob()], {type: mediaType});
        const filename = 'Comprobante ' + invoiceNumber + '.pdf';
        FileSaver.saveAs(blob, filename);
        return response.blob();
    } else {
        if (status === 408) {
            this._router.navigate(['/landing',
             'Error al descargar el Pdf' +
             ', la consulta esta tomando demasiado tiempo, por favor verifique su conexión de internet']);
        }
        this._router.navigate(['/landing', 'Error al descargar el Pdf']);
    }
    return null;
}


}
