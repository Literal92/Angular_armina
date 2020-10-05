export interface iCategory {
  id?:number,
  title: string,
  path?:string,
  icon: string,
  parentId?: string,
  order?:number
}

export interface iCSV {
  
  sendDate: string,
  file: string,
  description: string
}


