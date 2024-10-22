import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DashBoardURLConatants, RegistrationURLConstants } from 'src/app/shared/constants/url-constants';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { Customer } from './product';
import { Observable } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class DashBoardService {
    constructor(private http: HttpClient,
        private commonService: CommonService) {
    }
    getUserDetails(userId: string) {
        return this.http.get<any>(DashBoardURLConatants.LOAD_DATA ,{params:{"userId":userId}});
    }
    getAllUserDetails() {
        return this.http.get<any>(DashBoardURLConatants.GET_ALL_USERS);
    }
    contactMe(userId: number,currentUserId:any) {
        const mapParams = new Map();
        mapParams.set('userId', userId);
        mapParams.set('currentUserId', currentUserId);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.post<any>(`${DashBoardURLConatants.CONTACT_ME}`,{}, { params });
    }
    getOrganizations(): Observable<any> {
        return this.http.get<any>(DashBoardURLConatants.GET_ALL_ORGANIZATIONS);
    }
    getCustomersLarge() {
        return this.http.get<any>('assets/products.json')
            .toPromise()
            .then(res => <Customer[]>res.data)
            .then(data => { return data; });
    }
}
