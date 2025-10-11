import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { ResetPasswordConstants } from 'src/app/shared/constants/url-constants';

@Injectable({ providedIn: 'root' })
export class ForgotPasswordPasswordService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}

  sendOTP(emailId: string, orgId: string) {
    const mapParams = new Map();
    mapParams.set('emailId', emailId);
    const params = this.commonService.getHttpParams(mapParams);
    const url = ResetPasswordConstants.SEND_OTP(orgId);

    return this.http.post<any>(url, {}, { params });
  }

  validateOTP(emailId: string, orgId: string, otp: string) {
    const mapParams = new Map();
    mapParams.set('emailId', emailId);
    mapParams.set('otp', otp); // Sent as string, backend converts to int
    const params = this.commonService.getHttpParams(mapParams);
    const url = ResetPasswordConstants.VALIDATE_OTP(orgId);
    return this.http.post<any>(url, {}, { params });
  }

  changePassWord(emailId: string, orgId: string, newPassword: string) {
    const mapParams = new Map();
    mapParams.set('emailId', emailId);
    mapParams.set('newPassword', newPassword);
    const params = this.commonService.getHttpParams(mapParams);
    return this.http.post<any>(ResetPasswordConstants.FORGOT_PASSWORD(orgId), {}, { params });
  }
}
