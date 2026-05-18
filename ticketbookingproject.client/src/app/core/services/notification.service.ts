import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private hubConnection: signalR.HubConnection | undefined;

  public notificationReceived$ = new Subject<string>();

  constructor() { }

  public startConnection(token: string) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.api}/notificationHub`, {
        accessTokenFactory: () => token,
        //transport: signalR.HttpTransportType.WebSockets
      })
      //.configureLogging(signalR.LogLevel.Trace)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch((err) => console.log('Error while starting connection: ' + err));

    this.hubConnection.on('ReceiveNotification', (message: string) => {
      this.notificationReceived$.next(message);
    })
  };

  public stopConnection() {
      if (this.hubConnection)
        this.hubConnection.stop();
  }
}
