import { createFeatureSelector, createSelector } from '@ngrx/store';
import { RegisteredEventState } from './event.model';

export const selectEventState = createFeatureSelector<RegisteredEventState>('event');

export const selectRegisteredEvents = createSelector(
  selectEventState,
  (state: RegisteredEventState) => state?.registeredEventsList
);

export const selectEventError = createSelector(selectEventState, (state: RegisteredEventState) => state.error);
