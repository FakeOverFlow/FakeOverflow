
import {Component, inject, input, OnInit} from '@angular/core';
import {
  FakeoverFlowBackendHttpApiFeaturesPostsListPostsListPostsPostSummary,
  PostService
} from '../../../../../../../packages/fakeoverflow-angular-services';
import {RelativeTimePipe} from '@pipes/relative-time-pipe';
import {UserNameExtractorPipe} from '@pipes/user-name-extractor-pipe';

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

  ngOnInit(): void {

  }

}
