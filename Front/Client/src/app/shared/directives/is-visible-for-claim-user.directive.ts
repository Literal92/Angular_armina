import { Directive, ElementRef, Input, OnDestroy, OnInit } from "@angular/core";
import { AuthService, ShemaUrl } from 'src/app/core';
import { Subscription } from "rxjs";
// more info >>> https://alligator.io/angular/building-custom-directives-angular/
@Directive({
  selector: "[isVisibleForClaim]"
})
export class IsVisibleForClaimUserDirective implements OnInit, OnDestroy {

  private subscription: Subscription | null = null;

  // @Input() isVisibleForClaim: ShemaUrl[] | null = null;
  @Input() isVisibleForClaim: string | null = null;
  @Input() isArea: string | null = null;
  @Input() isController: string | null = null;
  @Input() isAction: string | null = null;
  constructor(private elem: ElementRef,
    // private _viewContainer: ViewContainerRef,
    private authService: AuthService) { }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.subscription = this.authService.authStatus$.subscribe(status => this.changeVisibility(status));
    this.changeVisibility(this.authService.isAuthUserLoggedIn());
  }

  private changeVisibility(status: boolean) {
    let isVisibleForClaim = { area: this.isArea, controller: this.isController, action: this.isAction } as ShemaUrl;
    const isInClaim = !isVisibleForClaim ? true : this.authService.isAuthUserInRoleClaim/*isAuthUserInRoles*/(isVisibleForClaim);
    //this.elem.nativeElement.style.display = isInClaim && status ? "" : "none";
    if (isInClaim==false) {
      let el = this.elem.nativeElement;
      el.parentNode ? el.parentNode.removeChild(el):"";
    }
  }
}
