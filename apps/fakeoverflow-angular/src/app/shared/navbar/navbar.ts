import {Component, inject, signal} from '@angular/core';
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
  private readonly authentication = inject(Authentication);
  private readonly commonService = inject(Common);
  private readonly router = inject(Router);

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
