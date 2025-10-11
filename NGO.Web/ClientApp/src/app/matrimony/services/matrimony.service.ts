import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { MatrimonialURLConstants } from 'src/app/shared/constants/url-constants';

@Injectable({ providedIn: 'root' })
export class MatrimonyService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}

  getAllMatrimonialUsers(orgId: number): Observable<any> {
    return this.http.get<any>(`${MatrimonialURLConstants.GET_ALL_MATRIMONIAL_USERS(orgId)}`);
  }

  submitMatrimonyProfile(data: FormData): Observable<any> {
    return this.http.post(MatrimonialURLConstants.CREATE_MATRIMONIAL_PROFILE, data);
  }

  approveProfiles(orgId: number, userIds: number[], userId: number): Observable<any> {
    return this.http.post<any>(MatrimonialURLConstants.APPROVE_PROFILES(orgId), { userIds, userId });
  }

  rejectProfiles(orgId: number, userIds: number[], userId: number): Observable<any> {
    return this.http.post<any>(MatrimonialURLConstants.REJECT_PROFILES(orgId), { userIds, userId });
  }

  getMatrimonyProfileDetails(id: number, orgId: number): Observable<any> {
    return this.http.get<any>(`${MatrimonialURLConstants.GET_MATRIMONY_PROFILE_DETAILS(id, orgId)}`);
  }

  getAllApprovedMatrimonialUsers(orgId: number, page: number, pageSize: number): Observable<any> {
    return this.http.get<any>(`${MatrimonialURLConstants.GET_ALL_APPROVED_MATRIMONIAL_USERS(orgId, page, pageSize)}`);
  }

  getFilteredMatrimonialUsers(filters: any, orgId: number, page: number, pageSize: number): Observable<any> {
    // Construct the URL dynamically by using the orgId

    const url = `${MatrimonialURLConstants.GET_FILTERED_MATRIMONIAL_USERS(orgId, page, pageSize)}`;

    // Construct query parameters with the filters
    const params = {
      gender: filters.gender || '',
      minAge: filters.minAge ? filters.minAge.toString() : '',
      maxAge: filters.maxAge ? filters.maxAge.toString() : '',
      minHeight: filters.minHeight ? filters.minHeight.toString() : '',
      maxHeight: filters.maxHeight ? filters.maxHeight.toString() : '',
      state: filters.state || '',
      country: filters.country || '',
      education: filters.education || '',
      profession: filters.profession || '',
      prevMarried: filters.prevMarried || '',
      searchValue: filters.searchValue || '', // Add searchValue parameter
    };

    // Return the HTTP GET request with the URL and query parameters
    return this.http.get<any>(url, { params });
  }
}
