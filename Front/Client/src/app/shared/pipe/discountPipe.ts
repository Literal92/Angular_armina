import { Pipe } from '@angular/core';
import { DatePipe } from '@angular/common';
import * as moment from 'jalali-moment';

@Pipe({
  name: 'discountPipe'
})
export class DiscountPipe {
  transform(price: number, discount: number) {
    if (price > 0 && discount != undefined) {

      return  price - ((price * discount) / 100);
    }
    return price;
  }
}
