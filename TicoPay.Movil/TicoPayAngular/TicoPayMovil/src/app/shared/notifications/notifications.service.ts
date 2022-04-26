import {Injectable} from '@angular/core';
import {Subject} from 'rxjs/Subject';
import { NotificationMessage, NotificationType } from '../../Models/NotificationModel';

@Injectable()
export class NotificationsService {

  constructor() {
    this.notifications = new Subject();
}

public notifications: Subject<NotificationMessage>;

private pushNotification(notification) {
    this.notifications.next(notification);
}

public addInfo(text: string) {
    const notification = new NotificationMessage();

    notification.message = text;
    notification.type = NotificationType.Success;

    this.pushNotification(notification);
}

public addError(text: string) {
    const notification = new NotificationMessage();

    notification.message = text;
    notification.type = NotificationType.Error;

    this.pushNotification(notification);
}

public addWarning(text: string) {
    const notification = new NotificationMessage();

    notification.message = text;
    notification.type = NotificationType.Warning;

    this.pushNotification(notification);
}

public addroute(text: string, route: string, parameter: string, routeText: string) {
    const notification = new NotificationMessage();

    notification.message = text;
    notification.type = NotificationType.Route;
    notification.route = route;
    notification.parameter = parameter;
    notification.routeText = routeText;

    this.pushNotification(notification);
}

}
