import {Component, OnInit} from '@angular/core';
import {LockUnlockEndpoint} from "../../endpoints/lock-unlock-endpoints/lock-unlock.endpoint";
import * as signalR from '@microsoft/signalr';
import {MojConfig} from "../../moj-config";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private hubConnection: signalR.HubConnection;
  public lockState: string = 'Unknown';
  public request:any=null;
  constructor(private lockUnlockEndpoint:LockUnlockEndpoint) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(MojConfig.adresa_servera+'/lockhub') // URL of your SignalR hub
      .build();

    this.hubConnection.on('ReceiveLockUpdate', (state: string) => {
      this.lockState = state;
      if (this.lockState==="Lock"){
        this.locked=true;
      }
      else if (this.lockState==="Unlock") this.locked=false;
      console.log(`Lock state updated: ${state}`);
    });
    this.hubConnection.start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));
  }
  locked:boolean=true;
  ngOnInit(): void {
    this.request =
      {
        status:""
      }
  }

  onLockChange() {
    this.request.status = this.locked ? "Lock" : "Unlock";
    this.lockUnlockEndpoint.obradi(this.request!).subscribe((x)=>{
      console.log(x);
    })
  }
}
