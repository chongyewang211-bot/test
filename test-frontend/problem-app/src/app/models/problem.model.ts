export interface Problem {
  id: string;
  problemNumber: number;
  title: string;
  description: string;
  difficulty: 'Easy' | 'Medium' | 'Hard';
  category: string;
  tags: string[];
  acceptanceRate: number;
  likes: number;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface Category {
  id: string;
  key: string;
  name: string;
  icon: string;
  color: string;
  problemCount: number;
  easyCount: number;
  mediumCount: number;
  hardCount: number;
  isActive: boolean;
  createdAt: Date;
}

