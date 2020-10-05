import { Pipe } from '@angular/core';

@Pipe({
  name: 'checkRoleClaim'
})
export class CheckRoleClaim {
  transform(claim: any, roleId:number, claims: any[] = []) {
    if (claims.length == 0) {
      return false;
    }
    let index = claims.findIndex(c => c.roleId == roleId && c.claimType == claim.groupName && c.claimValue == claim.shortName);
    if (index > -1) {
      return true;
    }
    return false;
  }
}
