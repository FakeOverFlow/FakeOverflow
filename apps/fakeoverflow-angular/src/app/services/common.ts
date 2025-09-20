import {DestroyRef, inject, Injectable} from '@angular/core';
import {uniqueNamesGenerator, adjectives, colors, animals, starWars, names} from 'unique-names-generator';
import {BehaviorSubject} from 'rxjs';
import {ConfigKeys} from '@constants/config-keys.enums';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root'
})
export class Common {

  private readonly destroyRef = inject(DestroyRef);

  private readonly darkModeSubject = new BehaviorSubject<boolean>(
    localStorage.getItem(ConfigKeys.DARK_MODE_PERSISTENCE) === 'true'
  );

  public get darkMode$() {
    return this.darkModeSubject.asObservable();
  }

  constructor() {
    this.darkMode$
      .pipe(
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(enabled => {
      document.documentElement.classList.toggle('fof-dark-mode-toggle', enabled);
    });
  }

  public get darkMode(): boolean {
    return this.darkModeSubject.value;
  }

  public toggleDarkMode(): void {
    this.setDarkMode(!this.darkModeSubject.value);
  }

  public setDarkMode(enabled: boolean): void {
    this.darkModeSubject.next(enabled);
    localStorage.setItem(ConfigKeys.DARK_MODE_PERSISTENCE, String(enabled));

    if (enabled) {
      document.documentElement.classList.add('fof-dark-mode-toggle');
    } else {
      document.documentElement.classList.remove('fof-dark-mode-toggle');
    }
  }

  public generateRandomUserName(options?: {
    length?: number;
  }) : string[] {
    const generatedName = uniqueNamesGenerator({
      length: options?.length ?? 2,
      dictionaries: [
        adjectives,
        colors,
        animals,
        starWars,
        names
      ],
      separator: '-'
    });

    return generatedName.split('-');
  }

}
