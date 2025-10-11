import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { RegistrationURLConstants } from 'src/app/shared/constants/url-constants';
import { RegistrationModel } from '../models/UserRegistrationModel';

@Injectable({ providedIn: 'root' })
export class RegistrationService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}

  register(registrationModel: RegistrationModel, photo: File | null, orgId: string) {
    const formData = new FormData();

    // Append registration model data to the FormData object
    formData.append('registrationModel', JSON.stringify(registrationModel));

    // Append the photo file if it exists
    if (photo) {
      formData.append('photo', photo, photo.name);
    }

    // Make the HTTP POST request with the FormData object
    return this.http.post<any>(`${RegistrationURLConstants.REGISTER(orgId)}`, formData);
  }

  getMembershipPlans(orgId: string) {
    return this.http.get<any>(RegistrationURLConstants.GET_MEMBERSHIP_PLANS(orgId), { params: {} });
  }
}
