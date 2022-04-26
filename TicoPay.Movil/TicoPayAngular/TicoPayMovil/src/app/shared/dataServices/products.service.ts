import { Injectable } from '@angular/core';
import { IProduct, Product } from '../../Models/ProductModel';
import { Http, RequestOptions, Response, Headers } from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { AppConsts } from '../appConst';

@Injectable()
export class ProductsService {

  // Direccion del WebApi Service: ruta completa en el archivo appConst.ts
  private _direccionServicio: string;
  private _token: string;
  private _tenancyName: string;
  private _services: IProduct[];

  constructor(private _httpService: Http, private _router: Router) {
      const currentUser = JSON.parse(localStorage.getItem('token'));
      const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
      this._token = currentUser.token;
      this._tenancyName = currentTenant.tenancyName;
  }

  /*
  *Realiza el Request del metodo Get /Service/GetAll en el WebApi
  */
  public getProducts(): Observable<IProduct[]> {
      const currentUser = JSON.parse(localStorage.getItem('token'));
      const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
      this._token = currentUser.token;
      this._tenancyName = currentTenant.tenancyName;
      this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/Product/GetAll?detallado=true';
      if (this._token != null) {
          const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
          const options = new RequestOptions({ headers: headers });
          // console.log('Usando Token ' + this._token);
          return this._httpService.get(this._direccionServicio, options)
              .map((response) => {
                  return this.processGetProducts(response);
              }).catch((response: any, caught: any) => {
                  if (response instanceof Response) {
                      try {
                          return Observable.of(this.processGetProducts(response));
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
  * Procesa la respuesta obtenida al llamar al metodo /Product/GetAll del WebApi
  */
  protected processGetProducts(response: Response): IProduct[] {
      const responseText = response.text();
      const status = response.status;
      // console.log(response.json());
      if (status === 200) {
          this._services = response.json().listObjectResponse;
          return this._services;
      } else if (status !== 200 && status !== 204) {
          if (status === 408) {
              this._router.navigate(['/landing',
               'Error al consultar los Productos' +
               ', la consulta esta tomando demasiado tiempo, por favor verifique su conexión de internet']);
          }
          this._router.navigate(['/landing', 'Error al consultar los Productos disponibles']);
          return null;
      }
      return null;
  }

  /*
  *Realiza el Request del metodo Post /Product/Post en el WebApi
  */
  public createQuickProduct(product: Product): Observable<IProduct> {
      const currentUser = JSON.parse(localStorage.getItem('token'));
      const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
      this._token = currentUser.token;
      this._tenancyName = currentTenant.tenancyName;
      this._direccionServicio = AppConsts.UrlBase + AppConsts.ApiServiceUrlBase + '/Product/Post';
      if (this._token != null) {
          const headers = new Headers({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._token });
          const options = new RequestOptions({ headers: headers });
          const body = JSON.stringify(product);
          return this._httpService.post(this._direccionServicio, body, options)
              .map((response) => {
                  return this.processCreateQuickProduct(response);
              }).catch((response: any, caught: any) => {
                  if (response instanceof Response) {
                      try {
                          return Observable.of(this.processCreateQuickProduct(response));
                      } catch (e) {
                          return <Observable<IProduct>><any>Observable.throw(e);
                      }
                  } else {
                      return <Observable<IProduct>><any>Observable.throw(response);
                  }
              });
      } else {
          // El token del usuario no existe o es invalido
          console.log('Usuario no Logeado');
          return Observable.throw('Usuario no Logeado');
      }
  }

  /*
  * Procesa la respuesta obtenida al llamar al metodo /Product/Post del WebApi
  */
  protected processCreateQuickProduct(response: Response): IProduct {
      const responseText = response.text();
      const status = response.status;
      // console.log(response.json());
      if (status === 200) {
          const product = response.json().objectResponse;
          if (product) {
              // console.log('Servicio añadido');
              return product;
          }
      } else if (status !== 200 && status !== 204) {
          return null;
      }
      return null;
  }

}
