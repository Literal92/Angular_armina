import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';
import { Router } from '@angular/router';
import { sortProvider } from 'src/app/shared/enum/orderProvider';
import { iorderSearch } from '../interfaces/iorderSearch';
import { iEditAddress } from '../interfaces/iEditAddress';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn: 'root'
})
  // بعدا نام این سرویس اصلاح شود و به
  //orderService
// تغییر کند
export class OrderService {

  constructor(private http: HttpClient, private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }

  get(model: iorderSearch) {
    let url = `${this.appConfig.apiEndpointProvider}/order/Get?`;

    if (model.id !== undefined)
      url += "id=" + encodeURIComponent("" + model.id) + "&";
    if (model.username !== undefined)
      url += "username=" + encodeURIComponent("" + model.username) + "&";
    if (model.from !== undefined)
      url += "from=" + encodeURIComponent("" + model.from) + "&";
    if (model.to !== undefined)
      url += "to=" + encodeURIComponent("" + model.to) + "&";

    if (model.productName !== undefined)
      url += "productName=" + encodeURIComponent("" + model.productName) + "&";

    if (model.orderType !== undefined)
      url += "orderType=" + encodeURIComponent("" + model.orderType) + "&";

    if (model.orderSendType !== undefined)
      url += "orderSendType=" + encodeURIComponent("" + model.orderSendType) + "&";

    if (model.reciverName !== undefined)
      url += "reciverName=" + encodeURIComponent("" + model.reciverName) + "&";

    if (model.reciverMobile !== undefined)
      url += "reciverMobile=" + encodeURIComponent("" + model.reciverMobile) + "&";

    if (model.senderName !== undefined)
      url += "senderName=" + encodeURIComponent("" + model.senderName) + "&";

    if (model.senderMobile !== undefined)
      url += "senderMobile=" + encodeURIComponent("" + model.senderMobile) + "&";

    if (model.address !== undefined)
      url += "address=" + encodeURIComponent("" + model.address) + "&";




    if (model.pageIndex !== undefined)
      url += "pageIndex=" + encodeURIComponent("" + model.pageIndex) + "&";
    if (model.pageSize !== undefined)
      url += "pageSize=" + encodeURIComponent("" + model.pageSize) + "&";
    url = url.replace(/[?&]$/, "");

    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }


  getOrderProduct(id: number) {
    let url = `${this.appConfig.apiEndpointProvider}/order/GetOrderProduct?`;


    url += "orderid=" + encodeURIComponent("" + id);
    url = url.replace(/[?&]$/, "");

    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }


  post(data: any) {
    let url = `${this.appConfig.apiEndpointProvider}/product/create`;
    return this.http
      .post(url, data, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }

  deleteByOrderProductId(id: number) {
    let url = `${this.appConfig.apiEndpointProvider}/order/DeleteByOrderProductId/${id}`;
    return this.http
      .delete(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }

  put(id: number, data: any) {
    let url = `${this.appConfig.apiEndpointProvider}/product/update/${id}`;
    return this.http
      .put(url, data, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  delete(id: number) {
    let url = `${this.appConfig.apiEndpointProvider}/product/delete/${id}`;
    return this.http
      .delete(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }

  complete(model: any) {
    let url = `${this.appConfig.apiEndpointProvider}/order/complete`;
    return this.http
      .post(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }


  senderType(model: any) {
    let url = `${this.appConfig.apiEndpointProvider}/order/ChangeSenderType`;
    return this.http
      .post(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  getOrdersByAddress() {
    let url = `${this.appConfig.apiEndpointProvider}/order/GetOrdersByAddress`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  editAddress(model: iEditAddress) {
    let url = `${this.appConfig.apiEndpointProvider}/order/EditAddress/${model.id}`;
    return this.http
      .put(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  AcceptPayment(id: number, accept: boolean) {
    let url = `${this.appConfig.apiEndpointProvider}/order/AcceptPaymnet/${id}`;
    let model = { id: id, accept: accept };
    return this.http
      .put(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  GetDailyReport(from, to) {
    let url = `${this.appConfig.apiEndpointProvider}/order/GetDailyReport?`;

    if (from !== undefined && from != "null")
      url += "from=" + encodeURIComponent("" + from) + "&";

    if (to !== undefined && to != "null")
      url += "to=" + encodeURIComponent("" + to) + "&";

    url = url.replace(/[?&]$/, "");

    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  GetMonthReport(from, to, year) {
    let url = `${this.appConfig.apiEndpointProvider}/order/GetMonthReport?`;

    if (from !== undefined && from != "null")
      url += "from=" + encodeURIComponent("" + from) + "&";

    if (to !== undefined && to != "null")
      url += "to=" + encodeURIComponent("" + to) + "&";

    if (year !== undefined && year != "null")
      url += "year=" + encodeURIComponent("" + year) + "&";

    url = url.replace(/[?&]$/, "");

    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }

}
