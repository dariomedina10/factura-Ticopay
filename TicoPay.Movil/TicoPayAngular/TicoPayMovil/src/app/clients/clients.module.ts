import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { Routes, RouterModule} from '@angular/router';
import { HomeComponent } from '../accounts/home/home.component';
import { HttpModule } from '@angular/http';
import { ClientsService } from '../shared/dataServices/clients.service';
import { CreateClientComponent } from './create-client/create-client.component';

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
  declarations: [
    CreateClientComponent
  ],
  providers : [ClientsService],
  exports : [CreateClientComponent]
})
export class ClientsModule { }
