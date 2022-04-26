import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { Routes, RouterModule} from '@angular/router';
import { HomeComponent } from '../accounts/home/home.component';
import { HttpModule } from '@angular/http';
import { CreateServiceComponent } from './create-service/create-service.component';
import { CreateClientComponent } from '../clients/create-client/create-client.component';
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
  declarations: [CreateServiceComponent
  ],
  exports : [CreateServiceComponent],
  providers : [TaxesService]
})
export class ServicesModule { }
