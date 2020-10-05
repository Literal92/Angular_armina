import { Pipe } from '@angular/core';
import { DatePipe } from '@angular/common';
import * as moment from 'jalali-moment';

@Pipe({
  name: 'dateGtF'
})
export class DateGTF {
  constructor(private datePipe: DatePipe) {}
  transform(date: any) {
   // let utc = Date.now();
   // let date = this.datePipe.transform(value, 'yyyy/MM/dd');
    let toPersian = moment(date, 'YYYY/MM/DD').locale('fa').format('YYYY/MM/DD');
    return toPersian;
  }
}
