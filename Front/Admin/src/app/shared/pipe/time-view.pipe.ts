import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeView'
})
export class TimeViewPipe implements PipeTransform {
  constructor() { }
  transform(time: string) {
    let h = Math.floor(+time);
    let m = (+time).toFixed(2).toString().split('.')[1];
    let H = h < 10 ? "0" + h : h.toString();
    return H + ":" + m;
  }
}
