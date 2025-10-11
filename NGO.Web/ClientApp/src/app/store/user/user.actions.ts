import { createAction, props } from '@ngrx/store';
import { UserState } from './user.model';

// Action for successful login
export const loginSuccess = createAction(
  '[User] Login Success',
  props<{ user: UserState }>() // User payload
);

// Action for logout
export const logout = createAction('[User] Logout');

// Action for login failure
export const loginFailure = createAction(
  '[User] Login Failure',
  props<{ error: string }>() // Error message payload
);
