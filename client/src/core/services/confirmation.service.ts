import { Injectable, signal } from '@angular/core';

export interface ConfirmationOptions {
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

  // Promise resolvers
  private resolvePromise: ((confirmed: boolean) => void) | null = null;

  /**
   * Show confirmation modal and return a promise that resolves to true if confirmed, false if cancelled
   */
  confirm(options: ConfirmationOptions): Promise<boolean> {
    return new Promise<boolean>((resolve) => {
      this.resolvePromise = resolve;
      
      // Set modal options
      this.title.set(options.title || 'Confirm Action');
      this.message.set(options.message);
      this.confirmText.set(options.confirmText || 'Confirm');
      this.cancelText.set(options.cancelText || 'Cancel');
      this.confirmButtonClass.set(options.confirmButtonClass || 'btn-primary');
      
      // Show modal
      this.isOpen.set(true);
    });
  }

  /**
   * Handle confirmation
   */
  onConfirmed() {
    this.isOpen.set(false);
    if (this.resolvePromise) {
      this.resolvePromise(true);
      this.resolvePromise = null;
    }
  }

  /**
   * Handle cancellation
   */
  onCancelled() {
    this.isOpen.set(false);
    if (this.resolvePromise) {
      this.resolvePromise(false);
      this.resolvePromise = null;
    }
  }
}
