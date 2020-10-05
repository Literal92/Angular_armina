import { Injectable, ElementRef } from "@angular/core";
import { Title, Meta, MetaDefinition } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root'
})
export class SeoService {
  constructor(private titleService: Title, private metaTag: Meta) { }

  SetTitle(title: any) {
    this.titleService.setTitle(title);
  }
  SetMeta(meta: MetaDefinition[]) {
    this.metaTag.removeTag("description");
    this.metaTag.removeTag("keywords");
    this.metaTag.addTags(meta);
  }
}
