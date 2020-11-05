import {Link, useHistory} from 'react-router-dom';
import UserSignUp from './UserSignUp';

const UserSignOut = () => {
    const history = useHistory();
    localStorage.removeItem("accessToken");
    localStorage.removeItem("username");

    history.push("/");
    return([]);
}

export default UserSignOut;
