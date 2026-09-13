export interface ShareLink {
    token: string;
    shareUrl: string;
    expiresAt: string;
  }
  
  export interface ShareLinkListItem {
    id: number;
    token: string;
    documentId: number;
    documentFileName: string;
    expiresAt: string;
    isRevoked: boolean;
    downloadCount: number;
    createdAt: string;
  }
  