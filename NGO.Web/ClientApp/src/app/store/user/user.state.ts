import { UserState } from './user.model';

export const initialState: UserState = {
  userId: 0,
  name: '',
  token: '',
  tokenExpiryDate: '',
  userRoles: [],
  organizations: [],
  error: null,
};
