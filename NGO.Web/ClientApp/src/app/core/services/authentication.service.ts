import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthModel } from 'src/app/login/models/login.model';
import { BrandURLConstants, LoginURLConstants } from 'src/app/shared/constants/url-constants';
import { NavPermission } from 'src/app/shared/models/role.model';
import { eventClearOnlogout } from 'src/app/store/events/event.actions';
import { RegisteredEventState } from 'src/app/store/events/event.model';
import { logout } from 'src/app/store/user/user.actions';
AuthModel;

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private currentUserSubject: BehaviorSubject<AuthModel>;
  public currentUser: Observable<AuthModel>;
  public permissions: BehaviorSubject<NavPermission[]> = new BehaviorSubject<NavPermission[]>([]);
  userClaims: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  orgId!: number;

  constructor(
    private http: HttpClient,
    private router: Router,
    private store: Store,
    private eventStore: Store<RegisteredEventState>
  ) {
    const user = JSON.parse(localStorage.getItem('currentUser') || '{}');
    this.currentUserSubject = new BehaviorSubject<AuthModel>(user);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  login(username: string, password: string, storedOrgId: string) {
    return this.http.post<AuthModel>(LoginURLConstants.LOGIN(storedOrgId), { username, password }).pipe(
      map(user => {
        // store user details and jwt token in local storage to keep user logged in between page refreshes
        localStorage.setItem('currentUser', JSON.stringify(user));
        return user;
      })
    );
  }

  getBrandDetails(orgId: string): Observable<any> {
    return this.http.get<any>(BrandURLConstants.GetBrand(orgId));
  }

  setUserContext(user: AuthModel) {
    this.currentUserSubject.next(user);
  }

  public get currentUserValue(): AuthModel {
    return this.currentUserSubject.value;
  }

  logout() {
    this.orgId = parseInt(localStorage.getItem('orgId') || '0', 10);

    this.store.dispatch(logout());
    this.eventStore.dispatch(eventClearOnlogout());
    // remove user from local storage to log user out
    localStorage.removeItem('currentUser');
    //this.currentUserSubject.next(null);
    this.router.navigate([`/login/${this.orgId}`]);
  }
}
