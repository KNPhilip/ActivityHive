import { toast } from "react-toastify";
import agent from "../../app/api/agent";
import useQuery from "../../app/util/hooks";
import { Button, Header, Icon, Segment } from "semantic-ui-react";

const RegisterSuccess = () => {
    const email = useQuery().get('email') as string;

    function handleConfirmEmailResend() {
        agent.Auth.resendEmailConfirm(email).then(() => {
            toast.success('Verification email resent - please check your inbox.');
        }).catch(error => console.log(error));
    }

    return (
        <Segment placeholder textAlign="center">
            <Header icon color="green">
                <Icon name="check" />
                Successfully registered!
            </Header>
            <p>Please check your inbox for verification (including junk email)</p>
            { email && 
                <>
                    <p>Didn't receive the email? Click the bellow button to resend</p>
                    <Button primary onClick={handleConfirmEmailResend} content="Resend Email" size="huge" />
                </>
            }
        </Segment>
    );
}

export default RegisterSuccess;
