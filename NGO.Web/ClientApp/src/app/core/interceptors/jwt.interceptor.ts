import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { map, catchError, exhaustMap, take } from 'rxjs/operators';
import { LoaderService } from 'src/app/shared/components/loading-spinner/loader.service';
import { UserState } from 'src/app/store/user/user.model';
import { selectCurrentUser } from 'src/app/store/user/user.selectors';
import { environment } from '../../../environments/environment';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(
    private store: Store<UserState>,
    private loaderService: LoaderService
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const skipLoader = this.loaderService.getSkipLoader();
    if (!skipLoader) {
      this.loaderService.setLoader(true);
    }

    return this.store.select(selectCurrentUser).pipe(
      take(1), // Get the latest token value and unsubscribe
      exhaustMap(currentUser => {
        if (currentUser && currentUser.token && request.url.startsWith(environment.apiUrl)) {
          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${currentUser.token}`,
            },
          });
        }

        return next.handle(request).pipe(
          map((event: HttpEvent<any>) => {
            if (event instanceof HttpResponse) {
              if (!skipLoader) {
                this.loaderService.setLoader(false);
              }
            }
            return event;
          }),
          catchError(error => {
            this.loaderService.setLoader(false);
            throw error;
          })
        );
      })
    );
  }
}
