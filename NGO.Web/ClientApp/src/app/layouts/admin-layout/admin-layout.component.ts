import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { UserState } from 'src/app/store/user/user.model';
import { selectCurrentUser } from 'src/app/store/user/user.selectors';
// Adjust path accordingly

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
})
export class AdminLayoutComponent implements OnInit {
  user$!: Observable<UserState | null>;
  constructor(private store: Store<UserState>) {}

  ngOnInit() {
    this.user$ = this.store.select(selectCurrentUser);

    // Subscribe to the observable to log the user state
    this.user$.subscribe(() => {});
  }
}
