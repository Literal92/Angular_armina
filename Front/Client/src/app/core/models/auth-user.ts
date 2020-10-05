import { ShemaRoleClaimsToken } from '..';

export interface AuthUser {
  userId: string;
  userName: string;
  serialNumber?: string;
  displayName: string;
  userType: string;
  roles: string[] | null;
  roleClaims: ShemaRoleClaimsToken [] | null;
}
