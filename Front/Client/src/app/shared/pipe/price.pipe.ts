import { Pipe } from '@angular/core';

@Pipe({
  name: 'toPrice'
})
export class FormatPricePipe {
  transform(input: number, isUnit: boolean = false) {
    if (input != 0 && input != undefined) {
      if (isUnit == false) {
        return input.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + " تومان";
      }
      else {
        return input.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
      }

    }
    else {
      if (isUnit == false) {
        return "0 تومان";
      }
      else {
        return "0";
      }
    }
  }
}
