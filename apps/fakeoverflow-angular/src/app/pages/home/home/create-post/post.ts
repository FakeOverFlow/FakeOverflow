import {Component, inject} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {Navbar} from '@shared/navbar/navbar';
import {CommunityStats} from '@shared/community-stats/community-stats';
import {TrendingTags} from '@shared/trending-tags/trending-tags';
import {PostService} from '../../../../../../../../packages/fakeoverflow-angular-services';

@Component({
  selector: 'app-post',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    Navbar,
    CommunityStats,
    TrendingTags
  ],
  templateUrl: './post.html',
  styleUrl: './post.scss'
})
export class PostComponent {
  private readonly postService = inject(PostService);

  protected readonly formGroup = new FormGroup({
    title: new FormControl(null, [Validators.required]),
    content: new FormControl(null, [Validators.required]),
  });

  loading = false;
  submitted = false;

  submit() {
    if (this.formGroup.invalid) return;

    console.log(this.formGroup.value);
    this.loading = true;

    this.postService.createPost(this.formGroup.value as any)
      .subscribe({
        next: (response) => {
          console.log(response);
          this.submitted = true;
          this.loading = false;
          this.formGroup.reset();

          setTimeout(() => {
            this.submitted = false;
          }, 3000);
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }
      });
  }
}
