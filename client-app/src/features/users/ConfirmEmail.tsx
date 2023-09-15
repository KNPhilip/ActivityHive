import { useEffect, useState } from "react";
import { useStore } from "../../app/stores/store";
import useQuery from "../../app/util/hooks";
import agent from "../../app/api/agent";
import { toast } from "react-toastify";
import { Button, Header, Icon, Segment } from "semantic-ui-react";
import LoginForm from "./LoginForm";

const ConfirmEmail = () => {
    const { modalStore } = useStore();
    const email = useQuery().get('email') as string;
    const token = useQuery().get('token') as string;
 
    const Status = {
        Verifying: 'Verifying',
        Failed: 'Failed',
        Success: 'Success'
    }

    const [status, setStatus] = useState(Status.Verifying);

    function handleConfirmEmailResend() {
        agent.Auth.resendEmailConfirm(email).then(() => {
            toast.success('Verification email resent - please check your inbox.');
        }).catch(error => console.log(error));
    }

    useEffect(() => {
        agent.Auth.verifyEmail(token, email).then(() => {
            setStatus(Status.Success);
        }).catch(() => {
            setStatus(Status.Failed);
        })
    }, [Status.Failed, Status.Success, token, email]);

    function getBody() {
        switch (status) {
            case Status.Verifying:
                return <p>Verifying...</p>
            case Status.Failed:
                return (
                    <div>
                        <p>Verification failed. Try resending the verify link to your email</p>
                        <Button 
                            primary 
                            onClick={handleConfirmEmailResend}
                            size="huge" 
                            content="Resend Email" 
                        />
                    </div>
                );
            case Status.Success:
                return (
                    <div>
                        <p>Email has been verified - You can now login</p>
                        <Button 
                            primary 
                            onClick={() => 
                                modalStore.openModal(<LoginForm />)} 
                            size="huge" 
                            content="Login" 
                        />
                    </div>
                );
        }
    }

    return (
        <Segment placeholder textAlign="center">
            <Header icon>
                <Icon name="envelope" />
                Email Verification
            </Header>
            <Segment.Inline>
                {getBody()}
            </Segment.Inline>
        </Segment>
    );
}

export default ConfirmEmail;
