import {Component, inject} from '@angular/core';
import {Navbar} from '@shared/navbar/navbar';
import {AuthService} from '../../../../../../../packages/fakeoverflow-angular-services/src';

@Component({
  selector: 'app-login',
  imports: [
    Navbar
  ],
  templateUrl: './login.html',
  standalone: true,
  styleUrl: './login.scss'
})
export class Login {
    private readonly authService = inject(AuthService);

    ngOnInit(): void {
      this.authService.login({
        type: 'Credentials',
        data: {
          username: 'admin',
          password: '123@Super'
        }
      })
    }
}
