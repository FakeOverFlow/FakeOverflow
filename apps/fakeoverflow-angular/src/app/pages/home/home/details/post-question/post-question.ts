import {Component, input} from '@angular/core';
import {FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse} from 'fakeoverflow-angular-services';
import {ButtonDirective} from 'primeng/button';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-post-question',
  imports: [
    ButtonDirective,
    DatePipe
  ],
  templateUrl: './post-question.html',
  styleUrl: './post-question.scss'
})
export class PostQuestion {
  public post = input.required<FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse>();
}
