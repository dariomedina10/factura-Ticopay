import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { Routes, RouterModule} from '@angular/router';
import { HomeComponent } from '../accounts/home/home.component';
import { InvoiceIndexComponent } from './invoice-index/invoice-index.component';
import { CreateInvoiceComponent } from './create-invoice/create-invoice.component';
import { HttpModule } from '@angular/http';
import { ClientsService } from '../shared/dataServices/clients.service';
import { ServicesService } from '../shared/dataServices/services.service';
import { ClientsModule } from '../clients/clients.module';
import { ServicesModule } from '../services/services.module';
import { InvoiceService } from '../shared/dataServices/invoice.service';
import { InvoiceDetailComponent } from './invoice-detail/invoice-detail.component';
import { NotificationsService } from '../shared/notifications/notifications.service';
import { LoaderService } from '../shared/loader/loader.service';
import { DataTablesModule } from 'angular-datatables';
import { AuthGuardService } from '../shared/guardServices/auth-guard.service';
import { CurrencyService } from '../shared/currency.service';
import { ProductsService } from '../shared/dataServices/products.service';
import { ProductsModule } from '../products/products.module';
import { TicketsService } from '../shared/dataServices/tickets.service';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    HttpModule,
    ClientsModule,
    ServicesModule,
    ProductsModule,
    DataTablesModule,
    RouterModule.forChild([
      {path: 'home', canActivate: [AuthGuardService], component: HomeComponent,
       children: [
        {path: 'createInvoice', component: CreateInvoiceComponent},
        {path: 'invoiceDetail/:id', component: InvoiceDetailComponent},
        {path: 'invoiceIndex', component: InvoiceIndexComponent}
       ]
    }
    ])
  ],
  declarations: [
    InvoiceIndexComponent,
    CreateInvoiceComponent,
    InvoiceDetailComponent
  ],
  providers : [ClientsService, ServicesService, ProductsService, InvoiceService, NotificationsService,
     LoaderService, AuthGuardService, CurrencyService, TicketsService],
  exports: []
})
export class InvoicesModule { }
