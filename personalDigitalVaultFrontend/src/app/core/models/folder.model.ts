export interface FolderItem {
  id: number;
  name: string;
  parentFolderId?: number | null;
  createdAt: string;
  documentCount: number;
  credentialCount: number;
  subFolderCount: number;
}
