import {Component, inject} from '@angular/core';
import {LoginCreds} from '../../../Types/user';
import {AccountService} from '../../../core/services/account.service';
import {FormsModule} from '@angular/forms';
import {ToastService} from '../../../core/services/toast.service';

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
  private toast = inject(ToastService);
  protected creds: LoginCreds = { email: '', password: '' };

  login() {
    this.accountService.login(this.creds).subscribe({
      next: result => {
        this.toast.success('Welcome ' + result.userName + '!');
      },
      error: err => {
       this.toast.error(err.error.errors[0]);
      }
    });
  }
}
