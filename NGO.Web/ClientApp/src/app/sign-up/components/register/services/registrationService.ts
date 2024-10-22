import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegistrationURLConstants, UserProfileURLConstants } from 'src/app/shared/constants/url-constants';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { RegistrationModel } from '../models/UserRegistrationModel';
import { CountryInterface } from 'src/app/userprofile/models/UpdateUserModel';
import { Observable } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class RegistrationService {
    constructor(private http: HttpClient,
        private commonService: CommonService) {
    }
    register(registrationModel: RegistrationModel) {
        debugger
        return this.http.post<any>(`${RegistrationURLConstants.REGISTER}`,registrationModel);
    }
    // getAllCountries() {
    //     return this.http.get<any>('assets/country.json')
    //         .toPromise()
    //         .then(res => <CountryInterface[]>res)
    //         .then(data => { return data; });
    // }
    uploadPhoto(formData:any,id:any):Observable<any> {
        const mapParams = new Map();
        mapParams.set('userId', id);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.post<any>(`${RegistrationURLConstants.UPLOAD_PHOTO}`,formData,{params});
    }
}
