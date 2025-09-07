import {Component, inject} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {RegisterCreds} from '../../../Types/user';
import {AccountService} from '../../../core/services/account.service';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  protected creds = {} as RegisterCreds;
  private accountService = inject(AccountService);

  register() {
    this.accountService.signup(this.creds).subscribe();
  }
}
