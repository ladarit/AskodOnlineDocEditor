import ActionsDispatcher from './redux/actionsDispatcher';
import { IWebConfigVariable } from './redux/reduxStore';

export default class WebConfigStore {

	public static AddWebConfigVariable(variable: IWebConfigVariable) {
		ActionsDispatcher.Actions.AddWebConfigVariable(variable);
	}
}