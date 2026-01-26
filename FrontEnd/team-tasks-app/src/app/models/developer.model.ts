export interface Developer {
  developerId: number;
  fullName: string;
  email: string;
}

export interface DeveloperWorkload {
  developerId: number;
  developerName: string;
  email: string;
  openTasksCount: number;
  averageEstimatedComplexity: number;
}

export interface DeveloperDelayRisk {
  developerId: number;
  developerName: string;
  email: string;
  openTasksCount: number;
  avgDelayDays: number;
  nearestDueDate: string | null;
  latestDueDate: string | null;
  predictedCompletionDate: string | null;
  highRiskFlag: number;
}
