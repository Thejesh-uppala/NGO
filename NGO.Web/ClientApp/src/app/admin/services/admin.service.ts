import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { DashBoardURLConstants, EventURLConstants } from 'src/app/shared/constants/url-constants';

@Injectable({ providedIn: 'root' })
export class AdminService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}
  getAllUsers(orgId: number): Observable<any> {
    return this.http.get<any>(`${DashBoardURLConstants.GET_ALL_USERS(orgId)}`);
  }
  getAllMembers(orgId: number): Observable<any> {
    return this.http.get<any>(`${DashBoardURLConstants.GET_ALL_MEMBERS(orgId)}`);
  }

  deleteEvent(eventId: number): Observable<any> {
    return this.http.delete<any>(`${EventURLConstants.DELETE_EVENT(eventId)}`);
  }

  getSummaryCounts(orgId: number): Observable<any> {
    return this.http.get<any>(`${DashBoardURLConstants.GET_SUMMARY_COUNTS(orgId)}`);
  }

  rejectMembership(orgId: number, userIds: number[], userId: number): Observable<any> {
    return this.http.post<any>(DashBoardURLConstants.REJECT_MEMBERSHIPS(orgId), { userIds, userId });
  }

  acceptMembership(orgId: number, userIds: number[], userId: number): Observable<any> {
    return this.http.post<any>(DashBoardURLConstants.ACCEPT_MEMBERSHIPS(orgId), { userIds, userId });
  }

  paymentReminder(userId: any): Observable<any> {
    const mapParams = new Map();
    mapParams.set('userId', userId);
    const params = this.commonService.getHttpParams(mapParams);
    return this.http.post<any>(`${DashBoardURLConstants.PAYMENT_REMINDER}`, {}, { params });
  }

  getOrganizations(): Observable<any> {
    return this.http.get<any>(DashBoardURLConstants.GET_ALL_ORGANIZATIONS);
  }
  getChapters(): Observable<any> {
    return this.http.get<any>(DashBoardURLConstants.GET_ALL_CHAPTERS);
  }

  deleteUsers(userIds: number[], updatedBy: number, orgId: number): Observable<any> {
    return this.http.delete(DashBoardURLConstants.DELETE_USER, {
      body: userIds,
      params: { updatedBy: updatedBy.toString(), orgId: orgId.toString() },
    });
  }
  remindUsers(userEmails: string[], updatedBy: number, orgId: number): Observable<any> {
    return this.http.post(`${DashBoardURLConstants.REMIND_USER(updatedBy, orgId)}`, userEmails);
  }
}
