import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common'; // <-- important
import { AccountService } from '../../core/services/account.service';
import { Login } from './login/login';
import { Register } from './register/register';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [
    FormsModule,   // ngModel
    CommonModule,  // structural directives (ngIf, ngFor)
    Login,
    Register
  ],
  templateUrl: './auth.html',
  styleUrls: ['./auth.css']
})
export class Auth {
  private accountService = inject(AccountService);

  isModalOpen = false;
  isLoggingIn = true;

  openModal() {
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  toggleMode() {
    this.isLoggingIn = !this.isLoggingIn;
  }
}
