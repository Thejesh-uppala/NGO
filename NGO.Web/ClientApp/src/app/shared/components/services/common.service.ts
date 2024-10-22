import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Title } from '@angular/platform-browser';
@Injectable()
export class CommonService {
  
  constructor(
    private http: HttpClient,
    private titleService: Title
  ) {}
  getHttpParams(mapParams: Map<string, string>): HttpParams {
    let params = new HttpParams();
    for (let param of mapParams.entries()) {
      if (param[1] && param[1] !== null) {
        params = params.append(param[0], param[1]);
      }
    }
    return params;
  }

}
