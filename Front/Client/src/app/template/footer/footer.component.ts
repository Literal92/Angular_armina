import { Component } from '@angular/core';
import { AuthService } from './../../core/services/auth.service';
import { Subscription } from "rxjs";

@Component({
  selector: 'client-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent {
  //isAuthUserLoggedIn: boolean=false;
  isLoggedIn = false;
  subscription: Subscription | null = null;
  constructor(
    private authService: AuthService
  ) {

  }

  //#region Methods
  ngOnInit() {

    this.subscription = this.authService.authStatus$.subscribe(status => {
      this.isLoggedIn = status;
      if (status) {


        // this.displayName = authUser ? authUser.displayName : "";
      }
    });
  }

  ngOnDestroy() {
    // prevent memory leak when component is destroyed
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

}
