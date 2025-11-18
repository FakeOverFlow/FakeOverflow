import {Component, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormControl, FormGroup, FormsModule, NgForm, ReactiveFormsModule, Validators} from '@angular/forms';
import {Navbar} from '@shared/navbar/navbar';
import {PostService} from '../../../../../../../../packages/fakeoverflow-angular-services';
import {CommunityStats} from '@shared/community-stats/community-stats';
import {TrendingTags} from '@shared/trending-tags/trending-tags';
import {RouterLink} from '@angular/router';


@Component({
  selector: 'app-post',
  standalone: true,
  imports: [Navbar, CommonModule, FormsModule, ReactiveFormsModule, CommunityStats, TrendingTags, RouterLink],
  templateUrl: './post.html',
  styleUrl: './post.scss'
})
export class PostComponent {

  private readonly postService = inject(PostService)

  protected readonly formGroup = new FormGroup({
    title: new FormControl(null, [Validators.required]),
    content: new FormControl(null, [Validators.required]),
  })

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

          // Hide success message after 3 seconds
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
