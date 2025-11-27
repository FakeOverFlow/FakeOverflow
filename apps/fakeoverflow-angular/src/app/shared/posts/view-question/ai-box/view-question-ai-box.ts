import {Component, input} from '@angular/core';
import {
  FakeoverFlowBackendHttpApiFeaturesPostsContentsListContentsListContentsPostContent
} from 'fakeoverflow-angular-services';
import {MarkdownComponent} from 'ngx-markdown';

@Component({
  selector: 'app-view-question-ai-box',
  imports: [
    MarkdownComponent
  ],
  templateUrl: './view-question-ai-box.html',
  styleUrl: './view-question-ai-box.scss'
})
export class ViewQuestionAiBox {

  public answer = input.required<FakeoverFlowBackendHttpApiFeaturesPostsContentsListContentsListContentsPostContent>();
}
