import { Component, OnInit, AfterViewInit, ViewChild, Inject, TemplateRef } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NotifierService } from 'angular-notifier';
import { FieldService } from '../../services/field.service';
import { iField } from '../../interfaces/iField';
import { Router, ActivatedRoute } from '@angular/router';
import { DomSanitizer, MetaDefinition } from '@angular/platform-browser';
import { SeoService } from 'src/app/core/services/seo.service';
import { SharedService } from 'src/app/shared/services/shared.service';
import { IAppConfig, APP_CONFIG } from 'src/app/core';
import { DisableLoading, EnableLoading } from 'src/app/core/loading/loading';

@Component({
  selector: 'field-add',
  templateUrl: './add.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class AddComponent implements OnInit{
  //#region Fields
  public addForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  public types: any[] = [
    { id: 0, display: 'متن' },
    { id: 1, display: 'یک انتخابی' },
    { id: 2, display: 'چند انتخابی' }
  ];
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
    private service: FieldService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {
    // Seo
    this.SeoConfig();
    this.addForm = this.formBuilder.group({
      title: ['', Validators.required],
      displayTitle: ['', Validators.required],
      description: [''],
      order: [''],
      fieldType: [this.types[0].id, Validators.required]

    });
  }
  ngAfterViewInit() {
   

  }
  onSubmit(items: iField) {
    EnableLoading();
    console.log(items);
    this.service.post(items).subscribe(result => {
      DisableLoading();
      this.router.navigate(['/field']);

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
    this.seoService.SetTitle('افزودن فیلد');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'افزودن فیلد', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

   //#endregion
}
