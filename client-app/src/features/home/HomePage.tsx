import { Link } from "react-router-dom";
import "./homePage.css";
import {
  Container,
  Header,
  Segment,
  Image,
  Button,
  Divider,
} from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { observer } from "mobx-react-lite";
import LoginForm from "../users/LoginForm";
import RegisterForm from "../users/RegisterForm";
import FacebookLogin from "@greatsumini/react-facebook-login";

const HomePage = () => {
  const { userStore, modalStore } = useStore();

  return (
    <Segment inverted textAlign="center" vertical className="masthead">
      <Container text>
        <Header as="h1" inverted>
          <Image
            size="massive"
            src="/assets/logo.png"
            alt="logo"
            style={{ marginBottom: 12 }}
          />
        </Header>
        {userStore.isLoggedIn ? (
          <>
            <Header as="h2" inverted content="Welcome to Reactivities" />
            <Button as={Link} to="/activities" size="huge" inverted>
              Go to Activities!
            </Button>
          </>
        ) : (
          <>
            <Button
              onClick={() => modalStore.openModal(<LoginForm />)}
              size="huge"
              inverted
            >
              Login
            </Button>
            <Button
              onClick={() => modalStore.openModal(<RegisterForm />)}
              size="huge"
              inverted
            >
              Register
            </Button>
            <Divider horizontal inverted>
              Or
            </Divider>
            <Button
              as={FacebookLogin}
              appId="976206497000319"
              size="huge"
              inverted
              color="facebook"
              content="Continue with Facebook"
              loading={userStore.fbLoading}
              onSuccess={(response: any) => {
                // userStore.facebookLogin(response.accessToken);
                console.log("Login success!", response);
              }}
              onFail={(response: any) => {
                console.log("Login failed..", response);
              }}
            />
          </>
        )}
      </Container>
    </Segment>
  );
};

export default observer(HomePage);
