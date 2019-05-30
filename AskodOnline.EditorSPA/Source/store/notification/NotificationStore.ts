//#region imports
import ActionsDispatcher from './../redux/actionsDispatcher';
import i18next from 'i18next';
import uniqid = require('uniqid');
import IMessage,
		{ MessageShowType,
		MessageHorizontalPosition,
		MessageVerticalPosition,
		IMessagePosition
		} from '../../components/notification/IMessage';
import isEmpty from 'lodash-es/isEmpty';
//#endregion

interface IIndexSignature {
	[key: string]: string;
}

export default class NotificationStore {

	private static commandsMessages: IIndexSignature = {
		documentChanged: 'documentChangedPopUp',
		openLastSavedVersion: 'loadLastSavedVersionSusseccedForClients',
		loadLastSavedVersion: 'loadLastSavedVersionSussecced',
		saveNewVersion: 'saveNewVersionSussecced',
		saveCurrentVersion: 'saveCurrentVersionSussecced'
	};

	public static AddMessageToQueue(command: string, position?: IMessagePosition, options?: IIndexSignature) {
		const msgContent = this.commandsMessages[command];
		if (!isEmpty(msgContent)) {
			const mesId = uniqid();
			const message: IMessage = {
				id: mesId,
				content: i18next.t(msgContent, options ? options : null),
				readed: false,
				showtype: MessageShowType.Snackbar,
				position: position ? position : {
					horizontal: MessageHorizontalPosition.Right,
					vertical: MessageVerticalPosition.Bottom
				}
			};
			ActionsDispatcher.Actions.AddMessageToQueue(message);
		}
	}
}