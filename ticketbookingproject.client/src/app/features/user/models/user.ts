export interface User {
  id: number,
  username: string,
  email?: string,
  firstname: string,
  lastname: string,
  status: string,
  logintype: string,
  isVerified: boolean,
  roles: string,
  createdAt?: Date, 
}
export interface UserCreate {
  firstname: string,
  lastname: string,
  username: string,
  email?: string,
  status: string,
  role: string,
  password: string,
  confirmPassword: string,
}
export enum UserStatus {
  Active = 0,
  Inactive = 1,
  Suspended = 2,
}
export enum LoginType {
  Local = 0,
  Google = 1,
  Facebook = 2,
}

export interface Permission {
  key: string;
  label: string;
}

export interface RolePermissionGroup {
  roleId: string;
  roleName: string;
  permissions: Permission[];
}

export interface StatUsers {
  totalUsers: number,
  totalUsersLastMonth: number,
  newUsersThisWeek: number,
  newUsersLastWeek: number
}

export interface StatItem {
  value: number;
  change: string;
}

export interface UserStats {
  totalUsers: StatItem;
  newUsers: StatItem;
}
