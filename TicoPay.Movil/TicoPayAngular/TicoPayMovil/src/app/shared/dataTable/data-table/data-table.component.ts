import { Component, OnInit, OnChanges, Input } from '@angular/core';
import { DataTableConfig } from '../../../Models/dataTableModel';
import { NotificationsService } from '../../notifications/notifications.service';

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.css']
})
export class DataTableComponent implements OnInit, OnChanges {

  @Input() private tableConfiguration: DataTableConfig;
  private _data: any[];

  constructor(private notificationsService: NotificationsService) { }

  ngOnInit() {
    this.tableConfiguration.data.subscribe(
      data => {
        this._data = data;
        },
        serviceError => this.notificationsService.addError('No se puede leer la data de la tabla')
    );
  }

  ngOnChanges() {}

}
