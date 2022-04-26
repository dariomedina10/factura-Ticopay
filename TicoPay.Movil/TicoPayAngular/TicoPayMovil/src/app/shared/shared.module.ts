import { NgModule, ModuleWithProviders} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NotificationWindowComponent } from './notifications/notification-window/notification-window.component';
import { NotificationBarComponent } from './notifications/notification-bar/notification-bar.component';
import { NotificationsService } from './notifications/notifications.service';
import { RouterModule } from '@angular/router';
import { MenuComponent } from './notifications/menu/menu.component';
import { LoaderComponent } from './loader/loader/loader.component';
import { LoaderService } from './loader/loader.service';
import { HomeComponent } from '../accounts/home/home.component';
import { InvoiceDetailComponent } from '../Invoices/invoice-detail/invoice-detail.component';
import { DataTableComponent } from './dataTable/data-table/data-table.component';
import { DataRowComponent } from './dataTable/data-row/data-row.component';
import { FormatCellPipe } from './pipes/format-cell.pipe';
import { InvoicesModule } from '../Invoices/invoices.module';
import { InvoiceStatusPipe } from './pipes/invoice-status.pipe';
import { TipoComprobantePipe } from './pipes/tipo-comprobante.pipe';

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild([
      {path: 'home', component: HomeComponent,
       children: [
        {path: 'invoiceDetail/:id', component: InvoiceDetailComponent}
       ]
    }
    ])
  ],
  declarations: [
  NotificationBarComponent,
  NotificationWindowComponent,
  MenuComponent,
  LoaderComponent,
  DataTableComponent,
  DataRowComponent,
  FormatCellPipe,
  InvoiceStatusPipe,
  TipoComprobantePipe
],
  exports: [
    CommonModule,
    FormsModule,
    NotificationBarComponent,
    NotificationWindowComponent,
    MenuComponent,
    LoaderComponent,
    FormatCellPipe,
    InvoiceStatusPipe,
    TipoComprobantePipe
  ],
  providers: [NotificationsService, LoaderService]
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
        ngModule: SharedModule,
        providers: [
            NotificationsService
        ]
    };
}
 }
