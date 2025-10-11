import { createReducer, on } from '@ngrx/store';
import { loginSuccess, logout, loginFailure } from './user.actions';
import { initialState } from './user.state';

export const userReducer = createReducer(
  initialState,
  on(loginSuccess, (state, { user }) => ({
    ...state,
    ...user,
    error: null,
    isAuthenticated: true,
    persistedAt: new Date().toISOString(),
  })),
  on(logout, () => {
    // Clear localStorage on logout
    localStorage.clear();
    return initialState;
  }),
  on(loginFailure, (state, { error }) => ({
    ...state,
    user: null,
    isAuthenticated: false,
    error,
  }))
);
