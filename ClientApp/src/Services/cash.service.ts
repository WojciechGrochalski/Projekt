import { Component, Inject, Input, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class CashService {

  baseUrl: string = '';

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }



  GetLastCurrency(iso: string, count: number): Observable<any>{

    return this.http.get(this.baseUrl + 'cash/' + iso + '/' + count);
  }

  GetDataOnInit(): Observable<any> {

    return this.http.get(this.baseUrl+ 'cash' );

  }
  GetChartData(iso: string, count: number): Observable<any> {
    return this.http.get(this.baseUrl + 'cash/' + iso + '/' + count + '/DataChart');

  }

  GetChartAskPrice(iso: string, count: number): Observable<any>{
    return this.http.get(this.baseUrl + 'cash/' + iso + '/' + count + '/AskPrice');


  }
  GetChartBidPrice(iso: string, count: number): Observable<any> {
    return this.http.get(this.baseUrl + 'cash/' + iso + '/' + count + '/BidPrice');
  }

}




