import {Component, inject, OnInit, signal} from '@angular/core';
import { Navbar } from '@shared/navbar/navbar';
import { RouterLink } from '@angular/router';
import { CommunityStats } from '@shared/community-stats/community-stats';
import { TrendingTags } from '@shared/trending-tags/trending-tags';

import {PostListCard} from '@shared/posts/post-list-card/post-list-card';

import {PostService, FakeoverFlowBackendHttpApiFeaturesPostsListPostsListPostsPostSummary} from '../../../../../../../packages/fakeoverflow-angular-services';

@Component({
  selector: 'app-home',
  imports: [
    Navbar, RouterLink, CommunityStats, TrendingTags, PostListCard
  ],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home implements OnInit{

  protected readonly postService = inject(PostService);
  protected posts: FakeoverFlowBackendHttpApiFeaturesPostsListPostsListPostsPostSummary[] = [];

  ngOnInit(): void {
        this.postService.listPosts()
          .subscribe({
            next: (response) => {
              this.posts = response.posts || [];
            },
            error: (err) => {
              console.error(err);
            }
          })
    }
}
