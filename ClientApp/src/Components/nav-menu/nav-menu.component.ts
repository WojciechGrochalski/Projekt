import { Component } from "@angular/core";
import { Router, Event, NavigationStart } from "@angular/router";

import { AuthService } from "../../Services/auth.service";

@Component({
  selector: "app-nav-menu",
  templateUrl: "./nav-menu.component.html",
  styleUrls: ["./nav-menu.component.css"],
})
export class NavMenuComponent {
  isExpanded = false;
  isLogged = false;

  constructor(private _authService: AuthService, private router: Router) {
    this.router.events.subscribe((event: Event) => {
      if (event instanceof NavigationStart) {
        if (this._authService.currentUserValue) {
          this.isLogged = true;
        } else {
          this.isLogged = false;
        }
      }
    });
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  Login() {
    this.isLogged = true;

    this.router.navigate(["/login"]);
  }

  Logout() {
    this._authService.logout();
    this.isLogged = false;

    this.router.navigate(["/"]);
  }
}
