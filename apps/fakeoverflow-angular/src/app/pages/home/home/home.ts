import { Component } from '@angular/core';
import { Navbar } from '@shared/navbar/navbar';
import { RouterLink } from '@angular/router';
import { CommunityStats } from '@shared/community-stats/community-stats';
import { TrendingTags } from '@shared/trending-tags/trending-tags';

@Component({
  selector: 'app-home',
  imports: [
    Navbar, RouterLink, CommunityStats, TrendingTags
  ],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {

}
