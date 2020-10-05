import { Component, OnInit, AfterViewInit, ViewChild, Inject, TemplateRef } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NotifierService } from 'angular-notifier';
import { TagService } from '../../services/tag.service';
import { iTag } from '../../interfaces/iTag';
import { Router, ActivatedRoute } from '@angular/router';
import { DomSanitizer, MetaDefinition } from '@angular/platform-browser';
import { SeoService } from 'src/app/core/services/seo.service';
import { SharedService } from 'src/app/shared/services/shared.service';
import { IAppConfig, APP_CONFIG } from 'src/app/core';
import { DisableLoading, EnableLoading } from 'src/app/core/loading/loading';

@Component({
  selector: 'tag-add',
  templateUrl: './add.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class AddComponent implements OnInit{
  //#region Fields
  public addForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  
  ////#endregion

  //#region Ctor
  constructor( private router: Router,
    protected route: ActivatedRoute,
    private formBuilder: FormBuilder,
    // loading
    private sanitizer: DomSanitizer,
    private notifierService: NotifierService,
    private seoService: SeoService,
    private shareService: SharedService,
    private service: TagService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {
    // Seo
    this.SeoConfig();
    this.addForm = this.formBuilder.group({
      title: ['', Validators.required]
    });
  }
  ngAfterViewInit() {
   

  }
  onSubmit(items: iTag) {
    EnableLoading();
    console.log(items);
    this.service.post(items).subscribe(result => {
      DisableLoading();
      this.router.navigate(['/tag']);

    }, err => {
      DisableLoading();

      this.notifier.show({
        type: 'error',
        message: err.error.text
      });
      throw err;
    });

  }
  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('افزودن تگ');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'افزودن تگ', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

   //#endregion
}
