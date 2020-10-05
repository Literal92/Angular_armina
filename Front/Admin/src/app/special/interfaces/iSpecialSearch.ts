import { SpecialState } from 'src/app/shared/enum/specialState';

export interface iSpecialSearch {
  id?:number,
  state?: SpecialState,
  pageIndex?: number,
  pageSize?: number,
  mobile?:string
}
