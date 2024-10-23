import {MyBaseEndpoint} from "../MyBaseEndpoint";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {MojConfig} from "../../moj-config";
import {Injectable} from "@angular/core";
@Injectable({providedIn: 'root'})
export class LockUnlockEndpoint implements MyBaseEndpoint<LockUnlockRequest, number>{
  constructor(public httpClient:HttpClient) {
  }
  obradi(request: LockUnlockRequest): Observable<number> {
    let url=MojConfig.adresa_servera+`/api/LockEvents/WebApp`;

    return this.httpClient.post<number>(url, request);
  }
}
export interface LockUnlockRequest {
  status: string
}
