import { Pipe } from '@angular/core';
import { DatePipe } from '@angular/common';
import * as moment from 'jalali-moment';

@Pipe({
  name: 'subServicePipe'
})
export class SubServicePipe {
  constructor() {}
  transform(service: any, list: any[]=[]) {
    if (list.length > 0) {
      if (service != undefined) {
        let index = list.findIndex(x => x.id == service.id);
        if (index != -1) {
          return true;
        }
      }
    }
    return false;
  }
}
