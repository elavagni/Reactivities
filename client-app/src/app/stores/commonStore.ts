import { makeAutoObservable, reaction } from 'mobx';
import { ServerError } from '../models/serverError';

export default class CommonStore {
  error: ServerError | null = null;
  token: string | null = window.localStorage.getItem('jwt');
  appLoaded = false;

  constructor() {
    makeAutoObservable(this);

    //this will be called when the value of the property token changes, after it has been initialized
    reaction(
      () => this.token,
      (token) => {
        if (token) {
          window.localStorage.setItem('jwt', token);
        } else {
          window.localStorage.removeItem('jwt');
        }
      },
    );
  }

  setServerError = (error: ServerError) => {
    this.error = error;
  };

  setToken = (token: string | null) => {
    //the mobx reaction will add the token to the localStorage
    this.token = token;
  };

  setAppLoaded = () => {
    this.appLoaded = true;
  };
}
