export interface Identity {
  id: string;
  name: string;
  avatarUrl?: string | null;
  secrets: Credentials;
}

export interface Credentials {
  accessToken: string;
  refreshToken: string;
}
