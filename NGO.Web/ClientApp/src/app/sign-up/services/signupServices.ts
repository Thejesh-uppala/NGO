import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegistrationURLConstants } from 'src/app/shared/constants/url-constants';
import { CommonService } from 'src/app/shared/components/services/common.service';;
import { CountryInterface } from 'src/app/userprofile/models/UpdateUserModel';


@Injectable({ providedIn: 'root' })
export class SigUpService {
    constructor(private http: HttpClient,
        private commonService: CommonService) {
    }
    register(userName:string,password:string,phNumber:string,email:string) {
        const mapParams = new Map();
        mapParams.set('userName', userName);
        mapParams.set('password', password);
        mapParams.set('phNumber', phNumber);
        mapParams.set('email', email);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.post<any>(`${RegistrationURLConstants.SIGNUP}`,{},{params});
    }
}
