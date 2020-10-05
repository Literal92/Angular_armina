export interface iorderSearch {
  id?: number,
  username?: string,
  from?: string,
  to?: string,


  reciverName: string,
  reciverMobile: string,
  senderName: string,
  senderMobile: string,
  address: string,
  productName:string,


  pageIndex?: number,
  pageSize?: number,
  orderType?: any,
  orderSendType?: any

}
