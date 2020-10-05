import { Location } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { AuthService } from 'src/app/core';

@Component({
  selector: "app-access-denied",
  templateUrl: "./access-denied.component.html",
  styleUrls: ["./access-denied.component.scss"]
})
export class AccessDeniedComponent implements OnInit {

  isAuthenticated = false;

  constructor(
    private location: Location,
    private authService: AuthService
  ) {
  }

  ngOnInit() {
    this.isAuthenticated = this.authService.isAuthUserLoggedIn();
  }

  goBack() {
    this.location.back(); // <-- go back to previous location on cancel
  }
}
