import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashBoardURLConatants, UserProfileURLConstants } from 'src/app/shared/constants/url-constants';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { CountryInterface } from '../models/UpdateUserModel';


@Injectable({ providedIn: 'root' })
export class UserProfileService {
    constructor(private http: HttpClient,
        private commonService: CommonService) {
    }
    updateUser(userDetailModel:any):Observable<any> {
        return this.http.post<any>(`${UserProfileURLConstants.USER_PROFILE}`,userDetailModel);
    }
    uploadPhoto(formData:any,id:any):Observable<any> {
        const mapParams = new Map();
        mapParams.set('userId', id);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.post<any>(`${UserProfileURLConstants.UPLOAD_PHOTO}`,formData,{params});
    }
    getUserDetails(userId: string) {
        return this.http.get<any>(DashBoardURLConatants.LOAD_DATA ,{params:{"userId":userId}});
    }
    getCurrentUserProfile(userId: string) {
        return this.http.get<any>(DashBoardURLConatants.GET_CURRENT_USER_PROFILE ,{params:{"userId":userId}});
    }
    getAllStates(e:any) {
        return this.http.get<any>('assets/states.json')
            .toPromise()
            .then(data => { return data; });
    }
    getAllCities(e:any) {
        return this.http.get<any>('assets/cities.json')
            .toPromise()
            .then(data => { return data; });
    }
}
