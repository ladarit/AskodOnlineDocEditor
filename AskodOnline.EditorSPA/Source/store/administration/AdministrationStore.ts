import ActionsDispatcher from './../redux/actionsDispatcher';

export default class AdministrationStore {

	public SetAdministrationPageVisible = () => {
		ActionsDispatcher.Actions.setAdminPageVisible(true);
	}
}