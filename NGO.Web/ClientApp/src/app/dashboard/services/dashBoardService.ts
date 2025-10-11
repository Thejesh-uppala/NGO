import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { DashBoardURLConstants, EventURLConstants } from 'src/app/shared/constants/url-constants';

@Injectable({ providedIn: 'root' })
export class DashBoardService {
  getUpcomingEvent(orgId: number): Observable<any> {
    return this.http.get<any>(`${EventURLConstants.GET_ALL_UPCOMING_EVENTS(orgId)}`);
  }

  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}
  getUserDetails(userId: number, orgId: number) {
    return this.http.get<any>(DashBoardURLConstants.GET_CURRENT_USER_PROFILE(orgId), { params: { userId: userId } });
  }

  getAllEvent(orgId: number): Observable<any> {
    return this.http.get<any>(`${EventURLConstants.GET_ALL_EVENTS(orgId)}`);
  }

  getAllUserRegisteredEvents(orgId: number, userId: number): Observable<any> {
    return this.http.get<any>(EventURLConstants.GET_ALL_USER_REGISTERED_EVENTS(orgId, userId));
  }

  getOrganizations(): Observable<any> {
    return this.http.get<any>(DashBoardURLConstants.GET_ALL_ORGANIZATIONS);
  }
}
