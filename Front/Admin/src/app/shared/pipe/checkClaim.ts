import { Pipe } from '@angular/core';
import { debug } from 'util';

@Pipe({
  name: 'checkClaim'
})
export class CheckClaim {
  transform(claim: any, userId: number, claims: any[] = [], type: string) {
    if (claims.length == 0) {
      return false;
    }
    let index = claims.findIndex(c => c.userId == userId && c.claimType == type && c.claimValue == claim.id);
    if (index > -1) {
      return true;
    }
    return false;
  }
}
