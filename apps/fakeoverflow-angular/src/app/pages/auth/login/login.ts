import { Component } from '@angular/core';
import {Navbar} from '@shared/navbar/navbar';

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

}
