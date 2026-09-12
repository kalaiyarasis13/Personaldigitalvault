export interface CredentialListItem {
  id: number;
  title: string;
  accountUsername?: string | null;
  url?: string | null;
  folderId?: number | null;
  createdAt: string;
  updatedAt?: string | null;
  maskedPassword: string;
}

export interface CredentialReveal {
  id: number;
  title: string;
  accountUsername?: string | null;
  password: string;
  notes?: string | null;
  url?: string | null;
}
