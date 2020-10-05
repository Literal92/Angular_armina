import { SpecialState } from 'src/app/shared/enum/specialState';

export interface iSpecial {
  id?:number,
  state?: SpecialState,
  providerId?: number
}
