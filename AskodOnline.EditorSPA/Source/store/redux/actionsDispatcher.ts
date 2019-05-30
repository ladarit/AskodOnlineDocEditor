
import { bindActionCreators, Dispatch } from 'redux';
import * as actions from './actions';
import { store, IReducerAction } from '../redux/reduxStore';

export default class ActionsDispatcher {

	private static dispatch: Dispatch<IReducerAction> = store.dispatch;
	private static actions = bindActionCreators(actions, ActionsDispatcher.dispatch);

	static get Actions() {
		return this.actions;
	}
}