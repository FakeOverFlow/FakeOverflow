import {Component, inject} from '@angular/core';
import {Spinner} from '@services/spinner';
import {ProgressSpinner} from 'primeng/progressspinner';
import {AsyncPipe} from '@angular/common';

@Component({
  selector: 'app-full-page-spinner',
  imports: [
    ProgressSpinner,
    AsyncPipe
  ],
  templateUrl: './full-page-spinner.html',
  styleUrl: './full-page-spinner.scss'
})
export class FullPageSpinner {
  private readonly spinnerService = inject(Spinner);
  protected readonly spinner$ = this.spinnerService.currentSpinner$;
}
