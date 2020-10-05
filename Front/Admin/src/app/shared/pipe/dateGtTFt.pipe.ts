import { Pipe } from '@angular/core';
import { DatePipe } from '@angular/common';
import * as moment from 'jalali-moment';

@Pipe({
  name: 'dateGtTFt'
})
export class DateGtTFt {
  constructor(private datePipe: DatePipe) {}
  transform(date: any, split?: any) {
    if (split == undefined) {
      try {
        let toPersian = moment(date, 'YYYY/MM/DD HH:mm:ss').locale('fa').format('YYYY/MM/DD HH:mm:ss');
      return toPersian;

      } catch  {
        return;
      }
    }
    else {
      let month = String(date.month);
      let day = String(date.day);
      if (month.length==1) {
        month = '0' + month;
      }
      if (day.length==1) {
        day = '0' + day;
      }
      let convert = '' + date.year + '' + month + '' + day;
      let toPersian = moment(convert, 'YYYY/MM/DD').locale('fa').format('YYYY/MM/DD');
      return toPersian;
    }
  }
}
