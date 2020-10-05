import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'timeView'
})
export class TimeViewPipe implements PipeTransform {
    constructor() { }
    transform(time: string) {
        let h = Math.round(+time);
        let m = (+time).toFixed(2).toString().split('.')[1];
        let H = h < 10 ? "0" + h : h.toString();
        return H + ":" + m;
    }
}


@Pipe({
    name: 'timeFindView'
})
export class TimeFindViewPipe implements PipeTransform {
    constructor() { }
    transform(time: any[], providerId: number, first: boolean) {
        debugger;
        let result = time.filter(p => p.providerId == providerId);
        if (first == true) {
            return result[0].hour;
        }
        else {
            return result[result.length - 1].hour;
        }

        return;
    }
}
