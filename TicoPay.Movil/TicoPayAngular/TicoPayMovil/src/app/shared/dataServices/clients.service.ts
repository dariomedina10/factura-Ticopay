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
import { IClient } from '../../Models/ClientModel';
import { AuthService } from './auth-service.service';
import { NotificationsService } from '../../shared/notifications/notifications.service';

@Injectable()
export class ClientsService {
    // Direccion del WebApi Service: ruta completa en el archivo appConst.ts
    private _direccionServicio: string;
    private _token: string;
    private _tenancyName: string;
    private _clients: IClient[];
    private errorType: string;

    constructor(private _httpService: Http, private _router: Router, private authService: AuthService,
         protected notificationsService: NotificationsService) {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
    }

    /*
    *Realiza el Request del metodo Get /client/GetAll en el WebApi
    */
    public getClients(): Observable<IClient[]> {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + 'client/GetAll?detallado=false';
        if (this._token != null) {
            const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
            const options = new RequestOptions({ headers: headers });
            // console.log('Usando Token ' + this._token);
            return this._httpService.get(this._direccionServicio, options)
                .map((response) => {
                    return this.processGetClients(response);
                }).catch((response: any, caught: any) => {
                    if (response instanceof Response) {
                        try {
                            return null;
                        } catch (e) {
                            return Observable.throw(e);
                        }
                    } else {
                        return null; // <Observable<IClient[]>><any>Observable.throw(response);
                    }
                });
        } else {
            // El token del usuario no existe
            console.log('Usuario no Logeado');
            return Observable.throw('Usuario no Logeado');
        }
    }

    /*
    * Procesa la respuesta obtenida al llamar al metodo /client/GetAll del WebApi
    */
    protected processGetClients(response: Response): IClient[] {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            this._clients = response.json().listObjectResponse;
            return this._clients;
        } else {
            if (status === 408) {
                this._router.navigate(['/landing',
                 'Error al consultar los Clientes' +
                 ', la consulta esta tomando demasiado tiempo, por favor verifique su conexión de internet']);
            }
            this._router.navigate(['/landing', 'Error al consultar los Clientes']);
        }
        return null;
    }

    /*
    *Realiza el Request del metodo Post /client/Post en el WebApi
    */
    public createQuickClient(client: IClient): Observable<IClient> {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + 'client/Post';
        if (this._token != null) {
            const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
            const options = new RequestOptions({ headers: headers });
            const body = JSON.stringify(client);
            // console.log(body);
            return this._httpService.post(this._direccionServicio, body, options)
                .map((response) => {
                    return this.processCreateQuickClient(response);
                }).catch((response: any, caught: any) => {
                    if (response instanceof Response) {
                        try {
                            return Observable.of(this.processCreateQuickClient(response));
                        } catch (e) {
                            return <Observable<IClient>><any>Observable.throw(e);
                        }
                    } else {
                        return <Observable<IClient>><any>Observable.throw(response);
                    }
                });
        } else {
            // El token del usuario no existe o es invalido
            console.log('Usuario no Logeado');
            return Observable.throw('Usuario no Logeado');
        }
    }

    /*
    * Procesa la respuesta obtenida al llamar al metodo /client/Post del WebApi
    */
    protected processCreateQuickClient(response: Response): IClient {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        console.log(status);
        if (status === 200) {
            let client = response.json().objectResponse;
            if (client) {
                console.log('Usuario añadido');
                return client;
            }
        } else if (status === 500) {
            this.errorType = response.json().error.message;
            if (this.errorType.indexOf('Existe un cliente con el mismo número de cédula.') >= 0) {
                this.notificationsService.addError('Error: Ya existe un cliente con el mismo número de identificación.');
            } else {
                this.notificationsService.addError('Error al crear el cliente, ' + this.errorType);
            }
            return null;
        } else if (status === 400) {
            this.errorType = response.json().error.message;
            this.notificationsService.addError('Error en los datos del cliente, Error: ' + this.errorType);
            return null;
        } else if (status !== 200 && status !== 204) {
            return null;
        }
        return null;
    }

}
