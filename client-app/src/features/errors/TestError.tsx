import {Button, Header, Segment} from "semantic-ui-react";
import axios from 'axios';

const TestErrors = () => {
    const baseUrl = 'http://localhost:5000/api/'

    function handleNotFound() {
        axios.get(baseUrl + 'error/not-found').catch(err => console.log(err.response));
    }

    function handleBadRequest() {
        axios.get(baseUrl + 'error/bad-request').catch(err => console.log(err.response));
    }

    function handleServerError() {
        axios.get(baseUrl + 'error/server-error').catch(err => console.log(err.response));
    }

    function handleUnauthorised() {
        axios.get(baseUrl + 'error/unauthorised').catch(err => console.log(err.response));
    }

    function handleBadGuid() {
        axios.get(baseUrl + 'activities/notaguid').catch(err => console.log(err.response));
    }

    function handleValidationError() {
        axios.post(baseUrl + 'activities', {}).catch(err => console.log(err.response));
    }

    return (
        <>
            <Header as="h1" content="Test Error Component" />
            <Segment>
                <Button.Group widths="7">
                    <Button onClick={handleNotFound} content="Not Found" basic primary />
                    <Button onClick={handleBadRequest} content="Bad Request" basic primary />
                    <Button onClick={handleValidationError} content="Validation Error" basic primary />
                    <Button onClick={handleServerError} content="Server Error" basic primary />
                    <Button onClick={handleUnauthorised} content="Unauthorised" basic primary />
                    <Button onClick={handleBadGuid} content="Bad Guid" basic primary />
                </Button.Group>
            </Segment>
        </>
    )
}

export default TestErrors;
