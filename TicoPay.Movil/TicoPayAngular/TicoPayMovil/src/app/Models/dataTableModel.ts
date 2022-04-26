import { Observable } from 'rxjs/Observable';

export class DataTableColumn {
    public name: string;
    public header: string;
    public type: ColumnType;
    public filter: ColumnFilterType;
  }

  export class DataTableConfig {
    public title: string;
    public data: Observable<any[]>;
    public columns: DataTableColumn[];
    public details: tableDetailType;
    public sortBy: DataTableColumn;
    public pageSize: number;
    public showSort: boolean;
  }

  export enum ColumnType {
      Text = 'Text',
      Numeric = 'Numeric',
      Currency = 'Currency',
      Date = 'Date',
      DateTime = 'DateTime',
      Time = 'Time',
      Boolean = 'Boolean',
  }

  export enum ColumnFilterType {
    None = 'None',
    Equals = 'Equals',
    Contains = 'Contains',
    NotContains = 'NotContains',
}

export enum tableDetailType {
    InvoiceDetail = 'InvoiceDetail',
}

