import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/observable/throw';
import 'rxjs/add/observable/of';
import { BodyInformation, RefreshCredentials, TimeLapsType, UserCredentials, ITenant } from '../../Models/Authentificatiomodel';
import { AppConsts } from '../appConst';
import { Console } from '@angular/core/src/console';
import { Token } from '@angular/compiler';


@Injectable(
)
export class AuthService {
    private _direccionServicio: string;
    private _tenancyName: string;
    public token: string;
    private _response: any;
    private _responseType: boolean;
    private _loginSuccess: boolean;
    private _token: string;
    private _currentCredentials: BodyInformation;

    constructor(private _httpService: Http) {
    }

    // funcion para autentificacion del usuario
    authentificateTenant(bodyInformation: BodyInformation): Observable<boolean> {
        // const currentUser = JSON.parse(localStorage.getItem('token'));
        // this.token = currentUser.token;
        this.token = null;
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + 'Account/InternalAuthenticate';
        if (this.token == null) {
            // Preparando contenido del http request Content-Type: application/json
            // Para debugear activar las etiquetas de console
            const headers = new Headers({ 'Content-Type': 'application/json' });
            const options = new RequestOptions({ headers: headers });
            const body = JSON.stringify(bodyInformation);
            // this._loginSuccess = false;
            // console.log(body);
            return this._httpService.post(this._direccionServicio, body, options)
                .map((response) => {
                    return this.processAuthenticate(response, bodyInformation);
                }).catch((response: any, caught: any) => {
                    if (response instanceof Response) {
                        try {
                            return Observable.of(this.processAuthenticate(response, bodyInformation));
                        } catch (e) {
                            return <Observable<boolean>><any>Observable.throw(e);
                        }
                    } else {
                        return <Observable<boolean>><any>Observable.throw(response);
                    }
                });
        } else {
            console.log('Token existe');
            return Observable.of(true);
        }
    }

    protected processAuthenticate(response: Response, bodyInformation: BodyInformation): boolean {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            let token = response.json().objectResponse.tokenAuthenticate;
            // console.log(token);
            if (token) {
                // Guardando el token del usuario mientras navega en la aplicacion
                localStorage.setItem('token', JSON.stringify({ userName: bodyInformation.usernameOrEmailAddress, token: token }));
                // console.log('Token obtenido y seteado');
            }
            // let result200: AuthenticateResultModel = null;
            // let resultData200 = responseText === "" ? null : JSON.parse(responseText, this.jsonParseReviver);
            // result200 = resultData200 ? AuthenticateResultModel.fromJS(resultData200) : new AuthenticateResultModel();
            return true;
        } else if (status !== 200 && status !== 204) {
            const errorType = response.json().error_msg;
            console.log(errorType);
            // this.throwException("An unexpected server error occurred.", status, responseText);
            return false;
        }
        return null;
    }

    /*
    *   Metodo para refrescar el token del usuario antes de realizar requests Account/RefreshToken
    */
    RefreshToken(): Observable<boolean> {
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._tenancyName = currentTenant.tenancyName;
        const currentUser = JSON.parse(localStorage.getItem('token'));
        this._token = currentUser.token;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + 'Account/RefreshToken';
        // Preparando contenido del http request Content-Type: application/json
        // Para debugear activar las etiquetas de console
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });
        const RefreshParameters = new RefreshCredentials(this._token, TimeLapsType.Hours, 12);
        const body = JSON.stringify(RefreshParameters);
        // console.log(body);
        return this._httpService.post(this._direccionServicio, body, options)
            .map((response) => {
                return this.processRefreshToken(response);
            }).catch((response: any, caught: any) => {
                if (response instanceof Response) {
                    try {
                        return Observable.of(this.processRefreshToken(response));
                    } catch (e) {
                        return <Observable<boolean>><any>Observable.throw(e);
                    }
                } else {
                    return <Observable<boolean>><any>Observable.throw(response);
                }
            });
    }

    /*
    *   Metodo para procesar el resquest del metodo de Refresh token
    */
    protected processRefreshToken(response: Response): boolean {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            const token = response.json().objectResponse.tokenAuthenticate;
            if (token) {
                // console.log('Token actual ' + this._token);
                // console.log('Token nuevo ' + token);
                if (token !== this._token) {
                    let user: UserCredentials;
                    user = JSON.parse(localStorage.getItem('token'));
                    localStorage.removeItem('token');
                    localStorage.setItem('token', JSON.stringify({ userName: user.userName, token: token }));
                    // console.log('Token Cambiado');
                    // console.log('Token nuevo ' + token);
                }
                return true;
            } else {
                return false;
            }
            // console.log(token);
        } else if (status !== 200 && status !== 204) {
            // this.throwException("An unexpected server error occurred.", status, responseText);
            return false;
        }
        return null;
    }

    /*
    *   Método para obtener la información del Tenant
    */
    public getTenant(): Observable<ITenant> {
        const currentUser = JSON.parse(localStorage.getItem('token'));
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this._token = currentUser.token;
        this._tenancyName = currentTenant.tenancyName;
        this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + 'Tenant/Get?Detallado=false';
        if (this._token != null) {
            const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
            const options = new RequestOptions({ headers: headers });
            // console.log('Usando Token ' + this._token);
            return this._httpService.get(this._direccionServicio, options)
                .map((response) => {
                    return this.processGetTenant(response);
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
    protected processGetTenant(response: Response): ITenant {
        const responseText = response.text();
        const status = response.status;
        // console.log(response.json());
        if (status === 200) {
            const tenant = response.json().objectResponse;
            return tenant;
        }
        return null;
    }


    logout(): void {
        // Remueve el usuario y su token del local storage para eliminar la sesion
        this.token = null;
        localStorage.removeItem('token');
    }

}




