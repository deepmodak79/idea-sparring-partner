export interface ThreadSummary {
  id: string;
  persona: string;
  status: string;
  messageCount: number;
}

export interface Idea {
  id: string;
  title: string;
  description: string;
  createdAt: string;
  updatedAt: string;
  threads: ThreadSummary[];
}

export interface CreateIdeaRequest {
  title: string;
  description: string;
}
