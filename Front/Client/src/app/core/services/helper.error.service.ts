import { ErrorHandler, Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { NotifierService } from 'angular-notifier';
import { error } from 'util';
@Injectable()
export class HelperError {
  public readonly notifier: NotifierService;

  constructor(protected router: Router, protected route: ActivatedRoute, notifierService: NotifierService) {
    this.notifier = notifierService;

  }
  public error(state: number, errBody: any,flag ?:any) {
    switch (state) {
      case 400: //badrequest
        {
          let msgError = 'BadRequest >>' + errBody;
          this.notifyError(msgError);
          console.log(msgError);
          break;
        }
      case 401:// unauthorized
        {
          let msgError = 'Unauthorized >>' + errBody;

          this.notifyError(msgError);
          console.log(msgError);
          this.router.navigate(['/login']);
          break;
        }
      case 403://forbidden
        {
          let msgError = 'Forbidden >>' + errBody;

          this.notifyError(msgError);
          console.log(msgError);
          this.router.navigate(['/accessDenied']);
          break;

        }
      case 404:// notfound
        {
          // notfound with status 200
          if (flag == 'NotFound') {
            let msgError = 'NotFoundApi >>' + errBody;

            this.notifyError(msgError);
            console.log(msgError);
            break;
          }
          // other
          else {
            let msgError = 'NotFound >>' + errBody;
            // got to page notfound
            this.notifyError(msgError);
            console.log(msgError);
            break;
          }
         
        }
      case 409: //conflict
        {
          let msgError = 'Conflict >>' + errBody;

          this.notifyError(msgError);
          console.log(msgError);
          break;

        }
      case 422: //UnprocessableEntity
        {
          let msgError = 'UnprocessableEntity >>' + errBody;

          this.notifyError(msgError);
          console.log(msgError);
          break;
        }
      case 500:// internal server error
        {
          let msgError = 'Internal Server Error >>' + errBody;

          this.notifyError(msgError);
          console.log(msgError);
          break;
        }
      case 502:// get bad a way
        {
          let msgError = '502 >> خطا در برقراری ارتباط با سرور ';

          this.notifyError(msgError);
          console.log(msgError);
          break;
        }
      default:
        {
          let msgError = errBody;
          this.notifyError(msgError);
          console.log(msgError);
          break;
        }

    }

  }
  public notifyError(message: any = null) {

    this.notifier.show({
      type: 'error',
      message: message
    });
  }
}

