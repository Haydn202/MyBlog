import {Component, Inject, inject, OnDestroy, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {AccountService} from '../../core/services/account.service';
import {Auth} from '../../features/auth/auth';
import {TextEditor} from '../../features/text-editor/text-editor';
import {RouterLink, RouterLinkActive} from '@angular/router';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, Auth, TextEditor, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav implements OnInit, OnDestroy {
  protected accountService = inject(AccountService);
  protected isDarkMode = signal<boolean>(false);
  private darkModeMediaQuery?: MediaQueryList;
  private mediaQueryListener?: (e: MediaQueryListEvent) => void;

  ngOnInit(): void {
    console.log(this.accountService.role())
    
    // Check for dark mode preference
    this.darkModeMediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    this.isDarkMode.set(this.darkModeMediaQuery.matches);
    
    // Listen for changes in dark mode preference
    this.mediaQueryListener = (e: MediaQueryListEvent) => {
      this.isDarkMode.set(e.matches);
    };
    this.darkModeMediaQuery.addEventListener('change', this.mediaQueryListener);
  }

  ngOnDestroy(): void {
    // Clean up the listener when component is destroyed
    if (this.darkModeMediaQuery && this.mediaQueryListener) {
      this.darkModeMediaQuery.removeEventListener('change', this.mediaQueryListener);
    }
  }

  getRubberDuckIcon(): string {
    return this.isDarkMode() 
      ? '/assets/icons/RubberDuckDark.svg' 
      : '/assets/icons/RubberDuck.svg';
  }

  Logout() {
    this.accountService.logout().subscribe({
      next: () => {
        console.log('Logged out successfully');
      },
      error: (error) => {
        console.error('Error logging out:', error);
        this.accountService.currentUser.set(null);
      }
    });
  }
}
