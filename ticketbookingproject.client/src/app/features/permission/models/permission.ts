export interface Permission {
  id: number;
  name: string;
  action: string;
  resource: string;
  description: string;
  roleStates: Record<number, boolean>; // { 1: true, 2: false, ... }
  isUpdating?: boolean;
}

export interface CreatePermissionDto {
  action: string;
  resource: string;
  description?: string;
}

export interface TogglePermissionDto {
  roleId: number;
  permissionId: number;
  isSelected: boolean;
}
export interface Role {
  id: number;
  name: string;
  description?: string;
  permissionCount: number;
}
