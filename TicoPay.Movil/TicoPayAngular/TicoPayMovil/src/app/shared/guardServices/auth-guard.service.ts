import { Injectable } from '@angular/core';
import { CanActivate, CanLoad } from '@angular/router/src/interfaces';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { UserCredentials } from '../../Models/Authentificatiomodel';

@Injectable()
export class AuthGuardService implements CanActivate {

  private _user: UserCredentials;

  constructor(private _router: Router) { }

 canActivate(Route: ActivatedRouteSnapshot): boolean {
   // Chequea que exista una sesion almacenada en los cookies
  if (localStorage.getItem('token')) {
    // Si existe , chequea que el token este vigente
    this._user = JSON.parse(localStorage.getItem('token'));
    return true;
  } else {
    // Si no existe sesion en los cookies redirecciona al login
    const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
    this._router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
    return false;
  }
}
}
