import { Component, OnInit, OnDestroy } from '@angular/core';
import { LockUnlockEndpoint } from "../../endpoints/lock-unlock-endpoints/lock-unlock.endpoint";
import * as signalR from '@microsoft/signalr';
import { MojConfig } from "../../moj-config";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  private hubConnection: signalR.HubConnection | null = null;
  public lockState: string = 'Unknown';
  public locked: boolean = true;
  public request: any = { status: "" };

  constructor(private lockUnlockEndpoint: LockUnlockEndpoint) {}

  ngOnInit(): void {
    this.startSignalRConnection();
  }

  private startSignalRConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${MojConfig.adresa_servera}/lockhub`) // URL of your SignalR hub
      .withAutomaticReconnect() // Enable automatic reconnection
      .build();

    this.hubConnection.on('ReceiveLockUpdate', (state: string) => {
      this.lockState = state;
      this.locked = this.lockState === 'Lock';
      console.log(`Lock state updated: ${state}`);
    });

    this.hubConnection.start()
      .then(() => console.log('Connection started'))
      .catch(err => console.error('Error while starting connection: ', err));
  }

  onLockChange(): void {
    this.request.status = this.locked ? "Lock" : "Unlock";
    this.lockUnlockEndpoint.obradi(this.request).subscribe(
      response => console.log("Server response:", response),
      error => console.error("Error in lock change request:", error)
    );
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.off('ReceiveLockUpdate'); // Remove the event listener
      this.hubConnection.stop() // Stop the connection
        .then(() => console.log('Connection stopped'))
        .catch(err => console.error('Error while stopping connection: ', err));
    }
  }
}
