import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '@shared/navbar/navbar';
import { CommunityStats } from '@shared/community-stats/community-stats';
import { TrendingTags } from '@shared/trending-tags/trending-tags';

@Component({
  selector: 'app-view-question',
  standalone: true,
  imports: [CommonModule, RouterLink, Navbar, CommunityStats, TrendingTags],
  templateUrl: './view-question.html',
  styleUrls: []
})
export class ViewQuestion {

}
