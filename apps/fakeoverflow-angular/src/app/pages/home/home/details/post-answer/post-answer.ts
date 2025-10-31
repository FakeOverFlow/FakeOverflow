import {Component, input, signal} from '@angular/core';
import {FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse} from 'fakeoverflow-angular-services';
import {ButtonDirective} from 'primeng/button';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-post-answer',
  imports: [
    ButtonDirective,
    DatePipe
  ],
  templateUrl: './post-answer.html',
  styleUrl: './post-answer.scss'
})
export class PostAnswer {
  public post = input.required<FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse>();
  protected readonly errorMessage = signal<string | null>(null);
}
