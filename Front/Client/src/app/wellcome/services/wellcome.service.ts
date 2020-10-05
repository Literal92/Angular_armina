import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParameterCodec } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';

//const headers = new HttpHeaders({ "Content-Type": "application/json;charset=utf-8" });

const headers = new HttpHeaders({ "Content-Type": "application/json;charset=utf-8" });

@Injectable({
  providedIn: 'root'
})
export class WellComeService {
  constructor(private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }


}
