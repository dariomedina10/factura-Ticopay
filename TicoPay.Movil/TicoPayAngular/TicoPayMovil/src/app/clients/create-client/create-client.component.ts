import { Component, OnInit, Injector, ElementRef, ViewChild, Output, EventEmitter } from '@angular/core';
import { IClient, IdentificationType, Client } from '../../Models/ClientModel';
import { NgForm } from '@angular/forms';
import { ClientsService } from '../../shared/dataServices/clients.service';
import { NotificationsService } from '../../shared/notifications/notifications.service';
import { AuthService } from '../../shared/dataServices/auth-service.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-quickclient',
  templateUrl: './create-client.component.html',
  styleUrls: ['./create-client.component.css']
})
export class CreateClientComponent implements OnInit {
  private _client: Client;
  public identificationType = IdentificationType;
  // Parámetro para devolver el cliente nuevo a Parent Component
  @Output() private notifyClientCreated: EventEmitter<IClient> = new EventEmitter();

  constructor(private clientService: ClientsService, protected notificationsService: NotificationsService,
    private authService: AuthService, private router: Router) {
    this._client = new Client();
    this._client.name = '';
    this._client.lastName = '';
    this._client.email = '';
    this._client.identification = '';
    this._client.identificacionExtranjero = '';
    this._client.identificationType = undefined;
  }

  /*
  *  Método que llama al servicio ClientService para crear un nuevo cliente Rápido
  * */
  submitQuickClient(createQuickClientForm: NgForm) {
    if (createQuickClientForm && createQuickClientForm.valid) {
      // Validaciones
      // Validación de Tipo de Cedula
      if (this._client.identificationType === undefined) {
        return this.notificationsService.addWarning('Debe seleccionar el tipo de documento de identificación');
      }
      // Validación de Cedula Física
      if (this._client.identificationType === IdentificationType.CedulaFisica) {
        if (this._client.identification.length > 9) {
          return this.notificationsService.addWarning('El número de Cédula física, debe contener 9 dígitos');
        }
        if (this._client.identification.indexOf('0') === 0) {
          return this.notificationsService.addWarning('El número de Cédula física, no debe tener un 0 al inicio');
        }
      }
      // Validación de Cedula Jurídica
      if (this._client.identificationType === IdentificationType.CedulaJuridica) {
        if ((this._client.identification.length < 10) || (this._client.identification.length > 10)) {
          return this.notificationsService.addWarning('El número de cédula jurídica, debe contener 10 dígitos');
        }
        if (this._client.lastName === undefined) {
          this._client.lastName = null;
        }
      }
      // Validación de Dimex
      if (this._client.identificationType === IdentificationType.DIMEX) {
        if (this._client.identification.length < 11) {
          return this.notificationsService.addWarning('El número DIMEX, debe contener 11 0 12 dígitos');
        }
        if (this._client.identification.indexOf('0') === 0) {
          return this.notificationsService.addWarning('El número DIMEX, no debe tener un 0 al inicio');
        }
      }
      // Validación de Nite
      if (this._client.identificationType === IdentificationType.NITE) {
        if (this._client.identification.length !== 10) {
          return this.notificationsService.addWarning('El número NITE, debe contener 10 dígitos');
        }
      }
      this.authService.RefreshToken()
        .subscribe(tokenActive => {
          if (tokenActive) {
            // Request al servidor
            this.clientService.createQuickClient(this._client)
              .subscribe((response: IClient) => {
                const client = response;
                if (client) {
                  this.notificationsService.addInfo('Cliente creado exitosamente');
                  // Devuelve el nuevo cliente mediante un evento al Parent Component
                  this.notifyClientCreated.emit(client);
                } else {
                  // Error en la creación , mostrar un mensaje de error de validación
                  // this.notificationsService.addError('Error al crear el cliente');
                  // console.log('Fallo la creación de cliente');
                }
              }, error => console.log(error)
              );
            // Fin de los Request
          } else {
            const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
            this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
          }
        },
        serviceError => console.log(serviceError));
    }
  }

  ngOnInit() {
  }

}
