import { createBrowserRouter, RouteObject } from 'react-router-dom';
import App from '../layout/App';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import ActivityForm from '../../features/activities/form/ActivityForm';
import ActivityDetails from '../../features/activities/details/ActivityDetails';

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            {path: 'activities', element: <ActivityDashboard />},
            {path: 'activities/:id', element: <ActivityDetails />},
            {path: 'create-activity', element: <ActivityForm key="create" />},
            {path: 'edit-activity/:id', element: <ActivityForm key="edit" />}
        ]
    }
];

const router = createBrowserRouter(routes);

export default router;
