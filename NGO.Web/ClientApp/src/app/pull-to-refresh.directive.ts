import { Directive, HostListener, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Directive({
  selector: '[appPullToRefresh]'
})
export class PullToRefreshDirective {
  private touchStartY = 0;
  private threshold = 50;  // Minimum pull distance to trigger refresh

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  @HostListener('touchstart', ['$event'])
  onTouchStart(event: TouchEvent): void {
    if (isPlatformBrowser(this.platformId) && this.isSmallScreen()) {
      this.touchStartY = event.touches[0].clientY;
    }
  }

  @HostListener('touchend', ['$event'])
  onTouchEnd(event: TouchEvent): void {
    if (!isPlatformBrowser(this.platformId) || !this.isSmallScreen()) return;

    const touchEndY = event.changedTouches[0].clientY;
    if (window.scrollY === 0 && touchEndY - this.touchStartY > this.threshold) {
      window.location.reload();
    }
  }

  private isSmallScreen(): boolean {
    return window.innerWidth <= 768;  // Adjust for your app's breakpoint
  }
}
