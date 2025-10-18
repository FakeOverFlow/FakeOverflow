import {Component, inject, OnInit, signal} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {Spinner} from '@services/spinner';
import {FullPageSpinner} from '@shared/full-page-spinner/full-page-spinner';
import {Authentication} from '@services/authentication';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, FullPageSpinner],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit{

  private readonly spinnerService = inject(Spinner);
  private readonly authenticationService = inject(Authentication);

    ngOnInit(): void {

    }
}
