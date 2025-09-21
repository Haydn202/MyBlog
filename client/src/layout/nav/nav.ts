import {Component, Inject, inject, OnInit, signal} from '@angular/core';
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
export class Nav implements OnInit {
  ngOnInit(): void {
      console.log(this.accountService.role())
  }
  protected accountService = inject(AccountService);

  Logout() {
    this.accountService.logout();
  }
}
