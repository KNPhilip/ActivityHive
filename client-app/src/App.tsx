import { useEffect, useState } from 'react';
import './App.css';
import axios from 'axios';

function App() {
  const [activities, setActivities] = useState([]);

  useEffect(() => {
    axios.get('http://localhost:5000/api/activities')
      .then(response => {
        setActivities(response.data);
      });
  }, []);

  return (
    <div className="App">
      <h1>Hello Reactivities!</h1>
      <ul>
        {activities.map((acitvity: any) => (
          <li key={acitvity.id}>
            {acitvity.title}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;