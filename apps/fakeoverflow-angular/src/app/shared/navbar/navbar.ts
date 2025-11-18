import {Component, inject, signal, HostListener} from '@angular/core';
import {Authentication} from '@services/authentication';
import {AsyncPipe} from '@angular/common';
import {Common} from '@services/common';
import {Router, RouterLink} from '@angular/router';
import {AvvvatarsComponent} from '@ngxpert/avvvatars';
import {Logo} from '@shared/logo/logo';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    AsyncPipe,
    AvvvatarsComponent,
    Logo, RouterLink
  ],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar {
  isMobileMenuOpen = false;
  isProfileMenuOpen = false;
  private readonly authentication = inject(Authentication);
  private readonly commonService = inject(Common);
  private readonly router = inject(Router);

  protected readonly darkMode$ = this.commonService.darkMode$;
  protected readonly identity$ = this.authentication.currentIdentity;

  ngOnInit(): void {

  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.relative')) {
      this.isProfileMenuOpen = false;
    }
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  toggleProfileMenu(): void {
    this.isProfileMenuOpen = !this.isProfileMenuOpen;
  }

  toggleTheme(): void {
    this.commonService.toggleDarkMode();
  }

  logout(): void {
    this.isProfileMenuOpen = false;
    this.authentication.logout({
      redirectToLogout: true,
      toastMessage: 'You have been logged out successfully'
    });
  }

  routeToLoginPage() {
    this.setForwardRoute()
    this.router.navigate(['/auth/login'])
      .catch(console.error)
  }

  routeToSignupPage() {
    this.setForwardRoute()
    this.router.navigate(['/auth/sign-up'])
      .catch(console.error)
  }

  private setForwardRoute(){
    const pathname = this.router.url;

    this.authentication.setForwardRoutePath(pathname);
    console.trace(`Routing to ${pathname} after login`)
  }
}
