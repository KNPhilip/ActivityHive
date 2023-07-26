import { Link } from 'react-router-dom';
import { Button, Header, Icon, Segment } from 'semantic-ui-react';

const PageNotFound = () => {
    return (
        <Segment placeholder>
            <Header icon>
                <Icon name="search" />
                Oops - Unfortunately, this page doesn't exist :/
            </Header>
            <Segment.Inline>
                <Button as={Link} to="/activities">
                    Return to activities page
                </Button>
            </Segment.Inline>
        </Segment>
    )
}

export default PageNotFound;
