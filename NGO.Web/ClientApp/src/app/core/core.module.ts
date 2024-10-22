import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Component, NgModule, OnInit } from '@angular/core';
import { ErrorInterceptor } from './interceptors/error.interceptor';

@NgModule({
  declarations:[],
  imports:[HttpClientModule],
  providers:[{provide:HTTP_INTERCEPTORS,useClass:ErrorInterceptor,multi:true}]
})
export class CoreModule  {

}
