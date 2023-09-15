import { createBrowserRouter, RouteObject } from 'react-router-dom';
import App from '../layout/App';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import ActivityForm from '../../features/activities/form/ActivityForm';
import ActivityDetails from '../../features/activities/details/ActivityDetails';
import TestErrors from '../../features/errors/TestError';
import NotFound from '../../features/errors/NotFound';
import ServerError from '../../features/errors/ServerError';
import PageNotFound from '../../features/errors/PageNotFound';
import ProfilePage from '../../features/profile/ProfilePage';
import RequireAuth from './RequireAuth';
import RegisterSuccess from '../../features/users/RegisterSuccess';

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            {element: <RequireAuth />, children: [
                {path: 'activities', element: <ActivityDashboard />},
                {path: 'activities/:id', element: <ActivityDetails />},
                {path: 'create-activity', element: <ActivityForm key="create" />},
                {path: 'edit-activity/:id', element: <ActivityForm key="edit" />},
                {path: 'profiles/:username', element: <ProfilePage />},
                {path: 'errors', element: <TestErrors />},
            ]},
            {path: 'not-found', element: <NotFound />},
            {path: 'server-error', element: <ServerError />},
            {path: 'account/registerSuccess', element: <RegisterSuccess />},
            {path: '*', element: <PageNotFound />}
        ]
    }
];

const router = createBrowserRouter(routes);

export default router;
