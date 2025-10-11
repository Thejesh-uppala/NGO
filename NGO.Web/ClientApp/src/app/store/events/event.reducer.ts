import { createReducer, on } from '@ngrx/store';
import {
  addRegisteredEvent,
  eventClearOnlogout,
  loadEvents,
  loadEventsFailure,
  loadRegisteredEventsSuccess,
} from './event.actions';
import { initialRegisteredEventState } from './event.state';

export const eventReducer = createReducer(
  initialRegisteredEventState,
  on(loadEvents, state => ({
    ...state,
    error: null,
  })),
  on(loadRegisteredEventsSuccess, (state, { registeredEventsList }) => ({
    ...state,
    registeredEventsList: registeredEventsList,
    error: null,
  })),
  on(loadEventsFailure, (state, { error }) => ({
    ...state,
    error,
  })),
  on(eventClearOnlogout, () => {
    localStorage.clear();
    return initialRegisteredEventState;
  }),
  on(addRegisteredEvent, (state, { eventId }) => ({
    ...state,
    registeredEventsList: [...state.registeredEventsList, eventId], // Directly push eventId as a number
  }))
);
