import { Pipe } from '@angular/core';
import { debug } from 'util';

@Pipe({
  name: 'checkPaymentWay'
})
export class CheckPaymentWay {
  transform(id: any, list: number[] = []) {
    if (list.length == 0) {
      return false;
    }
    let index = list.findIndex(c => c == id);
    if (index > -1) {
      return true;
    }
    return false;
  }
}
