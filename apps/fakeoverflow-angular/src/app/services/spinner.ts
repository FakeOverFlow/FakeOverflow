import { Injectable } from '@angular/core';
import {IPageSpinner, PageSpinner} from '@models/spinner.models';
import {BehaviorSubject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Spinner {

  private readonly spinnerMap = new Map<string, IPageSpinner>;
  private readonly currentSpinnerSubject = new BehaviorSubject<IPageSpinner | undefined>(undefined);

  public get isAnySpinnerActive(){
    return this.spinnerMap.size > 0;
  }

  public get currentSpinnerValue(){
    return this.currentSpinnerSubject.value;
  }

  public get currentSpinner$(){
    return this.currentSpinnerSubject.asObservable();
  }

  public for(text?: string) : IPageSpinner {
    const spinner : IPageSpinner = new PageSpinner(this, text);
    this.spinnerMap.set(spinner.id, spinner);
    this.updateCurrentSpinner();
    return spinner;
  }

  public remove(id: string){
    this.spinnerMap.delete(id);
    this.updateCurrentSpinner();
  }

  public clear(){
    this.spinnerMap.clear();
    this.updateCurrentSpinner();
  }

  private updateCurrentSpinner(){
    const firstSpinner = this.spinnerMap.values().next().value;
    this.currentSpinnerSubject.next(firstSpinner);
  }
}
