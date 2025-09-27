import {Component, inject, OnInit, signal} from '@angular/core';
import {Router, RouterOutlet} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {Nav} from '../layout/nav/nav';
import {AccountService} from '../core/services/account.service';
import {ConfirmationModal} from '../shared/components/confirmation-modal/confirmation-modal';

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet, ConfirmationModal],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected accountService = inject(AccountService);
  protected router = inject(Router);
  private http = inject(HttpClient);
  protected readonly title = signal('client');

  async ngOnInit() {
    this.setCurrentUser();
  };

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) {
      return;
    }

    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }
}
