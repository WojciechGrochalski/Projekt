import {Inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BaseUrlModel} from "../Models/BaseUrlModel";
import {Remainder} from "../Models/Remainder";
import {Observable} from "rxjs";
import {Token} from "../Models/Token";
import {NewTokens} from "../Models/NewTokens";
import {JwtHelperService} from "@auth0/angular-jwt";
import {tap} from "rxjs/operators";

const jwtHelper= new JwtHelperService();

@Injectable({
  providedIn: 'root'
})
export class UserService {


  BaseUrl: string = '';

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string ) {
    this.BaseUrl = baseUrl;
  }

  SendBaseUrl(): Observable<any>{
    let url=this.BaseUrl;
    return this.http.post(this.BaseUrl + `User/baseUrl?url=${url}`, url);
  }

  AddSubscription(userID: number){
    return this.http.get<any>(this.BaseUrl + 'User/sub/'+userID);
  }
  RemoveSubscription(userID: number){
    return this.http.get<any>(this.BaseUrl + 'User/remove/sub/'+userID);
  }
  SetAlert(alert :Remainder){
    return this.http.post<any>(this.BaseUrl + 'Alert/addAlert', alert);
  }
  GetAlerts(userID: number){
    return this.http.get<any>(this.BaseUrl + 'Alert/alerts/'+userID);
  }
  VerifyUser(token: string): Observable<any> {
    let jwtToken = new Token(token);
    return this.http.post(this.BaseUrl + 'User/verify-email', jwtToken);
  }
  VerifyPasswordToken(token: string): Observable<any> {
   let jwtToken = new Token(token);
    return this.http.post(this.BaseUrl + 'User/verify-resetpassword', jwtToken);
  }
 RefreshToken() {
    return this.http.get<NewTokens>(this.BaseUrl + 'Token/refreshToken').pipe(tap
    ((tokens:any)=>{
      this.storeJwtToken(tokens);
    }));
  }
  private storeJwtToken(tokens: any) {
    localStorage.setItem("accessToken", tokens.accessToken);
    localStorage.setItem("refreshToken", tokens.refreshToken);
  }

  CheckThatTokenNotExpired(token: string):boolean{
    const isExpired =jwtHelper.isTokenExpired(token);
    if (isExpired) {
      return true;
    }
    return false;
  }
}
