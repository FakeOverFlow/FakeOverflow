import { Pipe, PipeTransform } from '@angular/core';
import { formatDistanceToNow } from 'date-fns';

@Pipe({
  name: 'relativeTime',
  pure: true
})
export class RelativeTimePipe implements PipeTransform {
  transform(
    value: Date | string | number | null | undefined,
    withSuffix: boolean = true
  ): string {
    if (!value) return '';

    try {
      const date = value instanceof Date ? value : new Date(value);
      return formatDistanceToNow(date, { addSuffix: withSuffix });
    } catch {
      return '';
    }
  }
}
