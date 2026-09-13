export interface UserProfile {
  id: number;
  username: string;
  email: string;
  fullName?: string | null;
  role: "User" | "Administrator";
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: UserProfile;
}
