export interface AuditLogChange {
  field: string;
  oldValue: any | null;
  newValue: any | null;
}

export interface AuditActor {
  username?: string;
  email?: string;
}

export class AuditLog {
  id: string;
  action: string;
  entityType: string;
  entityId?: string | number;
  user?: AuditActor;
  createdAt: Date;
  description?: string;
  changes?: AuditLogChange[];
  metadata?: Record<string, any>;

  constructor(init?: Partial<AuditLog>) {
    this.id = init?.id ?? '';
    this.action = init?.action ?? '';
    this.entityType = init?.entityType ?? '';
    this.entityId = init?.entityId;
    this.user = init?.user;
    this.createdAt = init?.createdAt ? new Date(init.createdAt) : new Date();
    this.description = init?.description;
    this.changes = init?.changes ? [...init.changes] : [];
    this.metadata = init?.metadata ? { ...init.metadata } : {};
  }

  static fromJson(obj: any): AuditLog {
    if (!obj) throw new Error('Invalid object to create AuditLog');

    return new AuditLog({
      id: obj.id?.toString() ?? '',
      action: obj.action ?? '',
      entityType: obj.entityType ?? '',
      entityId: obj.entityId,
      description: obj.description,
      createdAt: obj.createdAt ? new Date(obj.createdAt) : new Date(),

      user: obj.user
        ? {
          username: obj.user.username,
          email: obj.user.email,
        }
        : undefined,

      changes: Array.isArray(obj.changes)
        ? obj.changes.map((c: any) => ({
          field: c.field,
          oldValue: c.oldValue ?? null,
          newValue: c.newValue ?? null,
        }))
        : [],

      metadata: obj.metadata ?? {},
    });
  }

  toJson(): any {
    return {
      id: this.id,
      action: this.action,
      entityType: this.entityType,
      entityId: this.entityId,
      description: this.description,
      createdAt: this.createdAt.toISOString(),

      user: this.user
        ? {
          username: this.user.username,
          email: this.user.email,
        }
        : null,

      changes: this.changes?.map(c => ({
        field: c.field,
        oldValue: c.oldValue ?? null,
        newValue: c.newValue ?? null,
      })) ?? [],

      metadata: this.metadata ?? {},
    };
  }
}

export interface AuditLogQuery {
  page?: number;
  pageSize?: number;
  search?: string;
}
