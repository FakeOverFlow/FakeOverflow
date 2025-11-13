import { Component, inject, OnInit } from '@angular/core';
import { Navbar } from '@shared/navbar/navbar';
import { RouterLink } from '@angular/router';
import { CommunityStats } from '@shared/community-stats/community-stats';
import { TrendingTags } from '@shared/trending-tags/trending-tags';
import { Authentication } from '@services/authentication';
import { AsyncPipe, CommonModule } from '@angular/common';
import { AvvvatarsComponent } from '@ngxpert/avvvatars';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    Navbar,
    RouterLink,
    CommunityStats,
    TrendingTags,
    AsyncPipe,
    AvvvatarsComponent
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class Profile implements OnInit {
  private readonly authentication = inject(Authentication);
  protected readonly identity$ = this.authentication.currentIdentity;

  // Profile data structure (placeholder)
  profileData = {
    name: 'John Doe',
    username: 'johndoe',
    bio: 'Full-stack developer passionate about building scalable web applications. Love solving complex problems and helping others in the community.',
    stats: {
      questions: 42,
      answers: 128
    }
  };

  ngOnInit(): void {
    // Initialize profile data
  }
}
