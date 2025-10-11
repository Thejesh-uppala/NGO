import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { ContactUSURLConstants } from 'src/app/shared/constants/url-constants';

export interface ContactFormData {
  contactFormName: string;
  contactFormEmail: string;
  contactFormSubjects: string;
  contactFormMessage: string;
  orgId: number;
}

@Injectable({ providedIn: 'root' })
export class ContactUsService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}

  sendMessage(formData: ContactFormData) {
    const mapParams = new Map<string, string>();
    Object.entries(formData).forEach(([key, value]) => {
      mapParams.set(key, value.toString());
    });

    const params = this.commonService.getHttpParams(mapParams);
    return this.http.post<any>(ContactUSURLConstants.SEND_MAIL, {}, { params });
  }
}
