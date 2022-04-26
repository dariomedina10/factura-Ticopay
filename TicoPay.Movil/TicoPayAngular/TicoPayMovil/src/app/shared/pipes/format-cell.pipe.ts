import { Pipe, PipeTransform } from '@angular/core';
import { ColumnType } from '../../Models/dataTableModel';

@Pipe({
  name: 'formatCell'
})
export class FormatCellPipe implements PipeTransform {

  transform(value: any, Blank?: boolean, Mobile?: boolean): any {
    if (( value === undefined ) || (value === null)) {
      if (Blank === true) {
        return '';
      } else {
        return 'N/D';
      }
    }
    return value;
  }

}
