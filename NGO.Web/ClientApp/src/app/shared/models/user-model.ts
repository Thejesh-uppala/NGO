export interface User {
  firstName: string;
  lastName: string;
  email: string;
  yearOfBirth: number;
  selected: boolean;
}

export interface Spouse {
  firstName: string;
  lastName: string;
  email: string;
  yearOfBirth: number;
  selected: boolean;
}

export interface Child {
  firstName: string;
  lastName: string;
  yearOfBirth: number;
  selected: boolean;
}

export interface Guest {
  firstName: string;
  lastName: string;
  yearOfBirth: number;
  email: string;
  phone: string;
  type: string;
  selected: boolean;
}
