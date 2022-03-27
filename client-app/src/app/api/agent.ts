import axios, { AxiosError, AxiosResponse } from 'axios';
import { toast } from 'react-toastify';
import { history } from '../..';
import { Activity, ActivityFormValues } from '../models/activity';
import { PaginatedResult } from '../models/pagination';
import { Photo, Profile, UserActivity } from '../models/profile';
import { User, UserFormValues } from '../models/user';
import { store } from '../stores/store';

const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay);
  });
};

axios.defaults.baseURL = 'http://localhost:5000/api';

axios.interceptors.request.use((config) => {
  const token = store.commonStore.token;
  if (token) config.headers!.Authorization = `Bearer ${token}`;
  return config;
});

axios.interceptors.response.use(
  async (response) => {
    await sleep(1000);
    const pagination = response.headers['pagination'];
    //If there is a header for pagination, return the data as a PaginatedResult
    if (pagination) {
      response.data = new PaginatedResult(response.data, JSON.parse(pagination));
      return response as AxiosResponse<PaginatedResult<any>>;
    }
    //If there isn't a header for pagination, it is a normal response, just return it as is
    return response;
  },
  (error: AxiosError) => {
    const { data, status, config } = error.response!;
    switch (status) {
      case 400:
        if (typeof data === 'string') {
          toast.error(data);
        }

        if (config.method === 'get' && data.errors.hasOwnProperty('id')) {
          history.push('/not-found');
        }

        if (data.errors) {
          const modalStateErrors = [];
          for (const key in data.errors) {
            if (data.errors[key]) {
              modalStateErrors.push(data.errors[key]);
            }
          }
          throw modalStateErrors.flat();
        }
        break;
      case 401:
        toast.error('unauthorized');
        break;
      case 404:
        history.push('/not-found');
        break;
      case 500:
        store.commonStore.setServerError(data);
        history.push('/server-error');
        break;
    }
  },
);

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
  put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
  del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
};

const Activities = {
  //to add query string we can use the syntax ?= for example /activities?=
  //but we are going to send a configuration object instead
  list: (params: URLSearchParams) =>
    axios.get<PaginatedResult<Activity[]>>('/activities', { params }).then(responseBody),
  details: (id: string) => requests.get<Activity>(`/activities/${id}`),
  create: (activity: ActivityFormValues) => requests.post<void>('activities', activity),
  update: (activity: ActivityFormValues) =>
    requests.put<void>(`activities/${activity.id}`, activity),
  delete: (id: string) => requests.del<void>(`activities/${id}`),
  attend: (id: string) => requests.post<void>(`/activities/${id}/attend`, {}),
};

const Account = {
  current: () => requests.get<User>('/account'),
  login: (user: UserFormValues) => requests.post<User>('/account/login', user),
  register: (user: UserFormValues) => requests.post<User>('/account/register', user),
};

const Profiles = {
  get: (userName: string) => requests.get<Profile>(`/profiles/${userName}`),
  uploadPhoto: (file: Blob) => {
    let formData = new FormData();
    //property needs to match the name of the property in the backend
    formData.append('File', file);
    return axios.post<Photo>('photos', formData, {
      headers: { 'Content-type': 'multipart/form-data' },
    });
  },
  setMainPhoto: (id: string) => requests.post<Profile>(`/photos/${id}/setMain`, {}),
  deletePhoto: (id: string) => requests.del(`/photos/${id}`),
  updateFollowing: (userName: string) => requests.post(`/follow/${userName}`, {}),
  listFollowings: (userName: string, predicate: string) =>
    requests.get<Profile[]>(`/follow/${userName}?predicate=${predicate}`),
  listActivities: (userName: string, predicate: string) =>
    requests.get<UserActivity[]>(`/profiles/${userName}/activities?predicate=${predicate}`),
};

const agent = {
  Activities,
  Account,
  Profiles,
};

export default agent;
