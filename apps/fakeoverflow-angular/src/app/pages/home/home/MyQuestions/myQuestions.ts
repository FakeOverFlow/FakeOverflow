import { Component } from '@angular/core';
import {Navbar} from '@shared/navbar/navbar';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-my-questions',
  imports: [
    Navbar, RouterLink
  ],
  templateUrl: './myQuestions.html',
  styleUrl: './myQuestions.scss'
})
export class MyQuestions {

}
