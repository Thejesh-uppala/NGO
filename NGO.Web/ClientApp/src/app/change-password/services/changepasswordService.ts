import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegistrationURLConstants, ResetPasswordConstants } from 'src/app/shared/constants/url-constants';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { ChangePasswordModel } from '../models/changePassword';


@Injectable({ providedIn: 'root' })
export class ChangePasswordService {
    constructor(private http: HttpClient,
        private commonService: CommonService) {
    }
    resetPassword(resetPasswordModel:ChangePasswordModel) {
        debugger
        return this.http.post<any>(`${ResetPasswordConstants.RESET_PASSWORD}`,resetPasswordModel);
    }
}
