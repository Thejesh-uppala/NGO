import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { EventURLConstants } from 'src/app/shared/constants/url-constants';
interface EventData {
  id: number;
  orgId: number;
  eventTitle: string;
  eventDesc: string;
  eventRequirements: string;
  eventPhoto: string;
  eventAttendenceTerms: string;
  eventContactEmail: string;
  eventContactPhone: string;
  additionalInfo: string;
  eventAccess: string;
  eventTimeUtc: string;
  eventLocation: string;
  eventCosts: Array<any>;
}

@Injectable({ providedIn: 'root' })
export class EventService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}

  getAllEvent(orgId: number): Observable<any> {
    return this.http.get<any>(`${EventURLConstants.GET_ALL_EVENTS(orgId)}`);
  }

  registerEvent(orgId: number, payload: any): Observable<any> {
    return this.http.post<any>(EventURLConstants.REGISTER_EVENT(orgId), payload);
  }

  getEventById(orgId: number, eventId: number): Observable<EventData> {
    const params = new HttpParams().set('orgId', orgId.toString()).set('EventId', eventId.toString());
    return this.http.get<EventData>(`${EventURLConstants.GET_EVENT_BY_ID}`, { params });
  }

  sendEventData(data: FormData): Observable<any> {
    return this.http.post(`${EventURLConstants.CREATE_EVENT}`, data);
  }
  updateEventData(data: FormData): Observable<any> {
    return this.http.put(`${EventURLConstants.UPDATE_EVENT}`, data);
  }
  getPaginatedEvents(filters: any, orgId: number, page: number, pageSize: number): Observable<any> {
    // Construct the URL dynamically by using the orgId

    const url = `${EventURLConstants.GET_PAGINATED_EVENTS(orgId, page, pageSize)}`;

    // Construct query parameters with the filters
    const params = {
      startdate: filters.startdate || '',
      enddate: filters.enddate || '',
      searchValue: filters.searchValue || '',
      eventtype: filters.eventtype, // Add searchValue parameter
    };

    // Return the HTTP GET request with the URL and query parameters
    return this.http.get<any>(url, { params });
  }

  getRegisteredUserPaginatedEvents(eventId: number, page: number, pageSize: number): Observable<any> {
    // Construct the URL dynamically by using the orgId

    const url = `${EventURLConstants.GET_USERREGISTERED_PAGINATED_EVENTS(eventId, page, pageSize)}`;

    // Return the HTTP GET request with the URL and query parameters
    return this.http.get<any>(url);
  }
}
