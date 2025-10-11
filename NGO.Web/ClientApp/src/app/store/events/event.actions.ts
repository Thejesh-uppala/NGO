import { createAction, props } from '@ngrx/store';

// Action to load events
export const loadEvents = createAction('[Event] Load Events');

// Action for successful event fetch
export const loadRegisteredEventsSuccess = createAction(
  '[Event] Load Events Success',
  props<{ registeredEventsList: number[] }>() // Payload containing events
);

// Action for event fetch failure
export const loadEventsFailure = createAction(
  '[Event] Load Events Failure',
  props<{ error: string }>() // Payload containing error message
);

// Action to add a registered event (eventId is a number)
export const addRegisteredEvent = createAction('[Event] Add Registered Event', props<{ eventId: number }>());

// Action for logout
export const eventClearOnlogout = createAction('[Event] Logout');
