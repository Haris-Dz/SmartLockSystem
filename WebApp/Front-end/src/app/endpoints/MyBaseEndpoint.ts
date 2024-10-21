import {Observable} from "rxjs";

export interface MyBaseEndpoint<TRequest, TResponse>{
  obradi(request: TRequest): Observable<TResponse>;
}
