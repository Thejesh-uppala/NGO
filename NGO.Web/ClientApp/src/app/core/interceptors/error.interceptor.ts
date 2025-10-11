import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { LoaderService } from 'src/app/shared/components/loading-spinner/loader.service';
import { AuthenticationService } from '../services/authentication.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private loaderService: LoaderService,
    private authenticationService: AuthenticationService
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError(err => {
        if (err.status === 401) {
          this.loaderService.setLoader(false);
          // auto logout if 401 response returned from api
          this.authenticationService.logout();
        }

        const error = err.error || err.error.message || err.statusText;
        return throwError(error);
      })
    );
  }
}
