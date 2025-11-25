import {Component, inject, input, output} from '@angular/core';
import {
  FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse,
  PostService
} from '../../../../../../../../packages/fakeoverflow-angular-services';
import {UserNameExtractorPipe} from '@pipes/user-name-extractor-pipe';
import {RelativeTimePipe} from '@pipes/relative-time-pipe';
import {isNegativeInteger} from '@utils/utils';
import {HotToastService} from '@ngxpert/hot-toast';
import {Authentication} from '@services/authentication';

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
  public updatePost = output();

  protected readonly authenticationService = inject(Authentication);
  private readonly postService = inject(PostService);
  private readonly toastService = inject(HotToastService);
  protected readonly isNegativeInteger = isNegativeInteger;

  protected deleteVote() {
    this.postService.deleteVote(this.post().postId!, this.post().contentId!)
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
    this.postService.voteContent(this.post().postId!, this.post().contentId!, {
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
