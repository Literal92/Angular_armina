import { Pipe } from '@angular/core';
import { debug } from 'util';

@Pipe({
  name: 'showProviderPipe'
})
export class ShowProviderPipe {
  transform(id: any, list: number[] = []) {
    if (list.length == 0) {
      return 'hidden';
    }
    let index = list.findIndex(c => c == id);
    if (index > -1) {
      $(`#provider${id}`).removeClass('hidden');
      $(`#provider${id}`).addClass('show');
      return 'show';
    }
    return 'hidden';
  }
}
