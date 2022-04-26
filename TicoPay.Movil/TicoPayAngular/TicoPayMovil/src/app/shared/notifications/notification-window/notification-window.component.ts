import {Component, EventEmitter, Input, Output} from '@angular/core';
import { NotificationMessage } from '../../../Models/NotificationModel';
import { Router } from '@angular/router';

@Component({
  selector: 'app-notification-window',
  templateUrl: './notification-window.component.html',
  styleUrls: ['./notification-window.component.css']
})
export class NotificationWindowComponent {

  @Input() notification: NotificationMessage;
  @Output() dismissNotify = new EventEmitter();

  constructor(private _router: Router) {}

    dismiss() {
        return this.dismissNotify.emit(this.notification);
    }

    navigate(notification: NotificationMessage) {
      this._router.navigate([notification.route, notification.parameter]);
    }

}
