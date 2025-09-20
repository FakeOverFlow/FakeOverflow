import {Component, inject, signal} from '@angular/core';
import {Authentication} from '@services/authentication';
import {AsyncPipe} from '@angular/common';
import {Common} from '@services/common';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-navbar',
  imports: [
    AsyncPipe,
    RouterLink
  ],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar {
  isMobileMenuOpen = false;
  private readonly authentication = inject(Authentication);
  private readonly commonService = inject(Common);

  protected readonly darkMode$ = this.commonService.darkMode$;
  protected readonly identity$ = this.authentication.currentIdentity;

  ngOnInit(): void {

  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  toggleTheme(): void {
    this.commonService.toggleDarkMode();
  }
}
