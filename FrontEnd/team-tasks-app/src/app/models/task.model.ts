export interface Task {
  taskId: number;
  projectId: number;
  title: string;
  description: string | null;
  assigneeId: number | null;
  assigneeName: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  estimatedComplexity: number;
  dueDate: string | null;
  completionDate: string | null;
  createdAt: string;
}

export interface TaskDetail extends Task {
  projectName: string;
}

export interface CreateTask {
  projectId: number;
  title: string;
  description: string | null;
  assigneeId: number | null;
  status: TaskStatus;
  priority: TaskPriority;
  estimatedComplexity: number;
  dueDate: string | null;
}

export interface UpdateTaskStatus {
  status: TaskStatus;
  priority?: TaskPriority;
  estimatedComplexity?: number;
}

export type TaskStatus = 'ToDo' | 'InProgress' | 'Blocked' | 'Completed';
export type TaskPriority = 'Low' | 'Medium' | 'High';

export const TASK_STATUSES: TaskStatus[] = ['ToDo', 'InProgress', 'Blocked', 'Completed'];
export const TASK_PRIORITIES: TaskPriority[] = ['Low', 'Medium', 'High'];
