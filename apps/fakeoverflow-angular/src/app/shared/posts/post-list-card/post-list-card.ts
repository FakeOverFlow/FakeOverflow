
import {Component, inject, input, OnInit} from '@angular/core';
import {
  FakeoverFlowBackendHttpApiFeaturesPostsListPostsListPostsPostSummary,
  PostService
} from '../../../../../../../packages/fakeoverflow-angular-services';
import {RelativeTimePipe} from '@pipes/relative-time-pipe';
import {UserNameExtractorPipe} from '@pipes/user-name-extractor-pipe';
import {Router} from '@angular/router';

@Component({
  selector: 'app-post-list-card',
  imports: [
    RelativeTimePipe,
    UserNameExtractorPipe
  ],
  templateUrl: './post-list-card.html',
  styleUrl: './post-list-card.scss'
})
export class PostListCard implements OnInit{

  readonly post = input.required<FakeoverFlowBackendHttpApiFeaturesPostsListPostsListPostsPostSummary>();
  private readonly router = inject(Router);

  ngOnInit(): void {

  }

  protected routeToPost() {
    this.router.navigate(['/', 'post', this.post().postId!.toLowerCase()])
      .catch(console.error);
  }
}
