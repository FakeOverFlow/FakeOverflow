import {Component, inject, input, output} from '@angular/core';
import {
  FakeoverFlowBackendHttpApiFeaturesPostsContentsListContentsListContentsPostContent, PostService
} from '../../../../../../../../packages/fakeoverflow-angular-services';
import {RelativeTimePipe} from '@pipes/relative-time-pipe';
import {UserNameExtractorPipe} from '@pipes/user-name-extractor-pipe';
import {Authentication} from '@services/authentication';
import {HotToastService} from '@ngxpert/hot-toast';
import {isNegativeInteger} from '@utils/utils';

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

  public updatePost = output();

  protected readonly authenticationService = inject(Authentication);
  private readonly postService = inject(PostService);
  private readonly toastService = inject(HotToastService);
  protected readonly isNegativeInteger = isNegativeInteger;

  protected deleteVote() {
    this.postService.deleteVote(this.answer().postId!, this.answer().id!)
      .pipe(
        this.toastService.observe({
          success: "Removed your vote",
          error: "Something went wrong",
          loading: "Removing your vote"
        })
      )
      .subscribe(post => {
        this.updatePost.emit()
      })
  }

  protected upvote() {
    this.vote(true)
  }

  protected downvote() {

    this.vote(false)
  }

  private vote(upvote: boolean){
    this.postService.voteContent(this.answer().postId!, this.answer().id!, {
      isUpvote: upvote,
    })
      .pipe(
        this.toastService.observe({
          success: `${upvote ? 'Upvoted' : 'Downvoted'}`,
          error: "Something went wrong",
          loading: "Adding your feedback"
        })
      )
      .subscribe(post => {
        this.updatePost.emit()
      })
  }
}
