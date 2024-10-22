import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashBoardURLConatants } from 'src/app/shared/constants/url-constants';
import { CommonService } from 'src/app/shared/components/services/common.service';


@Injectable({ providedIn: 'root' })
export class AdminService {

    constructor(private http: HttpClient,
        private commonService: CommonService) {
    }
    getAllUsers(): Observable<any> {
        return this.http.get<any>(`${DashBoardURLConatants.GET_ALL_USERS}`);
    }
    getPaymentDetails(userId: any): Observable<any> {
        const mapParams = new Map();
        mapParams.set('userId', userId);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.get<any>(`${DashBoardURLConatants.GET_PAYMENT_DETAILS}`, { params });
    }
    paymentReminder(userId: any): Observable<any> {
        const mapParams = new Map();
        mapParams.set('userId', userId);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.post<any>(`${DashBoardURLConatants.PAYMENT_REMINDER}`, {}, { params });
    }
    saveMember(userId: any,memberId:any,chapterId:any,organizationId:any): Observable<any> {
        debugger
        const mapParams = new Map();
        mapParams.set('userId', userId);
        mapParams.set('chapterId', chapterId);
        mapParams.set('organizationId', organizationId);
        mapParams.set('memberId', memberId);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.post<any>(`${DashBoardURLConatants.SAVE_MEMBER}`, {}, { params });
    }
    massPaymentRem(filterMassPaymentModels: any): Observable<any> {
        return this.http.post<any>(`${DashBoardURLConatants.MASS_PAYMENT_REMINDER}`, filterMassPaymentModels);
    }
    getAllUserDetails(): Observable<any> {
        return this.http.get<any>(DashBoardURLConatants.GET_ALL_USERS);
    }
    getOrganizations(): Observable<any> {
        return this.http.get<any>(DashBoardURLConatants.GET_ALL_ORGANIZATIONS);
    }
    getChapters(): Observable<any> {
        return this.http.get<any>(DashBoardURLConatants.GET_ALL_CHAPTERS);
    }
}
