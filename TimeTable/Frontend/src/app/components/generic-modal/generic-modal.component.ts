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
  @Input() isWideModal: boolean = false;

  @Input() showCancelButton: boolean = true;
  
  @Output() confirmEvent = new EventEmitter<{ confirmed: boolean, inputValue?: string }>(); // Emit inputValue on confirm
  @Output() inputChange = new EventEmitter<string>(); // Emit whenever the input value changes

  close() {
    this.isVisible = false;
  }

  cancel() {
    this.isVisible = false;
    this.confirmEvent.emit({ confirmed: false });
  }

  confirm() {
    this.isVisible = false;
    // Emit confirmed as true and also send the current inputValue
    this.confirmEvent.emit({ confirmed: true, inputValue: this.inputValue });
  }

  onInputChange() {
    // This method will emit the new value whenever the user changes the input
    this.inputChange.emit(this.inputValue);
  }

  checkInputValidity() {
    // This function is used to trigger Angular's change detection on input change
  }

}
