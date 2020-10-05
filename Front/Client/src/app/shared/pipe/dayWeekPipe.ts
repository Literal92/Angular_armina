import { Pipe } from '@angular/core';
import { DatePipe } from '@angular/common';
import * as moment from 'jalali-moment';

@Pipe({
  name: 'dayWeekPipe'
})
export class DayWeekPipe {
  transform(dayWeek: number) {
    switch (dayWeek) {
      case 0:
        return 'یکشنبه';
      case 1:
        return 'دوشنبه';
      case 2:
        return 'سه شنبه';
      case 3:
        return 'چهارشنبه';
      case 4:
        return 'پنجشنبه';
      case 5:
        return 'جمعه';
      case 6:
        return 'شنبه';

      default:
    }
  }
}
