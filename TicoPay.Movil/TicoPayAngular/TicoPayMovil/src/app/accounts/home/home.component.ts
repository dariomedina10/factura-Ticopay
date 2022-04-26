import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { UserCredentials, ITenant } from '../../Models/Authentificatiomodel';
import { Router } from '@angular/router';
import { AuthService } from '../../shared/dataServices/auth-service.service';
import { NotificationsService } from '../../shared/notifications/notifications.service';

@Component({
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  private _user: UserCredentials;
  private _newNotifications: number;
  private _tenant: ITenant;
  private _error: string;

  constructor(private router: Router, private authService: AuthService, private notificationsService: NotificationsService) { }

  // Llama al método de cerrar sesión en auth-services
  logOut() {
    this.authService.logout();
    // const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
    // this.router.navigate(['/login', currentTenant]);
    const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
    this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
  }

  /*
  * Método que recibe la notification de un nuevo mensaje recibido por app-notifications-menu Directive
  */
  private onNotificationSent (newNotification: number) {
    this._newNotifications = newNotification;
  }

  ngOnInit() {
    this._newNotifications = 0;
    if (localStorage.getItem('token')) {
      // Coloca en nombre de usuario en el menu de usuario
      this._user = JSON.parse(localStorage.getItem('token'));
      /*
    * Carga de data inicial a los controles desde el Webapi
    */
    this.authService.RefreshToken()
    .subscribe(tokenActive => {
      if (tokenActive) {
        // Request al servidor
        // Llenar el combo de clientes
        this.authService.getTenant()
          .subscribe(tenant => {
            this._tenant = tenant;
            localStorage.removeItem('tenant');
            localStorage.setItem('tenant', JSON.stringify(tenant));
          },
          serviceError => this._error = <any>serviceError);
        // Fin de los Request
      } else {
        const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
        this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
      }
    },
    serviceError => this._error = <any>serviceError);
    } else {
      this.router.navigateByUrl('https://www.ticopays.com');
    }
  }

}
