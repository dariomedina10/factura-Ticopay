import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'tipoComprobante'
})
export class TipoComprobantePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    if (value === 1) {
      return 'Factura';
    }
    if (value === 2) {
      return 'Nota de Crédito';
    }
    if (value === 3) {
      return 'Nota de Débito';
    }
    if (value === 4) {
      return 'Ticket';
    }
  }

}
