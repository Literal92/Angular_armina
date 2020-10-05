import { Pipe } from '@angular/core';

@Pipe({
  name: 'chkField'
})
export class CheckFieldPipe {
  transform(id: number, list: number[] = []) {
    if (list.length ==0) {
      return false;
    }
    let index= list.findIndex(c=> c== id);
    if(index >-1){
      return true;
    }
    return false;
  }
}
