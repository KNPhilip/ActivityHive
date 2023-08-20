import axios, { AxiosError, AxiosResponse } from 'axios';
import { Activity, ActivityFormValues } from '../models/activity';
import { toast } from 'react-toastify';
import router from '../router/Routes';
import { store } from '../stores/store';
import { User, UserFormValues } from '../models/user';
import { Photo, Profile, UserActivity } from '../models/profile';
import { PaginatedResult } from '../models/pagination';

const sleep = (delay: number) => {
    return new Promise((resolve) => {
        setTimeout(resolve, delay);
    })
}

axios.interceptors.request.use(config => {
    const token = store.commonStore.token;
    if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;
    return config;
})

axios.defaults.baseURL = process.env.REACT_APP_API_URL;
axios.interceptors.response.use(async response => {
    const pagination = response.headers['pagination'];
    if (process.env.NODE_ENV === 'development') await sleep(500);
    if (pagination) {
        response.data = new PaginatedResult(response.data, JSON.parse(pagination))
        return response as AxiosResponse<PaginatedResult<any>>
    }
    return response;
}, (error: AxiosError) => {
    const {data, status} = error.response as AxiosResponse;
    switch(status) {
        case 400:
            if (data.errors) {
                const modalStateErrors = [];
                for (const key in data.errors) {
                    if (data.errors[key]) {
                        modalStateErrors.push(data.errors[key])
                    }
                }
                throw modalStateErrors.flat();
            } else {
                toast.error(data);
            }
            break;
        case 401:
            toast.error('Unauthorised');
            break;
        case 403:
            toast.error('Forbidden');
            break;
        case 404:
            router.navigate('/not-found');
            break;
        case 500:
            store.commonStore.setServerError(data);
            router.navigate('/server-error');
            break;
    }
    return Promise.reject(error);
});

const responseBody = <T> (response: AxiosResponse<T>) => response.data;

const requests = {
    get: <T>(url: string) => axios.get<T>(url).then(responseBody),
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

const Activities = {
    list: (params: URLSearchParams) => axios.get<PaginatedResult<Activity[]>>('/activities', {params})
        .then(responseBody),
    details: (id: string) => requests.get<Activity>(`/activities/${id}`),
    create: (activity: ActivityFormValues) => requests.post<void>(`activities`, activity),
    update: (activity: ActivityFormValues) => requests.put<void>(`/activities/${activity.id}`, activity),
    delete: (id: string) => requests.del<void>(`activities/${id}`),
    attend: (id: string) => requests.post<void>(`/activities/${id}/attend`, {})
}

const Profiles = {
    get: (username: string) => requests.get<Profile>(`/profile/${username}`),
    uploadPhoto: (file: Blob) => {
        let formData = new FormData();
        formData.append('File', file)
        return axios.post<Photo>('photo', formData, {
            headers: {'Content-Type': 'multipart/form-data'}
        })
    },
    setMainPhoto: (id: string) => requests.post(`/photo/${id}/setMain`, {}),
    deletePhoto: (id: string) => requests.del(`/photo/${id}`),
    updateProfile: (profile: Partial<Profile>) => requests.put(`/profile`, profile),
    updateFollowing: (username: string) => requests.post(`/follow/${username}`, {}),
    listFollowings: (username: string, predicate: string) =>
        requests.get<Profile[]>(`/follow/${username}?predicate=${predicate}`),
    listActivities: (username: string, predicate: string) =>
        requests.get<UserActivity[]>(`/profile/${username}/activities?predicate=${predicate}`)
}

const Auth = {
    current: () => requests.get<User>('/auth'),
    login: (user: UserFormValues) => requests.post<User>('auth/login', user),
    register: (user: UserFormValues) => requests.post<User>('auth/register', user)
}

const agent = {
    Activities,
    Profiles,
    Auth
}

export default agent;
