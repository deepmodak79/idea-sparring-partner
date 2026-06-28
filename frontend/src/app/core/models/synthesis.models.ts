export interface Synthesis {
  id: string;
  ideaId: string;
  version: number;
  strongestChallenges: string[];
  weakestReasoning: string[];
  unresolvedTensions: string[];
  createdAt: string;
}
