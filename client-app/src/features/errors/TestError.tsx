import { Button, Header, Segment } from "semantic-ui-react";
import axios from "axios";
import { useState } from "react";
import ValidationError from "./ValidationError";

const TestErrors = () => {
  const [errors, setErrors] = useState(null);

  function handleNotFound() {
    axios
      .get("http://httpstat.us/404")
      .catch((err) => console.log(err.response));
  }

  function handleBadRequest() {
    axios.get("/error/bad-request").catch((err) => console.log(err.response));
  }

  function handleServerError() {
    axios.get("/error/server-error").catch((err) => console.log(err.response));
  }

  function handleUnauthorised() {
    axios
      .get("http://httpstat.us/401")
      .catch((err) => console.log(err.response));
  }

  function handleBadGuid() {
    axios.get("/activities/notaguid").catch((err) => setErrors(err));
  }

  function handleValidationError() {
    axios.post("/activities", {}).catch((err) => setErrors(err));
  }

  return (
    <>
      <Header as="h1" content="Test Error Component" />
      <Segment>
        <Button.Group widths="7">
          <Button onClick={handleNotFound} content="Not Found" basic primary />
          <Button
            onClick={handleBadRequest}
            content="Bad Request"
            basic
            primary
          />
          <Button
            onClick={handleValidationError}
            content="Validation Error"
            basic
            primary
          />
          <Button
            onClick={handleServerError}
            content="Server Error"
            basic
            primary
          />
          <Button
            onClick={handleUnauthorised}
            content="Unauthorised"
            basic
            primary
          />
          <Button onClick={handleBadGuid} content="Bad Guid" basic primary />
        </Button.Group>
      </Segment>
      {errors && <ValidationError errors={errors} />}
    </>
  );
};

export default TestErrors;
