import { observer } from "mobx-react-lite";
import { Profile } from "../../app/models/profile";
import { Card, Icon, Image } from "semantic-ui-react";
import { Link } from "react-router-dom";

interface Props {
    profile: Profile;
}

const ProfileCard = ({profile}: Props) => {
    function truncate(text: string | undefined)
    {
        if (text) return text.length > 40 ? text.substring(0, 37) + '...' : text;
    }

    return (
        <Card as={Link} to={`/profiles/${profile.username}`}>
            <Image src={profile.image || '/assets/user.png'} />
            <Card.Content>
                <Card.Header>{profile.displayName}</Card.Header>
                <Card.Description>{truncate(profile.bio)}</Card.Description>
            </Card.Content>
            <Card.Content extra>
                <Icon name='user' />
                {profile.followersCount}
            </Card.Content>
        </Card>
    )
}

export default observer(ProfileCard);
