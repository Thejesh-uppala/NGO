import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthenticationService } from '../services/authentication.service';
import { map, catchError } from 'rxjs/operators';
import { LoaderService } from 'src/app/shared/components/loading-spinner/loader.service';


@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(
    private authenticationService: AuthenticationService,
    private loaderService: LoaderService
  ) { }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const skipLoader = this.loaderService.getSkipLoader();
    if (!skipLoader) {
      this.loaderService.setLoader(true);
    }
    
    // add auth header with jwt if user is logged in and request is to the api url
    const currentUser = this.authenticationService.currentUserValue;
    const isLoggedIn = currentUser && currentUser.token;
    const isApiUrl = request.url.startsWith(environment.apiUrl);
    if (isLoggedIn && isApiUrl) {
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
      catchError((error) => {
        this.loaderService.setLoader(false);
        throw error;
      })
    );
  }
}
