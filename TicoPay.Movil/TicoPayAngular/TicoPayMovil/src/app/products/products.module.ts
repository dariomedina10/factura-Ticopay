import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { Routes, RouterModule} from '@angular/router';
import { HomeComponent } from '../accounts/home/home.component';
import { HttpModule } from '@angular/http';
import { CreateProductComponent } from './create-product/create-product.component';
import { TaxesService } from '../shared/dataServices/taxes.service';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    HttpModule,
    RouterModule.forChild([
      {path: 'home', component: HomeComponent,
       children: [
        // {path: 'createInvoice', component: CreateInvoiceComponent}
       ]
    }
    ])
  ],
  declarations: [CreateProductComponent],
  exports : [CreateProductComponent],
  providers : [TaxesService]
})
export class ProductsModule { }
