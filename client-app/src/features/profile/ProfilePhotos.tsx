import { observer } from "mobx-react-lite";
import { Card, Header, Tab, Image } from "semantic-ui-react";
import { Profile } from "../../app/models/profile";

interface Props {
    profile: Profile;
}

const ProfilePhotos = ({profile}: Props) => {
    return (
        <Tab.Pane>
            <Header icon="image" content="Photos" />
            <Card.Group itemsPerRow={5}>
                {profile.photos?.map(photo => (
                    <Card key={photo.id}>
                        <Image src={photo.url} />
                    </Card>
                ))}
            </Card.Group>
        </Tab.Pane>
    )
}

export default observer(ProfilePhotos);
