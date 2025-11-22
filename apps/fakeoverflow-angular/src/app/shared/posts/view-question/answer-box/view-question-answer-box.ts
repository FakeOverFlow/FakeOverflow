import {Component, input} from '@angular/core';
import {
  FakeoverFlowBackendHttpApiFeaturesPostsContentsListContentsListContentsPostContent
} from 'fakeoverflow-angular-services';
import {RelativeTimePipe} from '@pipes/relative-time-pipe';
import {UserNameExtractorPipe} from '@pipes/user-name-extractor-pipe';

@Component({
  selector: 'app-view-question-answer-box',
  imports: [
    RelativeTimePipe,
    UserNameExtractorPipe
  ],
  templateUrl: './view-question-answer-box.html',
  styleUrl: './view-question-answer-box.scss'
})
export class ViewQuestionAnswerBox {

  public answer = input.required<FakeoverFlowBackendHttpApiFeaturesPostsContentsListContentsListContentsPostContent>();
  public isLast = input<boolean>(false);
}
