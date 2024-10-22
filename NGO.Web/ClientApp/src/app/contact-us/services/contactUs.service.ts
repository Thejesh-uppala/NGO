import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ContactUSURLConatants } from 'src/app/shared/constants/url-constants';
import { CommonService } from 'src/app/shared/components/services/common.service';


@Injectable({ providedIn: 'root' })
export class ContactUsService {
    constructor(private http: HttpClient,
        private commonService: CommonService) {
    }
    sendMessage(value:any) {
        debugger
        const mapParams = new Map();
        mapParams.set('contactFormName', value.contactFormName);
        mapParams.set('contactFormEmail', value.contactFormEmail);
        mapParams.set('contactFormSubjects', value.contactFormSubjects);
        mapParams.set('contactFormMessage', value.contactFormMessage);
        const params = this.commonService.getHttpParams(mapParams);
        return this.http.post<any>(`${ContactUSURLConatants.SEND_MAIL}`,{}, { params });
      }
}
