import { EventState, RegisteredEventState } from './event.model';

export const initialRegisteredEventState: RegisteredEventState = {
  registeredEventsList: [], // Start with an empty list
  error: null,
};

// Initial state for the event feature
export const initialEventState: EventState = {
  EventsList: [],
  error: null,
};
