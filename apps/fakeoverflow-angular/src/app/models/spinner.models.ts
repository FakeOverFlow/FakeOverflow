import {Spinner} from '@services/spinner';
import { v7 as uuidv7 } from 'uuid';

/**
 * Represents an interface for a page spinner component.
 * This component is typically used to indicate loading or processing states.
 */
export interface IPageSpinner {
  /**
   * A unique identifier represented as a string.
   * Typically used to differentiate between objects, entities, or records.
   */
  id: string;

  /**
   * An optional string variable that can hold textual data.
   * This variable may be undefined if no value is assigned.
   */
  text?: string;

  /**
   * A boolean variable that indicates whether the spinner should be displayed as a blur effect.
   * This variable is typically used to indicate that the spinner is currently in a loading state.
   */
  blur?: boolean;

  /**
   * Releases resources or performs necessary cleanup tasks.
   * This method should be called when the related functionality or processing is no longer required.
   *
   * @return {void} This method does not return a value.
   */
  release() : void
}

export class PageSpinner implements IPageSpinner {
    private readonly _id: string;
    private _text?: string | undefined;
    private readonly service: Spinner;
    private _blur?: boolean | undefined;

    public get id(){
      return this._id;
    }

    public get text(){
      return this._text;
    }

    public release() {
      this.service.remove(this._id);
    }

    public get blur(){
      return this._blur ?? false;
    }

    public set blur(value: boolean | undefined) {
      if(value === undefined) {
        value = false;
      }

      this._blur = value;
    }

    public set text(value: string | undefined) {
      this._text = value;
    }

    constructor(service: Spinner, text?: string) {
        this._id = uuidv7();
        this._text = text;
        this.service = service;
    }
}
