import { Injectable } from "@angular/core";
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanActivateChild,
  CanLoad,
  Data,
  Route,
  Router,
  RouterStateSnapshot,
} from "@angular/router";

import { AuthGuardPermission } from "../models/auth-guard-permission";
import { AuthService } from "./auth.service";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild, CanLoad {

  private permissionObjectKey = "permission";

  constructor(private authService: AuthService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const permissionData = route.data[this.permissionObjectKey] as AuthGuardPermission;
    const returnUrl = state.url;
    return this.hasAuthUserAccessToThisRoute(permissionData, returnUrl);
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const permissionData = childRoute.data[this.permissionObjectKey] as AuthGuardPermission;
    const returnUrl = state.url;
    return this.hasAuthUserAccessToThisRoute(permissionData, returnUrl);
  }

  canLoad(route: Route): boolean {
    if (route.data) {
      const permissionData = route.data[this.permissionObjectKey] as AuthGuardPermission;
      const returnUrl = `/${route.path}`;
      return this.hasAuthUserAccessToThisRoute(permissionData, returnUrl);
    } else {
      return true;
    }
  }

  private hasAuthUserAccessToThisRoute(permissionData: Data, returnUrl: string): boolean {
    if (!this.authService.isAuthUserLoggedIn()) {
      this.showAccessDenied(returnUrl);
      return false;
    }

    if (!permissionData) {
      return true;
    }
    //  permittedRoles & permittedRoleClaims & deniedRoles
    if (Array.isArray(permissionData.permittedRoles)
        && Array.isArray(permissionData.permittedRoleClaims)
        && Array.isArray(permissionData.deniedRoles)
    ) {
      throw new Error("Don't set all three 'permittedRoles'  and 'permittedRoleClaims' and  'deniedRoles' in route data.");
    }
    // permittedRoles & deniedRoles
    if (Array.isArray(permissionData.permittedRoles) && Array.isArray(permissionData.deniedRoles)) {
      throw new Error("Don't set both 'permittedRoles' and 'deniedRoles' in route data.");
    }
    // permittedRoleClaims & deniedRoles
    if (Array.isArray(permissionData.permittedRoleClaims) && Array.isArray(permissionData.deniedRoles)) {
      throw new Error("Don't set both 'permittedRoleClaims' and 'deniedRoles' in route data.");
    }

    // permittedRoles && permittedRoleClaims
    if (Array.isArray(permissionData.permittedRoles) && Array.isArray(permissionData.permittedRoleClaims)) {
      const isInRole = this.authService.isAuthUserInRoles(permissionData.permittedRoles);
      const isInClaim = this.authService.isAuthUserInRoleClaims(permissionData.permittedRoleClaims);
      if (isInRole && isInClaim) {
        return true;
      }
      this.showAccessDenied(returnUrl);
      return false;
    }
    // permittedRoles
    if (Array.isArray(permissionData.permittedRoles)) {
      const isInRole = this.authService.isAuthUserInRoles(permissionData.permittedRoles);
      if (isInRole) {
        return true;
      }

      this.showAccessDenied(returnUrl);
      return false;
    }
    // roleClaims
    if (Array.isArray(permissionData.permittedRoleClaims)) {
      const isInClaim = this.authService.isAuthUserInRoleClaims(permissionData.permittedRoleClaims);
      if (isInClaim) {
        return true;
      }

      this.showAccessDenied(returnUrl);
      return false;
    }

    // deniedRoles
    if (Array.isArray(permissionData.deniedRoles)) {
      const isInRole = this.authService.isAuthUserInRoles(permissionData.deniedRoles);
      if (!isInRole) {
        return true;
      }

      this.showAccessDenied(returnUrl);
      return false;
    }

    return true;
  }

  private showAccessDenied(returnUrl: string) {
    this.router.navigate(["/login"], { queryParams: { returnUrl: returnUrl } });
  }
}
