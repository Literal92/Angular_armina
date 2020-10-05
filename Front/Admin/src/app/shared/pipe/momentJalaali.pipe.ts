import { Pipe, PipeTransform } from "@angular/core";

import * as momentJalaali from "moment-jalaali";
// https://www.dotnettips.info/post/2711/%D8%A7%D8%B3%D8%AA%D9%81%D8%A7%D8%AF%D9%87-%D8%A7%D8%B2-%DA%A9%D8%AA%D8%A7%D8%A8%D8%AE%D8%A7%D9%86%D9%87%E2%80%8C%DB%8C-moment-jalaali-%D8%AF%D8%B1-%D8%A8%D8%B1%D9%86%D8%A7%D9%85%D9%87%E2%80%8C%D9%87%D8%A7%DB%8C-angular
@Pipe({
  name: "momentJalaali"
})
export class MomentJalaaliPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return momentJalaali(value).format(args);
  }
}
