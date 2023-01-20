import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { UserService } from "../../Services/User.service";
import { FlashMessagesService } from "angular2-flash-messages";
import { Cash } from "../../Models/Cash";
import { CashService } from "../../Services/cash.service";
import { AuthService } from "../../Services/auth.service";
import { Remainder } from "../../Models/Remainder";

@Component({
  selector: "app-user-profile",
  templateUrl: "./user-profile.component.html",
  styleUrls: ["./user-profile.component.css"],
})
export class UserProfileComponent implements OnInit {
  cash_list: Cash[];
  remainders: Remainder[];
  message: string;
  subscriptions: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router,
    private _authService: AuthService,
    private flashMessagesService: FlashMessagesService,
    private cashService: CashService
  ) {}

  async ngOnInit() {
    let user = JSON.parse(localStorage.getItem("currentUser"));
    this.subscriptions = user.sub;
    try {
      this.cash_list = await this.cashService.GetDataOnInit().toPromise();
      this.remainders = await this.userService.GetAlerts(user.id).toPromise();
      let message = this.route.snapshot.paramMap.get("message");
      if (message) {
        this.flashMessagesService.show(message, {
          cssClass: "alert-success",
          timeout: 5000,
        });
      }
    } catch (e) {
      console.error(e);
    }
    console.log(this.remainders);
  }

  AddSubscription() {
    let user = JSON.parse(localStorage.getItem("currentUser"));
    this.userService.AddSubscription(user.id).subscribe((res) => {
      this.flashMessagesService.show(res.message, {
        cssClass: "alert-success",
        timeout: 3000,
      });
    });
    this.subscriptions = true;
  }
  RemoveSubscription() {
    let user = JSON.parse(localStorage.getItem("currentUser"));
    this.userService.RemoveSubscription(user.id).subscribe((res) => {
      this.flashMessagesService.show(res.message, {
        cssClass: "alert-success",
        timeout: 3000,
      });
    });
    this.subscriptions = false;
  }
  ChangeSubscription() {
    if (this.subscriptions == false) {
      this.AddSubscription();
    } else {
      this.RemoveSubscription();
    }
  }

  SetPriceAlert(iso: string, price: string) {
    this.router.navigate(["/set-alert/" + iso + "/" + price]);
  }

  Logout() {
    this._authService.logout();
    this.router.navigate(["/"]);
  }
}
