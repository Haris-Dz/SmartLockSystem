import {Injectable} from "@angular/core";
import {MyBaseEndpoint} from "../MyBaseEndpoint";
import {MojConfig} from "../../moj-config";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Injectable({providedIn: 'root'})
export class LogsGetEnpoint implements MyBaseEndpoint<void, LogsGetResponse>{
  constructor(public httpClient:HttpClient) {
  }
  obradi(request: void): Observable<LogsGetResponse> {
    let url = MojConfig.adresa_servera+`/api/LockEvents`;

    return this.httpClient.get<LogsGetResponse>(url);
  }

}
export interface LogsGetResponse{
  logs:LogsGetResponseLogs[];
}
export interface LogsGetResponseLogs {
  status: string,
  timestamp: string
}
