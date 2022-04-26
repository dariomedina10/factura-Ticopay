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
import { ITaxes } from '../../Models/TaxesModel';

@Injectable()
export class TaxesService {

    // Direccion del WebApi Service: ruta completa en el archivo appConst.ts
    private _direccionServicio: string;
    private _token: string;
    private _tenancyName: string;
    private _services: ITaxes[];

    constructor(private _httpService: Http, private _router: Router) {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
    }

    /*
 *Realiza el Request del metodo Get /Tax/GetAll en el WebApi
 */
    public getTaxes(): Observable<ITaxes[]> {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/Tax/GetAll?detallado=false';
        if (this._token != null) {
            const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
            const options = new RequestOptions({ headers: headers });
            return this._httpService.get(this._direccionServicio, options)
                .map((response) => {
                    return this.processGetTaxes(response);
                }).catch((response: any, caught: any) => {
                    if (response instanceof Response) {
                        try {
                            return Observable.of(this.processGetTaxes(response));
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
    * Procesa la respuesta obtenida al llamar al metodo /Tax/GetAll del WebApi
    */
    protected processGetTaxes(response: Response): ITaxes[] {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            this._services = response.json().listObjectResponse;
            return this._services;
        } else if (status !== 200 && status !== 204) {
            if (status === 408) {
                this._router.navigate(['/landing',
                 'Error al consultar los Impuestos' +
                 ', la consulta esta tomando demasiado tiempo, por favor verifique su conexión de internet']);
            }
            this._router.navigate(['/landing', 'Error al consultar los Impuestos']);
            return null;
        }
        return null;
    }
}
