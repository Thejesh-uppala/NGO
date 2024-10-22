import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegistrationURLConstants, ResetPasswordConstants } from 'src/app/shared/constants/url-constants';
import { CommonService } from 'src/app/shared/components/services/common.service';


@Injectable({ providedIn: 'root' })
export class ForgotPasswordPasswordService {
    constructor(private http: HttpClient,
        private commonService: CommonService) {
    }
    sendOTP(emailId: string) {
        const mapParams = new Map();
        mapParams.set('emailId', emailId);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.post<any>(`${ResetPasswordConstants.SEND_OTP}`, {}, { params });
    }
    changePassWord(emailId: string, newPassword: string) {
        const mapParams = new Map();
        mapParams.set('emailId', emailId);
        mapParams.set('newPassword', newPassword);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.post<any>(ResetPasswordConstants.FORGOT_PASSWORD, {},{ params });
    }
}
