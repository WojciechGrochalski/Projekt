import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { FlashMessagesModule } from 'angular2-flash-messages';

import { NgApexchartsModule } from 'ng-apexcharts';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavMenuComponent } from '../Components/nav-menu/nav-menu.component';
import { CashDataComponent } from '../Components/cash-data/cash-data.component';
import {UserProfileComponent} from "../Components/user-profile/user-profile.component";
import {ForgetPasswordComponent} from "../Components/forget-password/forget-password.component";
import {LogInComponent} from "../Components/Login/log-in.component";
import {RegisterComponent} from "../Components/register/register.component";
import {SetAlertComponent} from "../Components/set-alert/set-alert.component";
import {HttpInterceptorService} from "../Services/http-interceptor.service";
import {ErrorInterceptorService} from "../Services/error-interceptor.service";
import {VerifyUserComponent} from "../Components/verify-user/verify-user.component";
import {NewPasswordComponent} from "../Components/new-password/new-password.component";
import {AuthGuard} from "../Services/auth-guard.service";


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    CashDataComponent,
    UserProfileComponent,
    ForgetPasswordComponent,
    LogInComponent,
    RegisterComponent,
    SetAlertComponent,
    VerifyUserComponent,
    NewPasswordComponent

  ],
  imports: [
    ReactiveFormsModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NgApexchartsModule,
    FlashMessagesModule.forRoot(),
    RouterModule.forRoot([
      { path: '', component: CashDataComponent, pathMatch: 'full' },
      { path: 'user-profile', component: UserProfileComponent , canActivate: [AuthGuard] },
      { path: 'user-profile/:message', component: UserProfileComponent, canActivate: [AuthGuard] },
      { path: 'login', component: LogInComponent},
      { path: 'login/:confirm', component: LogInComponent},
      { path: 'register', component: RegisterComponent},
      { path: 'new-password', component: NewPasswordComponent},
      { path: 'verify-user', component: VerifyUserComponent},
      { path: 'forgot-password', component: ForgetPasswordComponent},
      {path: 'set-alert/:iso/:price', component: SetAlertComponent, canActivate: [AuthGuard]}
    ]),
    BrowserAnimationsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: HttpInterceptorService, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptorService, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
