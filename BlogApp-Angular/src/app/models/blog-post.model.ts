export interface BlogPost {
  id: number;
  title: string;
  content: string;
  summary: string;
  slug: string;
  featuredImageUrl: string;
  authorId: string;
  authorName: string;
  categoryId: number;
  categoryName: string;
  isPublished: boolean;
  publishedAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  viewCount: number;
  tags: string[];
  commentCount: number;
}

export interface CreateBlogPost {
  title: string;
  content: string;
  summary: string;
  featuredImageUrl: string;
  categoryId: number;
  tagIds: number[];
  isPublished: boolean;
}

export interface UpdateBlogPost {
  id: number;
  title: string;
  content: string;
  summary: string;
  featuredImageUrl: string;
  categoryId: number;
  tagIds: number[];
  isPublished: boolean;
}

export interface Category {
  id: number;
  name: string;
  description: string;
}

export interface Tag {
  id: number;
  name: string;
}

export interface Comment {
  id: number;
  content: string;
  authorName: string;
  createdAt: string;
  blogPostId: number;
} 