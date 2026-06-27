export interface UserDto {
  id: string;
  email: string;
  displayName: string;
  createdAt?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserDto;
}

export interface TokenRefreshResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export interface SignupRequest {
  email: string;
  password: string;
  displayName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}
