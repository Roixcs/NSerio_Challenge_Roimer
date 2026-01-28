import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'statusBadge',
  standalone: true
})
export class StatusBadgePipe implements PipeTransform {
  private statusColors: Record<string, { bg: string; text: string }> = {

    // Task statuses
    'ToDo': { bg: '#e3f2fd', text: '#1565c0' },
    'InProgress': { bg: '#fff3e0', text: '#ef6c00' },
    'Blocked': { bg: '#ffebee', text: '#c62828' },
    'Completed': { bg: '#e8f5e9', text: '#2e7d32' },

    // Project statuses
    'Planned': { bg: '#f3e5f5', text: '#7b1fa2' },

    // Prioridades
    'Low': { bg: '#e8f5e9', text: '#2e7d32' },
    'Medium': { bg: '#fff3e0', text: '#ef6c00' },
    'High': { bg: '#ffebee', text: '#c62828' },

    'default': { bg: '#f5f5f5', text: '#616161' }
  };

  transform(value: string, type: 'style' | 'class' = 'style'): string {
    const colors = this.statusColors[value] || this.statusColors['default'];

    if (type === 'style') {
      return `background-color: ${colors.bg}; color: ${colors.text}; padding: 4px 12px; border-radius: 16px; font-size: 12px; font-weight: 500;`;
    }

    return value.toLowerCase().replace(/\s+/g, '-');
  }
}
