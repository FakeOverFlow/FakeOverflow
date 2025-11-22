import {Component, input} from '@angular/core';
import {RouterLink} from "@angular/router";
import {FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse} from 'fakeoverflow-angular-services';
import {RelativeTimePipe} from '@pipes/relative-time-pipe';

@Component({
  selector: 'app-view-question-title-bar',
  imports: [
    RouterLink,
    RelativeTimePipe
  ],
  templateUrl: './view-question-title-bar.html',
  styleUrl: './view-question-title-bar.scss'
})
export class ViewQuestionTitleBar {

  public post = input.required<FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse>();

}
