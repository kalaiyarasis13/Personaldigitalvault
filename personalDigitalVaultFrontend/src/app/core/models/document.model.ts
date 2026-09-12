export interface DocumentItem {
  id: number;
  originalFileName: string;
  contentType: string;
  sizeBytes: number;
  fileHash: string;
  folderId?: number | null;
  uploadedAt: string;
  updatedAt?: string | null;
}
