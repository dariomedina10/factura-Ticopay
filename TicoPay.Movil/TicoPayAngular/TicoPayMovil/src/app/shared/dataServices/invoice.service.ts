import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/observable/throw';
import 'rxjs/add/observable/of';
import { AppConsts } from '../appConst';
import { Console } from '@angular/core/src/console';
import { Router } from '@angular/router';
import { IInvoice, ICompleteInvoice, InvoiceSearchConfiguration, Moneda } from '../../Models/InvoiceModel';
import { NotificationsService } from '../notifications/notifications.service';
import { forEach } from '@angular/router/src/utils/collection';

@Injectable()
export class InvoiceService {

    private _direccionServicio: string;
    private _token: string;
    private _tenancyName: string;
    private _Invoices: Observable<IInvoice[]>;
    private _completeInvoices: ICompleteInvoice[];
    private _printerData: string;

    constructor(private _httpService: Http, private _router: Router,
        protected notificationsService: NotificationsService) {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
    }

    /*
    *Realiza el Request del metodo Post /invoice/Post en el WebApi
    */
    public createInvoice(client: IInvoice): Observable<ICompleteInvoice> {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/Invoice/PostAsync';
        if (this._token != null) {
            const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
            const options = new RequestOptions({ headers: headers });
            const body = JSON.stringify(client);
            // console.log(body);
            return this._httpService.post(this._direccionServicio, body, options)
                .map((response) => {
                    return this.processCreateInvoice(response);
                }).catch((response: any, caught: any) => {
                    if (response instanceof Response) {
                        try {
                            return Observable.of(this.processCreateInvoice(response));
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
    * Procesa la respuesta obtenida al llamar al metodo /invoice/Post del WebApi
    */
    protected processCreateInvoice(response: Response): ICompleteInvoice {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            const invoice = response.json().objectResponse;
            if (invoice) {
                console.log('Factura añadida');
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
                this.notificationsService.addError('Problemas en la creación de la factura');
            }
            return null;
        } else if (status !== 200 && status !== 204) {
            this.notificationsService.addError('Problemas en la creación de la factura');
            return null;
        }
        return null;
    }

    /*
    *Realiza el Request del metodo Get /Invoice/GetInvoices en el WebApi
    */
    public getInvoices(body: InvoiceSearchConfiguration): Observable<ICompleteInvoice[]> {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/Invoice/GetInvoices';
        if (this._token != null) {
            const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
            const options = new RequestOptions({ headers: headers });
            // console.log(body);
            return this._httpService.post(this._direccionServicio, body, options)
                .map((response) => {
                    return this.processGetInvoices(response);
                }).catch((response: any, caught: any) => {
                    if (response instanceof Response) {
                        try {
                            return Observable.of(this.processGetInvoices(response));
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
    * Procesa la respuesta obtenida al llamar al metodo /Invoice/GetInvoices del WebApi
    */
    protected processGetInvoices(response: Response): ICompleteInvoice[] {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            this._completeInvoices = response.json().listObjectResponse;
            return this._completeInvoices;
        } else if (status !== 200 && status !== 204) {
            if (status === 408) {
                this._router.navigate(['/landing',
                    'Error al consultar las facturas' +
                    ', la consulta esta tomando demasiado tiempo, por favor verifique su conexión de internet']);
            }
            this._router.navigate(['/landing', 'Error al consultar las facturas']);
            return null;
        }
        return null;
    }

    /*
  *Realiza el Request del metodo Post /Invoice/Resend en el WebApi
  */
    public resendEmail(facturas: string[]): Observable<boolean> {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/Invoice/Resend';
        if (this._token != null) {
            const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
            const options = new RequestOptions({ headers: headers });
            const body = JSON.stringify(facturas);
            // console.log(body);
            return this._httpService.post(this._direccionServicio, body, options)
                .map((response) => {
                    return this.processResendEmail(response);
                }).catch((response: any, caught: any) => {
                    if (response instanceof Response) {
                        try {
                            return Observable.of(this.processResendEmail(response));
                        } catch (e) {
                            return <Observable<boolean>><any>Observable.throw(e);
                        }
                    } else {
                        return <Observable<boolean>><any>Observable.throw(response);
                    }
                });
        } else {
            // El token del usuario no existe o es invalido
            console.log('Usuario no Logeado');
            return Observable.throw('Usuario no Logeado');
        }
    }

    /*
    * Procesa la respuesta obtenida al llamar al metodo /Invoice/Resend del WebApi
    */
    protected processResendEmail(response: Response): boolean {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            // console.log('Email reenviado');
            return true;
        } else if (status !== 200 && status !== 204) {
            return null;
        }
        return null;
    }

     /*
  *Realiza el Request del metodo Post /Invoice/Resend en el WebApi
  */
 public GetPrinterData(idInvoiceOrTicket: string): Observable<string> {
    const currentUser = JSON.parse(localStorage.getItem('token'));
    const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
    this._token = currentUser.token;
    this._tenancyName = currentTenant.tenancyName;
    this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/Invoice/PrintDataForInvoiceOrTicket?IdInvoiceOrTicket='+ idInvoiceOrTicket;
    if (this._token != null) {
        const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
        const options = new RequestOptions({ headers: headers });
        // const body = JSON.stringify(facturas);
        // console.log(body);
        return this._httpService.post(this._direccionServicio,null, options)
            .map((response) => {
                return this.processGetPrinterData(response);
            }).catch((response: any, caught: any) => {
                if (response instanceof Response) {
                    try {
                        return Observable.of(this.processGetPrinterData(response));
                    } catch (e) {
                        return <Observable<string>><any>Observable.throw(e);
                    }
                } else {
                    return <Observable<string>><any>Observable.throw(response);
                }
            });
    } else {
        // El token del usuario no existe o es invalido
        console.log('Usuario no Logeado');
        return Observable.throw('Usuario no Logeado');
    }
}

/*
* Procesa la respuesta obtenida al llamar al metodo /Invoice/Resend del WebApi
*/
protected processGetPrinterData(response: Response): string {
    const responseText = response.text();
    const status = response.status;
    // console.log(response.json());
    if (status === 200) {
        this._printerData = response.json().objectResponse;
        if (this._printerData) {
            // console.log('');
            return this._printerData;
        }
    } else if (status === 400 || status === 404 || status === 500) {       
        if(status === 400){
            this.notificationsService.addWarning('Error: No tiene configurada una impresora de punto de venta.');
        }
        if(status === 404){
            this.notificationsService.addError('Error: No existe el tiquete que desea imprimir.');
        }
        if(status === 500){
            this.notificationsService.addError('Error: Imposible traer data de impresión para el tiquete.');
        }
        return null;
    }
    return null;
}

}


