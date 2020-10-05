import { Pipe } from '@angular/core';
import { TimeViewPipe } from './time-view.pipe';

@Pipe({
  name: 'checkReport'
})
export class ReportPipe {
  transform(providerId: number, model: any[] = []) {
    if (model.length ==0) {
      return `<i class="fa fa-2x fa-remove"></i>`;
    }
    let index = model.findIndex(c => c.providerId == providerId);
    if (index > -1) {
      if (model[index].typeSchedule == 1) {
        if (model[index].providerCapacity ==0) {
          return `<i class="fa fa-2x fa-check"></i>`
        }
        return `${model[index].countReseve}`
      }
      return `${this.toTime(model[index].startTime)} - ${this.toTime(model[index].endTime)}
              <br/>
              ${model[index].countReseve} نفر`;
    }
    return `<i class="fa fa-2x fa-remove"></i>`;
  }
  toTime(time: string) {
    let h = Math.floor(+time);
    let m = (+time).toFixed(2).toString().split('.')[1];
    let H = h < 10 ? "0" + h : h.toString();
    return H + ":" + m;
  }
}
