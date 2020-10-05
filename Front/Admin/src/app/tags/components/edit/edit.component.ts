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
import { iTagSearch } from '../../interfaces/iTagSearch';
import { DisableLoading, EnableLoading } from 'src/app/core/loading/loading';

@Component({
  selector: 'tag-edit',
  templateUrl: './edit.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class EditComponent implements OnInit{
  //#region Fields
  iconFile: any; 
  public editForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  Id: number = 0;
  tag: any = null;
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

    this.route.params.subscribe(params => {
      if (params['id'] != undefined) {
        this.Id = params['id'];
        let model: iTagSearch = { id: this.Id };
          this.service.get(model).subscribe(result => {
            let output = result as any;
            if (output != undefined) {
              if (output.pages != undefined) {
                this.tag = output.pages[0];
                if (this.tag != undefined) {
                  this.editForm = this.formBuilder.group({
                    title: [this.tag.title != undefined ? this.tag.title : '', Validators.required],
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
  onSubmit(items: iTag) {
    EnableLoading();
    console.log(items);
    items.id = this.Id;
    this.service.put(items).subscribe(result => {
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
    this.seoService.SetTitle('ویرایش تگ');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'ویرایش تگ', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

   //#endregion
}
