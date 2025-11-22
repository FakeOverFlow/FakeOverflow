import {Component, input} from '@angular/core';
import {FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse} from 'fakeoverflow-angular-services';
import {UserNameExtractorPipe} from '@pipes/user-name-extractor-pipe';
import {RelativeTimePipe} from '@pipes/relative-time-pipe';

@Component({
  selector: 'app-view-question-question-box',
  imports: [
    UserNameExtractorPipe,
    RelativeTimePipe
  ],
  templateUrl: './view-question-question-box.html',
  styleUrl: './view-question-question-box.scss'
})
export class ViewQuestionQuestionBox {
  public post = input.required<FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse>();

}
