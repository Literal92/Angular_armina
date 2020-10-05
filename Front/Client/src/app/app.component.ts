import { Component, HostListener} from "@angular/core";
import { RefreshTokenService } from './core';
import { EnableLoading, DisableLoading } from './core/loading/loading';
import * as SVGInject  from './core/theme-js/themejs.js';

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"]
})
export class AppComponent {
  constructor() {}
  ngOnInit() {
  }

}
