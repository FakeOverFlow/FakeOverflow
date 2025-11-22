import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '@shared/navbar/navbar';

@Component({
  selector: 'app-view-question',
  standalone: true,
  imports: [CommonModule, Navbar],
  templateUrl: './view-question.html',
  styleUrls: []
})
export class ViewQuestion {

}
