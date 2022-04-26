import { Component, OnInit, Injector, ElementRef, ViewChild, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Service, IService, UnitMeasurement } from '../../Models/ServiceModel';
import { ServicesService } from '../../shared/dataServices/services.service';
import { TaxesService } from '../../shared/dataServices/taxes.service';
import { ITaxes } from '../../Models/TaxesModel';
import { AuthService } from '../../shared/dataServices/auth-service.service';
import { NotificationsService } from '../../shared/notifications/notifications.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-quickservice',
  templateUrl: './create-service.component.html',
  styleUrls: ['./create-service.component.css']
})
export class CreateServiceComponent implements OnInit {

  private _service: Service;
  private _taxes: ITaxes[];
  private _taxErrorMessage: string;
  public unitMeasurement = UnitMeasurement;
  // Parámetro para devolver el cliente nuevo a Parent Component
  @Output() private notifyServiceCreated: EventEmitter<IService> = new EventEmitter();

  constructor(private serviceService: ServicesService, private taxesService: TaxesService,
    private authService: AuthService, protected notificationsService: NotificationsService,
    private router: Router) {
    this._service = new Service();
    this._service.name = '';
    this._service.taxId = undefined;
    this._service.price = 0;
    this._service.quantity = 1;
    this._service.isRecurrent = false;
    this._service.unitMeasurement = UnitMeasurement.ServiciosProfesionales;
  }

  /*
  *  Método que llama al servicio ServicesService para crear un nuevo servicio Rápido
  * */
  submitQuickService(createQuickServiceForm: NgForm) {
    if (createQuickServiceForm && createQuickServiceForm.valid) {
      // Validaciones
      if (this._service.taxId === undefined) {
        return this.notificationsService.addWarning('Debe seleccionar un tipo de impuesto');
      }
      // Validación de Precio del servicio
      if (this._service.price < 1) {
        return this.notificationsService.addWarning('El precio del servicio debe ser mayor a 1');
      }
      this.authService.RefreshToken()
        .subscribe(tokenActive => {
          if (tokenActive) {
            // Request al servidor
            // Buscar si ya Existe
            this.serviceService.getServices()
              .subscribe(services => {
                if (services.filter((consultedService: IService) =>
                consultedService.name.toLocaleLowerCase().indexOf(this._service.name.toLocaleLowerCase()) !== -1).length === 0) {
                  // Crear el Servicio
                  this.serviceService.createQuickService(this._service)
                    .subscribe((response: IService) => {
                    const service = response;
                    if (service) {
                      // console.log('Servicio creado');
                      // Devuelve el nuevo servicio mediante un evento al Parent Component
                      this.notificationsService.addInfo('Servicio creado exitosamente');
                      this.notifyServiceCreated.emit(service);
                    } else {
                      // Error en la creación , mostrar un mensaje de error de validacion
                      this.notificationsService.addError('Error al crear el servicio');
                      // console.log('Fallo la creación del servicio');
                    }
                    }, error => console.log(error)
                    );
                } else {
                  this.notificationsService.addError('El servicio ya Existe');
                }
            },
            error => console.log(error));
            // Fin de los Request
          } else {
            const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
            this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
          }
        },
          serviceError => console.log(serviceError));
    }
  }

  /*
  * Método para prevenir ingreso de valores mayores de 20 cifras (keypress)="keyPress($event)"
  */
  private onDecimalValueChange(inputValue: any) {
    // console.log(inputValue);
    if (inputValue.target.value.length > 12) {
      event.preventDefault();
    }
    if ((inputValue.keyCode === 45) || (inputValue.keyCode === 43) || (inputValue.keyCode === 44) || (inputValue.keyCode === 101)) {
      event.preventDefault();
    }
  }

  ngOnInit() {
    /*
    * Carga de data inicial a los controles desde el Webapi
    */
    this.authService.RefreshToken()
      .subscribe(tokenActive => {
        if (tokenActive) {
          // Request al servidor
          // Cargar los impuestos a la lista
          this.taxesService.getTaxes()
            .subscribe(taxes => {
              this._taxes = taxes;
            },
              serviceError => this._taxErrorMessage = <any>serviceError);
          // Fin de los Request
        } else {
          const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
          this.router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
        }
      },
        serviceError => console.log(serviceError));
  }

}
