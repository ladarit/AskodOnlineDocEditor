import ActionsDispatcher from './../redux/actionsDispatcher';

export default class AuthStore {

	public static SetAuthenticatedState = (value: boolean): void => {
		ActionsDispatcher.Actions.setAuthenticatedState(value);
	}
}