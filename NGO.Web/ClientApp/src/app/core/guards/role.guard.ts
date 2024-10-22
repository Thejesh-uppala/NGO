import { Injectable } from '@angular/core';
import {
  Router,
  CanActivate,
  ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree
} from '@angular/router';
import { from } from 'rxjs';
import { AuthenticationService } from '../services/authentication.service';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  constructor(public auth: AuthenticationService, public router: Router) {

  }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot) {

    return from(this.handleRequest(next));
  }

  async handleRequest(next: ActivatedRouteSnapshot): Promise<boolean | UrlTree> | Promise<boolean | UrlTree> {
    await this.auth.getNavPermissions();

    let routePermissionCode = next.data?.routePermissionCode;

    if (routePermissionCode) {
      let modulePermission = this.auth.permissions.value.find(x => x.code == routePermissionCode);
      if (modulePermission) {
          return true;
      }
        else {
          return false;
        }
      }

    return true;
  }
}