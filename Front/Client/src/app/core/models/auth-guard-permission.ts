export interface AuthGuardPermission {
  permittedRoles?: string[];
  permittedRoleClaims?: ShemaUrl[];
  deniedRoles?: string[];
}

export interface ShemaUrl {
  area ?: string;
  controller : string;
  action : string;
}
export interface ShemaRoleClaimsToken {
  type ?: string;
  value ?: string;
}
