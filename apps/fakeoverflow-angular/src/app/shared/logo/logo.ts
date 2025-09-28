import {Component, input, Input} from '@angular/core';

@Component({
  selector: 'app-logo',
  imports: [],
  standalone: true,
  templateUrl: './logo.html',
  styleUrl: './logo.scss'
})
export class Logo {
  logoSize = input(24);
  logoColor = input('currentColor');
}
