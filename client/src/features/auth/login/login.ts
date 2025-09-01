import {Component, inject} from '@angular/core';
import {LoginCreds} from '../../../Types/user';
import {AccountService} from '../../../core/services/account.service';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [
    FormsModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  protected accountService = inject(AccountService);
  protected creds: LoginCreds = { email: '', password: '' };

  login() {
    this.accountService.login(this.creds).subscribe({
      next: result => console.log(result),
      error: err => console.error(err.message)
    });
  }
}
