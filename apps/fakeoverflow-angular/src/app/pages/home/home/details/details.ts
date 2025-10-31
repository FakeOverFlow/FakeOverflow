import { Component, OnInit , inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import {Navbar} from '@shared/navbar/navbar';
import { PostService } from '../../../../../../../../packages/fakeoverflow-angular-services/api/post.service';
import {PostTitle} from '@pages/home/home/details/post-title/post-title';
import {PostQuestion} from '@pages/home/home/details/post-question/post-question';
import {PostAnswer} from '@pages/home/home/details/post-answer/post-answer';
import {FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse} from 'fakeoverflow-angular-services';
import {HotToastService} from '@ngxpert/hot-toast';
import {Spinner} from '@services/spinner';

@Component({
  selector: 'app-post',
  standalone: true,
  imports: [Navbar, CommonModule, FormsModule, PostTitle, PostQuestion, PostAnswer],
  templateUrl: './details.html',
  styleUrl: './details.scss'
})
export class DetailsComponent implements OnInit {
  private readonly postService = inject(PostService);
  private readonly route = inject(ActivatedRoute);
  protected readonly spinnerService = inject(Spinner);

  protected post: FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse | null = null;
  loading = false;
  errorMessage = '';
  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.errorMessage = 'No post ID provided.';
      return;
    }
     this.loading = true;

    const pageSpinner = this.spinnerService.for("Loading post");
     this.postService.getPostById(id).subscribe({
      next: (response) => {
        pageSpinner.release()
        this.post = response;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading post:', err);
        this.errorMessage = 'Failed to load post.';
        this.loading = false;
        pageSpinner.release()
      }
    });

  }
}

