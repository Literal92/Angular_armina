import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import {Headers } from '@angular/http';
const headers = new Headers({ "Content-Type": "application/json" });

@Injectable()
export class ChangePasswordAdminService {

  //#region Ctor
  constructor(private http: HttpClient/*,protected http: HttpResponseInterceptor*/,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }
  //#endregion
  changePassword(serialNumber: string, password:string): Observable<any> {
    const headers = new HttpHeaders({ "Content-Type": "application/json" });
    const url = `${this.appConfig.apiEndpointCommon}/Account/ChangePassword`;
    return this.http
      .post(url, { item1: serialNumber , item2:password }, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  //changePassword(model: ChangePassword) {

  //  const url = `${this.appConfig.apiEndpoint}/ChangePassword`;
  //  return this.http.post(url, model, { headers: headers })
  //    .pipe(
  //      map((res:Response) => res.json() || {}),
  //    //catchError((error: HttpErrorResponse) => throwError(error))
  //     catchError(err => { return throwError(err); })

  //    );
  //}

  //constructor(protected http: HttpInterceptor, private router: Router) {
  //  super(http);
  //}
  //Get(id?: any, pagesize?: number, pageIndex?: number) {
  //  if (id != undefined) {
  //    var url = '/api/admin/categoryPrd/GetAsync';
  //    return this.http.post(url, { item1: id }, { headers: headers })
  //      .pipe(
  //        map((res: Response) => res.json())
  //        , catchError(err => {return throwError(err);})
  //      );
  //  }
  //}
}
