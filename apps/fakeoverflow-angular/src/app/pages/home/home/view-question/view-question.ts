import {Component, inject, OnInit, signal, WritableSignal} from '@angular/core';
import { CommonModule } from '@angular/common';
import {ActivatedRoute, RouterLink} from '@angular/router';
import { Navbar } from '@shared/navbar/navbar';
import { CommunityStats } from '@shared/community-stats/community-stats';
import { TrendingTags } from '@shared/trending-tags/trending-tags';
import {ViewQuestionTitleBar} from '@shared/posts/view-question/title-bar/view-question-title-bar';
import {ViewQuestionQuestionBox} from '@shared/posts/view-question/question-box/view-question-question-box';
import {ViewQuestionAiBox} from '@shared/posts/view-question/ai-box/view-question-ai-box';
import {ViewQuestionAnswerBox} from '@shared/posts/view-question/answer-box/view-question-answer-box';
import {ViewQuestionCreateAnswer} from '@shared/posts/view-question/create-answer/view-question-create-answer';
import {ViewQuestionLoginToAnswer} from '@shared/posts/view-question/login-to-answer/view-question-login-to-answer';
import {
  FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse,
  PostService,
  FakeoverFlowBackendHttpApiFeaturesPostsContentsListContentsListContentsPostContent
} from '../../../../../../../../packages/fakeoverflow-angular-services';
import {Spinner} from '@services/spinner';
import {HotToastService} from '@ngxpert/hot-toast';
import {Authentication} from '@services/authentication';

@Component({
  selector: 'app-view-question',
  standalone: true,
  imports: [CommonModule, RouterLink, Navbar, CommunityStats, TrendingTags, ViewQuestionTitleBar, ViewQuestionQuestionBox, ViewQuestionAiBox, ViewQuestionAnswerBox, ViewQuestionCreateAnswer, ViewQuestionLoginToAnswer],
  templateUrl: './view-question.html',
  styleUrls: []
})
export class ViewQuestion implements OnInit{
  private readonly postService = inject(PostService);
  private readonly route = inject(ActivatedRoute);
  protected readonly spinnerService = inject(Spinner);
  protected readonly authenticationService = inject(Authentication);
  protected readonly toastService = inject(HotToastService);

  protected post = signal<FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse | null>(null);
  protected loadingQuestion = signal(false);
  protected loadingAnswer = signal(false);
  protected answers = signal<FakeoverFlowBackendHttpApiFeaturesPostsContentsListContentsListContentsPostContent[]>([])

  private id: string | null = null

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (!this.id) {
      this.toastService.error("Failed to load the question");
      return;
    }

    this.loadQuestion(this.id)
  }

  loadQuestion(postId?: string): void {
    this.loadingQuestion.set(true);
    postId = postId ?? this.id!;
    const pageSpinner = this.spinnerService.for("Loading post");
    this.postService.getPostById(postId).subscribe({
      next: (response) => {
        pageSpinner.release()
        this.post.set(response);
        this.loadingQuestion.set(false);

        this.loadAnswers();
      },
      error: (err) => {
        console.error('Error loading post:', err);
        this.toastService.error("Failed to load the post");
        this.loadingQuestion.set(false);
        pageSpinner.release()
      }
    });
  }

  loadAnswers(){
    if(!this.post()) return;

    this.loadingAnswer.set(true);
    this.postService.listAnswers(this.post()?.postId!)
      .subscribe({
        next: (response) => {
          this.loadingAnswer.set(false);
          this.answers.set(response.answers || [])
        },
        error: (err) => {
          this.loadingAnswer.set(false);
          this.toastService.error("Failed to load the answers");
        }
      })
  }

  protected onPostUpdateEvent() {
    this.loadQuestion();
  }

  protected onAnswerUpdateEvent() {
    this.loadAnswers();
  }
}
