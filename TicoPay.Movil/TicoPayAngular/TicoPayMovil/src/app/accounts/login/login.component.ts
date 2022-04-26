import { Component, OnInit, Injector, ElementRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { AuthService } from '../../shared/dataServices/auth-service.service';
import { BodyInformation, Login } from '../../Models/Authentificatiomodel';
import { Injectable } from '@angular/core';


@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  private _errorMessage: string;
  private _tenancyName: string;
  pageTitle = 'Log In';
  private _loginSuccess: boolean;
  private _error: boolean;
  private _body: BodyInformation;
  private _token: string;

  loginInformation = new Login('', '');

  constructor(private authService: AuthService,
    private router: Router, private activatedRoute: ActivatedRoute) { }

  submitLoginInformation(loginForm: NgForm) {
    if (loginForm && loginForm.valid) {
      this._body = new BodyInformation(this._tenancyName, this.loginInformation.userName, this.loginInformation.password);
      this.authService.authentificateTenant(this._body)
        .subscribe((response: boolean) => {
          this._loginSuccess = response;
          if (this._loginSuccess) {
            this._error = false;
            this.router.navigate(['/home/invoiceIndex']);
          } else {
            this._error = true;
            this._errorMessage = 'Nombre de usuario o Contraseña inválidos';
          }
        }, error => console.log(error)
        );
      // console.log('Funcion de login ejecutada');
    }
  }
  ngOnInit() {
    // Obtener Tenancy del url de direccionamiento
    let tenancyName;
    this.activatedRoute.queryParams.subscribe((params: Params) => {
      tenancyName = params.tenancyName;
      console.log(tenancyName);
    });
    if (tenancyName === undefined) {
      localStorage.setItem('tenancyName', JSON.stringify({ tenancyName: 'ticopay' }));
    } else {
      localStorage.setItem('tenancyName', JSON.stringify({ tenancyName: tenancyName }));
    }
    const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
    this._tenancyName = currentTenant.tenancyName;
    const currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this._token = currentUser && currentUser.token;
    if (this._token) {
      this.router.navigate(['/home/invoiceIndex']);
    } else {
      this._errorMessage = null;
      this._loginSuccess = false;
      this._error = false;
    }
  }
}


