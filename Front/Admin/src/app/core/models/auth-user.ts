import { ShemaRoleClaimsToken, ShemaUserClaimsToken } from '..';

export interface AuthUser {
  userId: string;
  userName: string;
  serialNumber?: string;
  displayName: string;
  roles: string[] | null;
  roleClaims: ShemaRoleClaimsToken[] | null;
  userClaims: ShemaUserClaimsToken[] | null;
}
