import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-generic-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './generic-modal.component.html',
  styleUrls: ['./generic-modal.component.css']
})
export class GenericModalComponent {
  @Input() isVisible: boolean = false;
  @Input() title: string = '';
  @Input() message: string = '';
  @Input() isInputRequired: boolean = false;
  @Input() inputPlaceholder: string = '';
  @Input() inputValue: string = '';
  @Input() showCancelButton: boolean = true;
  
  @Output() confirmEvent = new EventEmitter<{ confirmed: boolean }>();

  close() {
    this.isVisible = false;
  }

  cancel() {
    this.isVisible = false;
    this.confirmEvent.emit({ confirmed: false });
  }

  confirm() {
    this.isVisible = false;
    this.confirmEvent.emit({ confirmed: true });
  }

  checkInputValidity() {
    // This function is used to trigger Angular's change detection on input change
  }
}
