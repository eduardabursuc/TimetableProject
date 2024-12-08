import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.css'],
  imports: [CommonModule, FormsModule],
})
export class TableComponent<T extends object> {
  @Input() data: T[] = []; // The list of entities to display
  @Input() columns: { field: keyof T; label: string }[] = []; // Column definitions
  @Input() entityTemplate: T | null = null; // Empty entity template for creation
  @Input() showActions = true; // Whether to show action buttons
  @Output() delete = new EventEmitter<T>(); // Emit the entity to delete
  @Output() update = new EventEmitter<T>(); // Emit the entity to update
  @Output() create = new EventEmitter<T>(); // Emit the new entity to create

  filteredData: T[] = []; // Filtered data for search
  searchText: string = ''; // Search input text
  isCreating = false; // Tracks if a new row is being created
  newEntity: T | null = null; // Placeholder for the new entity
  editingEntity: T | null = null; // Placeholder for the entity being edited
  sortField: keyof T | null = null; // Currently sorted field
  sortDirection: 'asc' | 'desc' = 'asc'; // Current sort direction

  constructor(private snackBar: MatSnackBar) {}

  ngOnInit(): void {
    this.filteredData = [...this.data];
  }

  ngOnChanges(): void {
    this.filteredData = [...this.data];
  }

  /**
   * Filter the data based on the search text.
   */
  onSearch(): void {
    if (this.searchText.trim() === '') {
      this.filteredData = [...this.data];
      return;
    }

    this.filteredData = this.data.filter((entity) =>
      Object.values(entity).some((value) =>
        value.toString().toLowerCase().includes(this.searchText.toLowerCase())
      )
    );
  }

  /**
   * Emit the delete event for a specific entity.
   * @param entity - The entity to delete.
   */
  onDelete(entity: T): void {
    const confirmed = confirm('Are you sure you want to delete this entity?');
    if (confirmed) {
      this.delete.emit(entity);
      this.showSnackBar('Entity deleted successfully.', 'success-snackbar');
    }
  }

  /**
   * Start editing a specific entity.
   * @param entity - The entity to edit.
   */
  startEdit(entity: T): void {
    this.editingEntity = { ...entity }; // Clone the entity for editing
  }

  /**
   * Save the updated entity.
   */
  finishUpdate(): void {
    if (!this.editingEntity) return;

    this.update.emit(this.editingEntity);
    this.editingEntity = null; // Reset editing state
    this.showSnackBar('Entity updated successfully.', 'success-snackbar');
  }

  /**
   * Cancel editing a specific entity.
   */
  cancelEdit(): void {
    this.editingEntity = null;
  }

  /**
   * Initialize creation of a new entity.
   */
  startCreate(): void {
    if (!this.entityTemplate) {
      this.showSnackBar('Entity template is required for creation.', 'error-snackbar');
      return;
    }
    this.isCreating = true;
    this.newEntity = { ...this.entityTemplate }; // Clone the template
    this.filteredData.unshift(this.newEntity); // Add it to the table temporarily
  }

  /**
   * Emit the create event and reset creation state.
   */
  finishCreate(): void {
    if (!this.newEntity) return;

    this.create.emit(this.newEntity);
    this.isCreating = false;
    this.newEntity = null; // Reset the new entity
    this.showSnackBar('Entity created successfully.', 'success-snackbar');
  }

  /**
   * Cancel creation of a new entity.
   */
  cancelCreate(): void {
    this.isCreating = false;
    this.newEntity = null;
    this.filteredData.shift(); // Remove the temporary entry from the table
  }

  /**
   * Sort the table data by a specific column.
   * @param field - The field to sort by.
   */
  sortBy(field: keyof T): void {
    if (this.sortField === field) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortDirection = 'asc';
    }

    this.filteredData.sort((a, b) => {
      const aValue = a[field];
      const bValue = b[field];

      if (aValue === bValue) return 0;

      const result = aValue > bValue ? 1 : -1;
      return this.sortDirection === 'asc' ? result : -result;
    });
  }

  /**
   * Show a snack bar notification.
   * @param message - The message to display.
   * @param panelClass - The class for the notification type.
   */
  private showSnackBar(message: string, panelClass: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: [panelClass],
      horizontalPosition: 'right',
      verticalPosition: 'top',
    });
  }
}