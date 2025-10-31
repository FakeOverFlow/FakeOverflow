import { Component, OnInit , inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import {Navbar} from '@shared/navbar/navbar';
import { PostService } from '../../../../../../../../packages/fakeoverflow-angular-services/api/post.service';

@Component({
  selector: 'app-post',
  standalone: true,
  imports: [Navbar, CommonModule, FormsModule],
  templateUrl: './details.html',
  styleUrl: './details.scss'
})
export class DetailsComponent implements OnInit {
  private readonly postService = inject(PostService);
  private readonly route = inject(ActivatedRoute);
    post: any = null;
  loading = false;
  errorMessage = '';
  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.errorMessage = 'No post ID provided.';
      return;
    }
     this.loading = true;

     this.postService.getPostById(id).subscribe({
      next: (response) => {
        this.post = response;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading post:', err);
        this.errorMessage = 'Failed to load post.';
        this.loading = false;
      }
    });
  
  }
}

