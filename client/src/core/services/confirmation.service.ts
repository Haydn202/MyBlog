import { Injectable, signal } from '@angular/core';

export interface ConfirmationModalOptions {
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  confirmButtonClass?: string;
}

/**
 * Global confirmation service for showing confirmation modals throughout the application.
 *
 * Usage example:
 * ```typescript
 * constructor(private confirmationService = inject(ConfirmationService)) {}
 *
 * async deleteItem() {
 *   const confirmed = await this.confirmationService.confirm({
 *     title: 'Delete Item',
 *     message: 'Are you sure you want to delete this item?',
 *     confirmText: 'Delete',
 *     cancelText: 'Cancel',
 *     confirmButtonClass: 'btn-error'
 *   });
 *
 *   if (confirmed) {
 *     // Perform the action
 *   }
 * }
 * ```
 */
@Injectable({
  providedIn: 'root'
})
export class ConfirmationService {
  // Modal state
  isOpen = signal(false);
  title = signal('Confirm Action');
  message = signal('');
  confirmText = signal('Confirm');
  cancelText = signal('Cancel');
  confirmButtonClass = signal('btn-primary');

  private resolvePromise!: (value: boolean | PromiseLike<boolean>) => void;

  confirm(options: ConfirmationModalOptions): Promise<boolean> {
    this.title.set(options.title || 'Confirm Action');
    this.message.set(options.message);
    this.confirmText.set(options.confirmText || 'Confirm');
    this.cancelText.set(options.cancelText || 'Cancel');
    this.confirmButtonClass.set(options.confirmButtonClass || 'btn-primary');
    this.isOpen.set(true);

    return new Promise<boolean>(resolve => {
      this.resolvePromise = resolve;
    });
  }

  onConfirmed() {
    this.isOpen.set(false);
    this.resolvePromise(true);
  }

  onCancelled() {
    this.isOpen.set(false);
    this.resolvePromise(false);
  }
}
