import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { ResetPasswordConstants } from 'src/app/shared/constants/url-constants';
import { ChangePasswordModel } from '../models/changePassword';

@Injectable({ providedIn: 'root' })
export class ChangePasswordService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}
  resetPassword(orgId: string, resetPasswordModel: ChangePasswordModel) {
    return this.http.post<any>(`${ResetPasswordConstants.RESET_PASSWORD(orgId)}`, resetPasswordModel);
  }
}
