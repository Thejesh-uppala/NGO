import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';

@Injectable()
export class LoaderService {
  private showCount = 0;
  private skipLoader = false;
  public loader$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  toggleLoader() {
    this.loader$.next(this.loader$.getValue());
  }
  setLoader(isLoader: boolean) {
    if (isLoader) {
      this.showCount++;
    } else if (this.showCount > 0) {
      this.showCount--;
    }
    if (isLoader || this.showCount == 0) {
      this.loader$.next(isLoader);
    }
  }

  setSkipLoader(skipLoader: boolean): void {
    this.skipLoader = skipLoader;
  }

  getSkipLoader(): boolean {
    return this.skipLoader;
  }
}
