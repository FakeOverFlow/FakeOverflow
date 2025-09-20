import {Component, inject, OnInit, signal} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {Button} from 'primeng/button';
import {Spinner} from '@services/spinner';
import {FullPageSpinner} from '@shared/full-page-spinner/full-page-spinner';
import {Common} from '@services/common';
import {Authentication} from '@services/authentication';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FullPageSpinner],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit{

  private readonly spinnerService = inject(Spinner);
  private readonly authenticationService = inject(Authentication);

    ngOnInit(): void {
      this.authenticationService.setIdentity({
        id: '1',
        name: 'Fake User',
        avatarUrl: 'https://avatars.githubusercontent.com/u/10179525?v=4',
        secrets: {
          accessToken: 'fake-access-token',
          refreshToken: 'fake-refresh-token'
        }
      }, {
        redirectAfterLogin: true,
        redirectTo: '/auth/login'
      })
      setTimeout(() => {
        const spinner = this.spinnerService.for('Loading data...');
        setTimeout(() => spinner.release(), 3000);
        setTimeout(() => this.authenticationService.logout({redirectToLogout: false}), 6000);
      }, 1000);
    }
}
