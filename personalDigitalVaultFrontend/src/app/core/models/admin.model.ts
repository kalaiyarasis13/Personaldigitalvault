export interface DashboardStats {
  totalUsers: number;
  activeUsers: number;
  totalUploads: number;
  totalStoredFiles: number;
  totalStorageBytes: number;
  totalCredentialRecords: number;
}

export interface AdminUserListItem {
  id: number;
  username: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  documentCount: number;
  folderCount: number;
}
