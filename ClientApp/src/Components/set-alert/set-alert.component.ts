import { Component, OnInit } from '@angular/core';
import {UserService} from "../../Services/User.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Remainder} from "../../Models/Remainder";
import {FlashMessagesService} from "angular2-flash-messages";

@Component({
  selector: 'app-set-alert',
  templateUrl: './set-alert.component.html',
  styleUrls: ['./set-alert.component.css']
})
export class SetAlertComponent implements OnInit {

  priceLessThan:boolean;
  iso:string;
  userID:number;
  isAdding:boolean;

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    private flashMessagesService: FlashMessagesService) {
    this.isAdding = false;
}

  ngOnInit() {
    this.userID= +JSON.parse(localStorage.getItem('currentUser')).id;
    console.log(this.userID)

      this.iso = this.route.snapshot.paramMap.get('iso');
       let price= this.route.snapshot.paramMap.get('price');
       if(price=='More'){
         this.priceLessThan=false;
       }
       else{
         this.priceLessThan=true;
       }
  }

  AddAlert(value: string, date: string){
    let alert = new Remainder();
    this.isAdding=true;
    if(this.priceLessThan){
      alert.price='More';
      alert.askPrice= +value;
    }
    else{
      alert.price='Less';
      alert.bidPrice= +value;
    }
    alert.endDateOfAlert=new Date(date);
    alert.code=this.iso;
    alert.userID=  this.userID;

    this.userService.SetAlert(alert).subscribe(res =>{
        this.flashMessagesService.show(res.message, {cssClass: 'alert-success', timeout: 5000})
      setTimeout(() => {
        this.isAdding = false;
          this.router.navigate(['/user-profile']);
        }, 3000)

    },
      error => {
        this.flashMessagesService.show(error.error.message, {cssClass: 'alert-danger', timeout: 3000})
      });
  }
}
