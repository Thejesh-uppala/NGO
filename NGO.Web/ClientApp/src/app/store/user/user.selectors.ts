import { createFeatureSelector, createSelector } from '@ngrx/store';
import { UserState } from './user.model';

// Select the user feature state from the root state
export const selectUserState = createFeatureSelector<UserState>('user');

// Selector to get the current user
export const selectCurrentUser = createSelector(
  selectUserState,
  (state: UserState) => state // This returns the entire user state
);

// Selector to get user roles
export const selectUserRoles = createSelector(selectUserState, (state: UserState) => state.userRoles);

// Selector to get the user's organizations
export const selectUserOrganizations = createSelector(selectUserState, (state: UserState) => state.organizations);

// Selector to get user error (if any)
export const selectUserError = createSelector(selectUserState, (state: UserState) => state.error);

// Selector to get user token
export const selectUserToken = createSelector(selectUserState, (state: UserState) => state.token);
