import {Component, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormControl, FormGroup, FormsModule, NgForm, ReactiveFormsModule, Validators} from '@angular/forms';
import {Navbar} from '@shared/navbar/navbar';
import {
  FakeoverFlowBackendHttpApiFeaturesPostsCreatePostsPostsRequest,
  PostService
} from '../../../../../../../../packages/fakeoverflow-angular-services';
import {CommunityStats} from '@shared/community-stats/community-stats';
import {TrendingTags} from '@shared/trending-tags/trending-tags';
import {Router, RouterLink} from '@angular/router';


@Component({
  selector: 'app-post',
  standalone: true,
  imports: [Navbar, CommonModule, FormsModule, ReactiveFormsModule, CommunityStats, TrendingTags, RouterLink],
  templateUrl: './post.html',
  styleUrl: './post.scss'
})
export class PostComponent {

  private readonly postService = inject(PostService)
  private readonly router = inject(Router)

  protected readonly formGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
    content: new FormControl('', [Validators.required]),
    tags: new FormControl([], [])
  })

  loading = false;
  submitted = false;

  submit() {
    if (this.formGroup.invalid) return;
    const request : FakeoverFlowBackendHttpApiFeaturesPostsCreatePostsPostsRequest = this.formGroup.value! as FakeoverFlowBackendHttpApiFeaturesPostsCreatePostsPostsRequest;
    this.loading = true;
    this.postService.createPost(request)
      .subscribe({
        next: (response) => {
          console.log(response);
          this.submitted = true;
          this.loading = false;
          this.formGroup.reset();
          this.router.navigate(['/post', response.id!])
            .catch(console.error)

          setTimeout(() => {
            this.submitted = false;
          }, 3000);
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }
      })
  }
}
