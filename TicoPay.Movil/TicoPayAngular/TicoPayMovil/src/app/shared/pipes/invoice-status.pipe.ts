import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'invoiceStatus'
})
export class InvoiceStatusPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    if (value === 0) {
      return 'Pagada';
    }
    if (value === 1) {
      return 'Provisional';
    }
    if (value === 2) {
      return 'Contabilizada';
    }
    if (value === 3) {
      return 'Reversada';
    }
    if (value === 4) {
      return 'Pendiente';
    }
    if (value === 5) {
      return 'Anulada';
    }
  }

}
