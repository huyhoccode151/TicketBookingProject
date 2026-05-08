export interface UIActionResponse {
  id: number;
  actionKey: string;
  label: string;
  icon?: string | null;
  routePath?: string | null;
  permissionRequired?: string | null;
  actionType: string;
  parentId?: number | null;
  displayOrder: number;
  isActive: boolean;
  children?: UIActionResponse[];
}

export interface UIActionRequest {
  actionKey: string;
  label: string;
  icon?: string | null;
  routePath?: string | null;
  permissionRequired?: string | null;
  actionType: string;
  parentId?: number | null;
  displayOrder: number;
  isActive: boolean;
}

export interface ListUIActionRequest {
  keyword?: string | null;
  actionType?: string | null;
  isActive?: boolean | null;
  page: number;
  pageSize: number;
}
