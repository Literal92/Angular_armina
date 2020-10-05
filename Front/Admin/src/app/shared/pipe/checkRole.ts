import { Pipe } from '@angular/core';

@Pipe({
  name: 'checkRole'
})
export class CheckRole {
  transform(roleId: any, userRole: any[]) {
    if (userRole.length == 0) {
      return false;
    }
    let index = userRole.findIndex(c => c.id == roleId);
    if (index > -1) {
      return true;
    }
    return false;
  }
}
