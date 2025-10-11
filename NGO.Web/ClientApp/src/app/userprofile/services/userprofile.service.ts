import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { DashBoardURLConstants, UserProfileURLConstants } from 'src/app/shared/constants/url-constants';
import { UserDetailModel } from '../models/UpdateUserModel';

@Injectable({ providedIn: 'root' })
export class UserProfileService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}
  updateUser(userDetailModel: UserDetailModel, photo: File | null, orgId: string): Observable<any> {
    const formData = new FormData();
    formData.append('userUpdateModel', JSON.stringify(userDetailModel));
    if (photo) {
      formData.append('photo', photo, photo.name);
    }
    return this.http.put<any>(`${UserProfileURLConstants.USER_PROFILE(orgId)}`, formData);
  }

  uploadPhoto(formData: any, id: any): Observable<any> {
    const mapParams = new Map();
    mapParams.set('userId', id);
    const params = this.commonService.getHttpParams(mapParams);
    return this.http.post<any>(`${UserProfileURLConstants.UPLOAD_PHOTO}`, formData, { params });
  }

  getCurrentUserProfile(storedOrgId: number, userId: string) {
    return this.http.get<any>(DashBoardURLConstants.GET_CURRENT_USER_PROFILE(storedOrgId), {
      params: { userId: userId },
    });
  }
}
