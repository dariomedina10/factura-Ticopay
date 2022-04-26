import {Component, OnDestroy, OnInit} from '@angular/core';
import {Subscription} from 'rxjs/Subscription';
import {Router, NavigationStart} from '@angular/router';
import 'rxjs/add/operator/filter';
import { NotificationMessage } from '../../../Models/NotificationModel';
import { NotificationsService } from '../notifications.service';

@Component({
  selector: 'app-notification-bar',
  templateUrl: './notification-bar.component.html',
  styleUrls: ['./notification-bar.component.css']
})
export class NotificationBarComponent implements OnInit, OnDestroy {

  protected sub: Subscription;
  protected routerSub: Subscription;
  protected notifications: NotificationMessage[] = [];

    protected render(notification) {
        this.notifications.push(notification);
    }

    protected findSimilar(notification: NotificationMessage) {
        return this.notifications.find(existingNotification => {
            return existingNotification.message === notification.message
                && existingNotification.type === notification.type;
        });
    }

    constructor(protected notificationsService: NotificationsService,
                protected router: Router) {
    }

    ngOnInit() {
        this.sub = this.notificationsService.notifications
            .subscribe((n: NotificationMessage) => {
                if (this.findSimilar(n)) {
                    return;
                }

                this.render(n);
                // Tiempo antes de cerrar la notificacion Toast
                setTimeout(function() {
                    this.notifications.splice(this.notifications.indexOf(n), 1);
                }.bind(this), 10000);
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

    onDismissNotify(n: NotificationMessage) {
        return this.notifications.splice(
            this.notifications.indexOf(n),
            1
        );
    }

}
