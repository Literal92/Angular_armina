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
import { iFieldSearch } from '../../interfaces/iFieldSearch';
import { DisableLoading, EnableLoading } from 'src/app/core/loading/loading';

@Component({
  selector: 'field-edit',
  templateUrl: './edit.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class EditComponent implements OnInit{
  //#region Fields
  iconFile: any; 
  public editForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  serviceId: number = 0;
  providerItem: any = null;
  errorFile: any = null;
  Id: number = 0;
  field: any = null;
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

    this.route.params.subscribe(params => {
      if (params['id'] != undefined) {
        this.Id = params['id'];
        let model: iFieldSearch = { id: this.Id};
          this.service.get(model).subscribe(result => {
            let output = result as any;
            if (output != undefined) {
              if (output.pages != undefined) {
                this.field = output.pages[0];
                this.providerItem = output.provider;
                if (this.field != undefined) {
                  this.editForm = this.formBuilder.group({
                    title: [this.field.title != undefined ? this.field.title : '', Validators.required],
                    displayTitle: [this.field.displayTitle != undefined ? this.field.displayTitle : '', Validators.required],
                    description: [this.field.description != undefined ? this.field.description: ''],
                    order: [this.field.order != undefined ? this.field.order : 0],
                    fieldType: [this.field.fieldType != undefined ? this.field.fieldType : this.types[0].id, Validators.required]

                });
                }
              }
            }
          }, err => {
            throw err;
          });
        }
    });
  }
  ngAfterViewInit() {
   

  }
  onSubmit(items: iField) {
    EnableLoading();
    console.log(items);
    items.id = this.Id;
    this.service.put(items).subscribe(result => {
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
    this.seoService.SetTitle('ویرایش فیلد');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'ویرایش فیلد', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

   //#endregion
}
