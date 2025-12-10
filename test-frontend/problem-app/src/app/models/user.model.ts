export interface User {
  id: string;
  username: string;
  email: string;
  passwordHash: string;
  createdAt: Date;
  isActive: boolean;
}

export interface OnlineUser {
  id: string;
  username: string;
  email: string;
  isActive: boolean;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  email: string;
  expiresAt: Date;
}
