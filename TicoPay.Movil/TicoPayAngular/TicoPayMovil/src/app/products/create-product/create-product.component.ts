import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Product, IProduct, Estatus } from '../../Models/ProductModel';
import { ITaxes } from '../../Models/TaxesModel';
import { UnitMeasurement } from '../../Models/ServiceModel';
import { TaxesService } from '../../shared/dataServices/taxes.service';
import { AuthService } from '../../shared/dataServices/auth-service.service';
import { NotificationsService } from '../../shared/notifications/notifications.service';
import { Router } from '@angular/router';
import { ProductsService } from '../../shared/dataServices/products.service';

@Component({
  selector: 'app-create-quickproduct',
  templateUrl: './create-product.component.html',
  styleUrls: ['./create-product.component.css']
})
export class CreateProductComponent implements OnInit {

  private _product: Product;
  private _taxes: ITaxes[];
  private _taxErrorMessage: string;
  public unitMeasurement = UnitMeasurement;
  // Parámetro para devolver el cliente nuevo a Parent Component
  @Output() private notifyProductCreated: EventEmitter<IProduct> = new EventEmitter();

  constructor(private productService: ProductsService, private taxesService: TaxesService,
    private authService: AuthService, protected notificationsService: NotificationsService,
    private router: Router) {
    this._product = new Product();
    this._product.name = '';
    this._product.taxId = undefined;
    this._product.retailPrice = 0;
    this._product.unitMeasurement = UnitMeasurement.Unidad;
    this._product.estado = Estatus.Activo;
  }

  /*
  *  Método que llama al servicio ProductService para crear un nuevo producto Rápido
  * */
  submitQuickProduct(createQuickProductForm: NgForm) {
    if (createQuickProductForm && createQuickProductForm.valid) {
      // Validaciones
      if (this._product.taxId === undefined) {
        return this.notificationsService.addWarning('Debe seleccionar un tipo de impuesto');
      }
      // Validación de Precio del servicio
      if (this._product.retailPrice < 1) {
        return this.notificationsService.addWarning('El precio del producto debe ser mayor a 1');
      }
      this.authService.RefreshToken()
        .subscribe(tokenActive => {
          if (tokenActive) {
            // Request al servidor
            // Verificar si existe
            this.productService.getProducts()
            .subscribe(products => {
              if ( products.filter((consultedProduct: IProduct) =>
              consultedProduct.name.toLocaleLowerCase().indexOf(this._product.name.toLocaleLowerCase()) !== -1).length === 0) {
                // Crear Producto
                this.productService.createQuickProduct(this._product)
                  .subscribe((response: IProduct) => {
                  console.log(response);
                  const product = response;
                  if (product) {
                    // console.log('Producto creado');
                    // Devuelve el nuevo producto mediante un evento al Parent Component
                    this.notificationsService.addInfo('Producto creado exitosamente');
                    this.notifyProductCreated.emit(product);
                  } else {
                    // Error en la creación , mostrar un mensaje de error de validacion
                    this.notificationsService.addError('Error al crear el producto');
                    // console.log('Fallo la creación del producto');
                  }
                  }, error => console.log(error));
              } else {
                this.notificationsService.addError('El producto ya Existe');
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
