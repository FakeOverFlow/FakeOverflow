import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'userNameExtractor'
})
export class UserNameExtractorPipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): string {
    const name = value?.toString() ?? "";
    const split = name.split(" ")
    if(split.length <= 1){
      return name.at(0)?.toUpperCase() || 'UK';
    }

    return (split[0][0] + split[1][0])?.toUpperCase() || 'UK';
  }

}
