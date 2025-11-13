import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {Navbar} from '@shared/navbar/navbar';
import { CommunityStats } from '@shared/community-stats/community-stats';
import { TrendingTags } from '@shared/trending-tags/trending-tags';

@Component({
  selector: 'app-post',
  standalone: true,
  imports: [Navbar, CommonModule, FormsModule, RouterLink, CommunityStats, TrendingTags],
  templateUrl: './post.html',
  styleUrl: './post.scss'
})
export class PostComponent {
  title = '';
  body = '';
  tags = '';
  loading = false;
  submitted = false;

  submit(form: NgForm) {
    if (form.invalid) return;
    this.loading = true;
    const payload = {
      title: this.title.trim(),
      body: this.body.trim(),
      tags: this.tags.split(',').map(t => t.trim()).filter(Boolean)
    };
    console.log('Post submit', payload);
    this.submitted = true;
    this.loading = false;
    form.resetForm();
  }
}
