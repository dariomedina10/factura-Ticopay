import {Component, OnDestroy, OnInit, EventEmitter, Input, Output} from '@angular/core';
import {Subscription} from 'rxjs/Subscription';
import {Router, NavigationStart} from '@angular/router';
import 'rxjs/add/operator/filter';
import { NotificationMessage } from '../../../Models/NotificationModel';
import { NotificationsService } from '../notifications.service';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
  selector: 'app-notifications-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit, OnDestroy {

  protected sub: Subscription;
  protected routerSub: Subscription;
  protected notifications: NotificationMessage[] = [];
  @Output() private notifyMessage: EventEmitter<number> = new EventEmitter();
  private _numerOfNotifications: number;
  protected dismissNotify = new EventEmitter();

  constructor(protected notificationsService: NotificationsService,
    protected router: Router) {}

    protected render(notification: NotificationMessage) {
        this.notifications.push(notification);
    }

    ngOnInit() {
        this._numerOfNotifications = 0;
        this.sub = this.notificationsService.notifications
            .subscribe((newMessage: NotificationMessage) => {
                /*
                if (this.findSimilar(n)) {
                    return;
                }
                */
                this.render(newMessage);
                this._numerOfNotifications++;
                this.notifyMessage.emit(this._numerOfNotifications);
            });
        this.routerSub = this.router.events
            .filter(event => event instanceof NavigationStart)
            .subscribe((e) => {
                this.notifications = [];
            });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
        this.routerSub.unsubscribe();
    }

    dismissAll() {
        for (const dismisedNotification of this.notifications) {
            this.notifications.splice(this.notifications.indexOf(dismisedNotification), 1);
        }
        this._numerOfNotifications = 0;
        this.notifyMessage.emit(this._numerOfNotifications);
    }

    onDismissNotify(dismisedNotification: NotificationMessage) {
        this._numerOfNotifications--;
        this.notifications.splice(this.notifications.indexOf(dismisedNotification), 1);
        this.notifyMessage.emit(this._numerOfNotifications);
    }

    navigate(notification: NotificationMessage) {
        this.router.navigate([notification.route, notification.parameter]);
      }

    /*
    protected findSimilar(notification: NotificationMessage) {
        return this.notifications.find(existingNotification => {
            return existingNotification.message === notification.message
                && existingNotification.type === notification.type;
        });
    }
    */

}
