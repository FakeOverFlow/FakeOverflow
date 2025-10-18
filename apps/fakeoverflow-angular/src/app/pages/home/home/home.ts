import { Component } from '@angular/core';
import {Navbar} from '@shared/navbar/navbar';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [
    Navbar, RouterLink
  ],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {

}
