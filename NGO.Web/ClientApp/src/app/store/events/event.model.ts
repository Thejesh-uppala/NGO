export interface Event {
  id: number;
  orgId: number;
  eventTitle: string;
  eventDesc: string;
  eventRequirements: string;
  eventPhoto: string;
  eventAttendenceTerms: string;
  eventContactEmail: string;
  eventContactPhone: string;
  additionalInfo: string;
  eventAccess: string;
  createdDate: string;
  eventDate: string;
  registrationLastDate: string;
  eventTimeUtc: string;
  eventLocation: string;
  eventCosts: any[]; // Adjust type if necessary
}

export interface EventState {
  EventsList: Event[];
  error: string | null;
}

export interface RegisteredEventList {
  id: number;
}

export interface RegisteredEventState {
  registeredEventsList: number[];
  error: string | null;
}
