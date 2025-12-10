export interface Comment {
  id: string;
  content: string;
  username: string;
  userId: string;
  createdAt: Date;
  isActive: boolean;
}

export interface CreateCommentRequest {
  content: string;
}

