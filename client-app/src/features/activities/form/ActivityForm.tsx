import { useState, useEffect } from 'react';
import { Button, Segment } from 'semantic-ui-react';
import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';
import { Link, useNavigate, useParams } from 'react-router-dom';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import {v4 as uuid} from 'uuid';
import { Formik, Form, Field } from 'formik';

const ActivityForm = () => {

    const { activityStore } = useStore();
    const { createActivity, updateActivity, loading, loadActivity, loadingInitial } = activityStore;
    const { id } = useParams();
    const navigate = useNavigate();

    const [activity, setActivity] = useState({
        id: '',
        title: '',
        category: '',
        description: '',
        date: '',
        city: '',
        venue: ''
    });

    useEffect(() => {
        if (id) loadActivity(id).then(activity => setActivity(activity!))
    }, [id, loadActivity]);

    function handleSubmit() {
        if (!activity.id) {
            activity.id = uuid();
            createActivity(activity).then(() => navigate(`/activities/${activity.id}`));
        } 
        else updateActivity(activity).then(() => navigate(`/activities/${activity.id}`));
    }

    if (loadingInitial) return <LoadingComponent content='Loading activity...' />

    return (
        <Segment clearing>
            <Formik enableReinitialize initialValues={activity} onSubmit={values => console.log(values)}>
                {({ handleSubmit }) => (
                    <Form className="ui form" onSubmit={handleSubmit} autoComplete="off" >
                        <Field placeholder="Title" name="title" />
                        <Field placeholder="Description" name="description" />
                        <Field placeholder="Category" name="category" />
                        <Field type="date" placeholder="Date" name="date" />
                        <Field placeholder="City" name="city" />
                        <Field placeholder="Venue" name="venue" />
                        <Button floated="right" positive type="submit" content="Submit" />
                        <Button as={Link} to="/activities" disabled={loading} floated="right" type="button" content="Cancel" />
                    </Form>
                )}
            </Formik>
        </Segment>
    );
}

export default observer(ActivityForm);
