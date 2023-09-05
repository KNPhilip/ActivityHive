import { Message } from "semantic-ui-react";

interface Props {
  errors?: string[];
}

const ValidationError = ({ errors }: Props) => {
  return (
    <Message error>
      {errors && (
        <Message.List>
          {errors.map((err: string, i: number) => (
            <Message.Item key={i}>{err}</Message.Item>
          ))}
        </Message.List>
      )}
    </Message>
  );
};

export default ValidationError;
