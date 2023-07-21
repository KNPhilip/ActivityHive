import { Segment, Item } from 'semantic-ui-react';
import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';
import ActivityListItem from './ActivityListItem';

const ActivityList = () => {
    const { activityStore } = useStore();
    const { activitiesByDate: activities } = activityStore;

    return (
        <Segment>
            <Item.Group divided>
                {activities.map(activity => (
                    <ActivityListItem key={activity.id} activity={activity} />
                ))}
            </Item.Group>
        </Segment>
    );
}

export default observer(ActivityList);
