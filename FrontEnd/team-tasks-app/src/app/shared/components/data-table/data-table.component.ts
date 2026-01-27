import { Component, Input, Output, EventEmitter, ViewChild, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';

export interface TableColumn {
  key: string;
  header: string;
  sortable?: boolean;
  type?: 'text' | 'number' | 'date' | 'badge';
  format?: (value: any, row: any) => string;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatSortModule, MatPaginatorModule],
  templateUrl: './data-table.component.html',
  styleUrl: './data-table.component.css'
})
export class DataTableComponent implements OnInit, OnChanges {
  @Input() data: any[] = [];
  @Input() columns: TableColumn[] = [];
  @Input() showPaginator = false;
  @Input() totalCount = 0;
  @Input() pageSize = 10;
  @Input() pageIndex = 0;
  @Input() pageSizeOptions = [5, 10, 25, 50];
  @Input() highlightCondition?: (row: any) => boolean;
  @Input() rowClickable = false;

  @Output() sortChange = new EventEmitter<Sort>();
  @Output() pageChange = new EventEmitter<PageEvent>();
  @Output() rowClick = new EventEmitter<any>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  dataSource = new MatTableDataSource<any>();
  displayedColumns: string[] = [];

  ngOnInit(): void {
    this.displayedColumns = this.columns.map(c => c.key);
    this.dataSource.data = this.data;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.dataSource.data = this.data;
    }
    if (changes['columns']) {
      this.displayedColumns = this.columns.map(c => c.key);
    }
  }

  onSortChange(sort: Sort): void {
    this.sortChange.emit(sort);
  }

  onPageChange(event: PageEvent): void {
    this.pageChange.emit(event);
  }

  onRowClick(row: any): void {
    if (this.rowClickable) {
      this.rowClick.emit(row);
    }
  }

  getCellClass(column: TableColumn, row: any): string {
    if (column.type === 'badge') {
      return 'badge';
    }
    return '';
  }
}
