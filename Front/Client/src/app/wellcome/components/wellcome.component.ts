import { Component, OnInit, Inject} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { IAppConfig, APP_CONFIG } from 'src/app/core';
import { NotifierService } from 'angular-notifier';
import { SeoService } from 'src/app/core/services/seo.service';
import { MetaDefinition } from '@angular/platform-browser';


@Component({
  selector: "app-wellcome",
  templateUrl: "./wellcome.component.html",
  styleUrls: ["./wellcome.component.scss"]
})
export class WellcomeComponent implements OnInit {

  //#region Fields
  public searchForm: FormGroup | undefined;
  public populars: any[] = [];
  private readonly notifier: NotifierService;
  totalPage: number = 0; pages: number[] = []; row: number = 1;
  pageSize: number = 20; pageIndex: number = 0;// and set in paging config
  products: any[] = [];
  categories: any[] = [];
  id: number = 0;
  title: string = '';

  //#endregion

  //#region Ctor
  constructor(
    private seoService: SeoService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
  ) {
  }
  //#endregion

  //#region Methods
  ngOnInit() {
  }
  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('فروشگاه لباس کودک و نوجوان نی نی حراجی');
    // seo >> set meta for page
    let meta = {
      charset: '', content: ' ', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }

  //#endregion

  //#endregion
}
