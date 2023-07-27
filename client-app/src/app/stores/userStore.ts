import { makeAutoObservable } from "mobx";
import { User, UserFormValues } from "../models/user";
import agent from "../api/agent";

export default class UserStore {
    user: User | null = null;

    constructor() {
        makeAutoObservable(this);
    }

    get isLoggedIn() {
        return !!this.user;
    }

    login = async (creds: UserFormValues) => {
        try {
            const user = await agent.Auth.login(creds);
            console.log(user);
        }
        catch(error) {
            throw error;
        }
    }
}