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
import { IService, Service } from '../../Models/ServiceModel';

@Injectable()
export class ServicesService {

    // Direccion del WebApi Service: ruta completa en el archivo appConst.ts
    private _direccionServicio: string;
    private _token: string;
    private _tenancyName: string;
    private _services: IService[];

    constructor(private _httpService: Http, private _router: Router) {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
    }

    /*
    *Realiza el Request del metodo Get /Service/GetAll en el WebApi
    */
    public getServices(): Observable<IService[]> {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/Service/GetAll?detallado=false';
        if (this._token != null) {
            const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
            const options = new RequestOptions({ headers: headers });
            // console.log('Usando Token ' + this._token);
            return this._httpService.get(this._direccionServicio, options)
                .map((response) => {
                    return this.processGetServices(response);
                }).catch((response: any, caught: any) => {
                    if (response instanceof Response) {
                        try {
                            return Observable.of(this.processGetServices(response));
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
    * Procesa la respuesta obtenida al llamar al metodo /Service/GetAll del WebApi
    */
    protected processGetServices(response: Response): IService[] {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            this._services = response.json().listObjectResponse;
            return this._services;
        } else if (status !== 200 && status !== 204) {
            if (status === 408) {
                this._router.navigate(['/landing',
                 'Error al consultar los Servicios' +
                 ', la consulta esta tomando demasiado tiempo, por favor verifique su conexión de internet']);
            }
            this._router.navigate(['/landing', 'Error al consultar los Servicios disponibles']);
            return null;
        }
        return null;
    }

    /*
    *Realiza el Request del metodo Post /service/Post en el WebApi
    */
    public createQuickService(service: Service): Observable<IService> {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/service/Post';
        if (this._token != null) {
            const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
            const options = new RequestOptions({ headers: headers });
            const body = JSON.stringify(service);
            return this._httpService.post(this._direccionServicio, body, options)
                .map((response) => {
                    return this.processCreateQuickService(response);
                }).catch((response: any, caught: any) => {
                    if (response instanceof Response) {
                        try {
                            return Observable.of(this.processCreateQuickService(response));
                        } catch (e) {
                            return <Observable<IService>><any>Observable.throw(e);
                        }
                    } else {
                        return <Observable<IService>><any>Observable.throw(response);
                    }
                });
        } else {
            // El token del usuario no existe o es invalido
            console.log('Usuario no Logeado');
            return Observable.throw('Usuario no Logeado');
        }
    }

    /*
    * Procesa la respuesta obtenida al llamar al metodo /service/Post del WebApi
    */
    protected processCreateQuickService(response: Response): IService {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            let service = response.json().objectResponse;
            if (service) {
                // console.log('Servicio añadido');
                return service;
            }
        } else if (status !== 200 && status !== 204) {
            return null;
        }
        return null;
    }

}
