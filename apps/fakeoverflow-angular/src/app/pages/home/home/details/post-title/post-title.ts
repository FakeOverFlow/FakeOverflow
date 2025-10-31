import {Component, input} from '@angular/core';
import {FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse} from 'fakeoverflow-angular-services';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-post-title',
  imports: [
    DatePipe
  ],
  templateUrl: './post-title.html',
  styleUrl: './post-title.scss'
})
export class PostTitle {

  public post = input.required<FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse>();


}
