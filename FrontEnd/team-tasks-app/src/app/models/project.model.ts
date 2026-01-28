export interface Project {
  projectId: number;
  name: string;
  clientName: string;
  status: string;
  totalTasks: number;
  openTasks: number;
  completedTasks: number;
}

export interface ProjectHealth {
  projectId: number;
  projectName: string;
  clientName: string;
  projectStatus: string;
  totalTasks: number;
  openTasks: number;
  completedTasks: number;
}
