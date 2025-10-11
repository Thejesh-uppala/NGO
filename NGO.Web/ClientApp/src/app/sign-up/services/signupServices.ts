import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { RegistrationURLConstants } from 'src/app/shared/constants/url-constants';
@Injectable({ providedIn: 'root' })
export class SigUpService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}

  signup(
    password: string,
    phNumber: string,
    email: string,
    firstName: string,
    lastName: string,
    address: string,
    city: string,
    country: string,
    state: string,
    orgId: string,
    chapterId: number
  ) {
    const mapParams = new Map();

    mapParams.set('password', password);
    mapParams.set('phNumber', phNumber);
    mapParams.set('email', email);
    mapParams.set('firstName', firstName);
    mapParams.set('lastName', lastName);
    mapParams.set('address', address);
    mapParams.set('city', city);
    mapParams.set('country', country);
    mapParams.set('state', state);
    mapParams.set('orgId', orgId.toString()); // Ensure orgId is passed as a string if required by the API
    mapParams.set('chapterId', chapterId.toString()); // Ensure chapterId is passed as a string if required by the API

    const params = this.commonService.getHttpParams(mapParams);

    return this.http.post<any>(RegistrationURLConstants.SIGNUP(orgId), {}, { params });
  }

  getChapters(orgId: string): Observable<any[]> {
    return this.http.get<any[]>(RegistrationURLConstants.GET_CHAPTERS(orgId));
  }
}
