import { Component, OnInit } from '@angular/core';
import {LogsGetEnpoint, LogsGetResponse, LogsGetResponseLogs} from "../../endpoints/logs-endpoints/logs-get.endpoint";

@Component({
  selector: 'app-lock-logs',
  templateUrl: './lock-logs.component.html',
  styleUrls: ['./lock-logs.component.css']
})
export class LockLogsComponent implements OnInit {

  constructor(private logsGetEnpoint:LogsGetEnpoint) { }
  logovi:LogsGetResponseLogs[]=[];
  ngOnInit(): void {
    this.fetchLogs();
  }
  fetchLogs(){
    this.logsGetEnpoint.obradi().subscribe((x:LogsGetResponse)=>{
      this.logovi=x.logs
    })
  }

}
