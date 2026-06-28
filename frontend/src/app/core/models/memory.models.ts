export interface MemoryItem {
  id: string;
  scope: string;
  type: string;
  content: string;
  ideaId?: string | null;
  sourceThreadId?: string | null;
  sourceMessageId?: string | null;
  createdAt: string;
}
