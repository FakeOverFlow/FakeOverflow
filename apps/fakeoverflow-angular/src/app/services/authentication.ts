import {inject, Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {Credentials, Identity} from '@models/identity.models';
import {Optional} from '@utils/types';
import {ConfigKeys} from '@constants/config-keys.enums';
import {Router} from '@angular/router';
import {HotToastService} from '@ngxpert/hot-toast';
import {
  FakeoverFlowBackendHttpApiFeaturesAuthLoginLoginResponse,
  FakeoverFlowBackendHttpApiFeaturesMeGetGetMeResponse, MeService
} from "fakeoverflow-angular-services";
import {HttpClient} from '@angular/common/http';
import {environment} from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class Authentication {

  private readonly loggedInIdentitySubject = new BehaviorSubject<Optional<Identity>>(undefined);
  private readonly router = inject(Router)
  private readonly toastService= inject(HotToastService)
  private readonly httpClient = inject(HttpClient)

  public initAuthentication() {
    console.trace('Initializing authenticated identity...');
    const identityRaw = localStorage.getItem(ConfigKeys.IDENTITY);
    if(!identityRaw) {
      console.trace('No identity found in local storage.');
      return;
    }

    const identity : Identity = JSON.parse(identityRaw);
    this.loggedInIdentitySubject.next(identity);
    console.info("Identity initialized: ", identity.id)

    this.getIdentity(identity.secrets.accessToken)
      .subscribe({
        next: (res) => {
          if(!this.isAuthenticated)
            return;

          this.setIdentity({
            identity: res,
            secrets: identity.secrets
          }, {
            redirectAfterLogin: false,
          })
        },
        error: (err) => {
          console.error("Error loading identity", err);
          this.logout({
            redirectToLogout: true,
            toastMessage: 'Failed to load identity. Please log in again.'
          });
        }
      })
  }

  public getIdentity(accessToken?: string) : Observable<FakeoverFlowBackendHttpApiFeaturesMeGetGetMeResponse>{
    console.trace('Loading/Refreshing identity...');
    return this.httpClient.get<FakeoverFlowBackendHttpApiFeaturesMeGetGetMeResponse>(`${environment.apiBaseUrl}/me`, {
      headers: {
        Authorization: `Bearer ${accessToken}`
      }
    })
  }

  /**
   * Retrieves the current identity as a value from the logged-in identity subject.
   *
   * @return {Optional<Identity>} The current logged-in identity encapsulated in an Optional, or an empty Optional if no identity is logged in.
   */
  public get currentIdentityAsValue(): Optional<Identity> {
    return this.loggedInIdentitySubject.value;
  }

  /**
   * Checks if the user is authenticated based on the existence of a valid identity.
   *
   * @return {boolean} True if the current identity is valid and the user is authenticated, false otherwise.
   */
  public get isAuthenticated(): boolean {
    return !!this.currentIdentityAsValue;
  }

  /**
   * Retrieves an observable of the current logged-in identity.
   *
   * @return {Observable<Optional<Identity>>} An observable emitting the optional current identity of the user.
   */
  public get currentIdentity(): Observable<Optional<Identity>> {
    return this.loggedInIdentitySubject.asObservable();
  }


  /**
   * Retrieves the current identity ID or throws an error if the identity or its ID is not available.
   *
   * @return {string} The ID of the current identity.
   * @throws {Error} If no identity is found or the identity ID is missing.
   */
  public get currentIdentityIdOrThrow(): string {
    const identity = this.currentIdentityAsValue;
    if(!identity) {
      throw new Error('No identity found.');
    }

    if(!identity.id) {
      throw new Error('Identity ID is missing.');
    }

    return identity.id;
  }

  /**
   * Logs the user out by clearing the stored identity, notifying subscribers,
   * and optionally redirecting to the login page and displaying a toast message.
   *
   * @param {Object} [options] Optional parameters for the logout operation.
   * @param {boolean} [options.redirectToLogout=true] Whether to redirect the user to the login page after logout.
   * @param {string} [options.toastMessage] A message to display as a toast notification after logging out.
   * @return {void} This method does not return any value.
   */
  public logout(options?: {
    redirectToLogout?: boolean,
    toastMessage?: string
  }) : void {
    localStorage.removeItem(ConfigKeys.IDENTITY);
    localStorage.removeItem(ConfigKeys.FORWARD_ROUTE_PATH);

    this.loggedInIdentitySubject.next(undefined);
    const redirectToLogout = options?.redirectToLogout ?? true;
    if(redirectToLogout) {
      this.router.navigate(['/auth/login'])
        .catch(console.error);
    }

    if(options?.toastMessage) {
      this.toastService.info(options.toastMessage)
    }
  }

  /**
   * Sets the user's identity after authentication and performs optional redirection.
   *
   * @param {Object} res Object containing the user's identity and authentication secrets.
   * @param {FakeoverFlowBackendHttpApiFeaturesMeGetGetMeResponse} res.identity User's identity data.
   * @param {FakeoverFlowBackendHttpApiFeaturesAuthLoginLoginResponse} res.secrets User's authentication secrets.
   * @param {Object} [options] Optional settings for redirection.
   * @param {boolean} [options.redirectAfterLogin=true] Whether to navigate to the specified route after setting the identity. Defaults to true.
   * @param {'forward-route' | string} [options.redirectTo='home'] Route to navigate to. Defaults to 'home' or a forward route if available.
   * @return {void} This method does not return a value.
   */
  public setIdentity(res: {
    identity: FakeoverFlowBackendHttpApiFeaturesMeGetGetMeResponse,
    secrets: FakeoverFlowBackendHttpApiFeaturesAuthLoginLoginResponse
  }, options? : {
    redirectAfterLogin?: boolean,
    redirectTo?: 'forward-route' | string
  }): void {
    this.logout({
      redirectToLogout: false
    });

    const authModel : Identity = {
      id: res.identity.id!,
      name: res.identity.userName!,
      avatarUrl: res.identity.profilePicture,
      secrets: {
        accessToken: res.secrets.accessToken!,
        refreshToken: res.secrets.refreshToken!
      }
    }
    localStorage.setItem(ConfigKeys.IDENTITY, JSON.stringify(authModel));
    this.loggedInIdentitySubject.next(authModel);

    const redirectAfterLogin = options?.redirectAfterLogin ?? true;
    // Get Forward Route Path - Forward route path is something that needs to be routed after login
    const redirectTo = this.getForwardRoutePath() ?? options?.redirectTo ?? 'home';
    if(redirectAfterLogin){
      this.router.navigate([redirectTo])
        .then(() => {
          this.clearForwardRoutePath();
        })
        .catch(err => console.error("Error navigating to dashboard page", err));
    }
  }

  /**
   * Sets the forward route path to be navigated after login.
   * The path is stored in localStorage under a predefined configuration key.
   *
   * @param {string} path - The route path to be set for forwarding after login.
   * @return {void} Does not return a value.
   */
  public setForwardRoutePath(path: string): void {
    localStorage.setItem(ConfigKeys.FORWARD_ROUTE_PATH, path);
  }

  /**
   * Clears the stored forward route path from local storage and logs a trace message.
   *
   * @return {void} This method does not return a value.
   */
  public clearForwardRoutePath(): void {
    localStorage.removeItem(ConfigKeys.FORWARD_ROUTE_PATH);
    console.trace("Cleared forward route path");
  }

  /**
   * Retrieves the forward route path from local storage.
   *
   * @return {string | undefined} The forward route path if it exists in local storage, otherwise undefined.
   */
  private getForwardRoutePath() : string | undefined {
    return localStorage.getItem(ConfigKeys.FORWARD_ROUTE_PATH) ?? undefined;
  }

  /**
   * Updates the stored secrets for the current user identity.
   *
   * @param {Credentials} secrets - The new secrets to be updated for the current user's identity.
   * @return {void} Does not return any value.
   */
  updateSecrets(secrets: Credentials) {
    const value = this.currentIdentityAsValue;
    if(!value){
      this.logout({redirectToLogout: false, toastMessage: 'Failed to update secrets. Please log in again.'})
      return;
    }

    value.secrets = secrets;
    localStorage.setItem(ConfigKeys.IDENTITY, JSON.stringify(value));
  }
}
