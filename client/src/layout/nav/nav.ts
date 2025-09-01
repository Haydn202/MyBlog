import {Component, Inject, inject, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {AccountService} from '../../core/services/account.service';
import {Auth} from '../../features/auth/auth';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, Auth],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav {
  protected accountService = inject(AccountService);

  Logout() {
    this.accountService.logout();
  }
}
