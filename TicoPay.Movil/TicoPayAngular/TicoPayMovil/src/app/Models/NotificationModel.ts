export class NotificationMessage {
    id: number;
    message: string;
    type: NotificationType;
    route: string;
    parameter: string;
    routeText: string;
}

export enum NotificationType {
    Error = 'error',
    Success = 'success',
    Warning = 'warning',
    Route = 'route',
}
