export interface PermissionResponse {
  id: number;
  name: string;
  action: string;
  resource: string;
  description?: string | null;
}

export interface RoleResponse {
  id: number;
  name: string;
  description?: string | null;
  permissions: PermissionResponse[];
}

export interface RoleListResponse {
  id: number;
  name: string;
  description?: string | null;
  permissionCount: number;
}

export interface CreateRoleRequest {
  name: string;
  description?: string | null;
  permissionNames: string[];
}

export interface UpdateRoleRequest {
  description?: string | null;
  permissionIds?: string[]; // optional giống backend
}

export interface ListRoleRequest {
  keyword?: string | null;
  permissionNames?: string[] | null;
  page: number;
  pageSize: number;
}
