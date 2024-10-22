import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DashBoardURLConatants } from 'src/app/shared/constants/url-constants';
import { CommonService } from 'src/app/shared/components/services/common.service';


@Injectable({ providedIn: 'root' })
export class NavBarService {
    constructor(private http: HttpClient,
        private commonService: CommonService) {
    }
    getUserDetails(userId: string) {
        debugger
        return this.http.get<any>(DashBoardURLConatants.LOAD_DATA ,{params:{"userId":userId}});
    }
}
