//#region imports
import IVendorDocument from '../document/interfaces/IVendorDocument';
import ActionsDispatcher from './../redux/actionsDispatcher';
import IUserInfo from '../../components/UserView/IUserInfo';
import NotificationStore from '../notification/NotificationStore';
import get from 'lodash-es/get';
import 'root/Source/interfaces/IJqueryExtended';
//#endregion

export default class SignalRManager {
	constructor(editedDocument: IVendorDocument) {
		this.Document = editedDocument;
	}

	private document: IVendorDocument = undefined;
	private connectionRetryingCounter: number = 0;

	private get Document(): IVendorDocument {
		return this.document;
	}

	private set Document(value: IVendorDocument) {
		this.document = value;
	}

	private get ConnectionRetryingCounter(): number {
		return this.connectionRetryingCounter;
	}

	private set ConnectionRetryingCounter(value: number) {
		this.connectionRetryingCounter = value;
	}

	get RequsetParameters(): any {
		return $.connection.hub.qs;
	}

	set RequsetParameters(value: any) {
		$.connection.hub.qs = { ...value, group: this.Document.uniqueID };
	}

	public static sendTypingNotificationToClients(obj: any) {
		$.connection.notificationHub.server.sendTypingNotificationToClients(get(obj, 'command', null));
	}

	public static sendMessageToClients(obj: any) {
		$.connection.notificationHub.server.sendMessageToClients(get(obj, 'command', null));
	}

	private connect(isReconnecting?: boolean) {
		this.RequsetParameters = { isReconnecting: isReconnecting === true };
		$.connection.hub.start().done(() => {
			console.log('signalR connection established');
		}).fail((reason: any) => {
			console.log(reason);
		});
	}

	public init() {
		const root = this;
		const notificationhub = $.connection.notificationHub;
		this.RequsetParameters = { isReconnecting: false };
		$.connection.hub.logging = true;
		ActionsDispatcher.Actions.SetTeamworkId(this.Document.cpFileModel.TeamworkId);
		ActionsDispatcher.Actions.SetDocumentId(this.Document.uniqueID);

		// this function call '...Partial' action of 'Spreadsheet/RichEdit' controller
		notificationhub.client.updateSpreadsheetPartialView = (userName: string, incomingCommand: string) => {
			NotificationStore.AddMessageToQueue(incomingCommand, null, { name: userName });
			root.Document.PerformCallback({ command: incomingCommand });
		};

		notificationhub.client.showTypingAnimation = (userId: string, incomingCommand: string) => {
			ActionsDispatcher.Actions.EnableUserTypingAnimation(userId);
		};

		notificationhub.client.hideTypingAnimation = (userId: string, incomingCommand: string) => {
			ActionsDispatcher.Actions.DisableUserTypingAnimation(userId);
		};

		// only called for a new user
		$.connection.notificationHub.client.onConnected = (users: IUserInfo[]) => {
			this.RequsetParameters = { isReconnecting: false };
			for (const user of users) {
				ActionsDispatcher.Actions.AddUser(user);
			}
		};

		$.connection.notificationHub.client.onReconnected = (users: IUserInfo[]) => {
			this.RequsetParameters = { isReconnecting: false };
			ActionsDispatcher.Actions.DeleteAllUsers();
			for (const user of users) {
				ActionsDispatcher.Actions.AddUser(user);
			}
		};

		// called for all users except the new user
		$.connection.notificationHub.client.onNewUserConnected = (user: IUserInfo) => {
			ActionsDispatcher.Actions.AddUser(user);
		};

		// called when user is disconnected for all users except the disconnected user
		$.connection.notificationHub.client.onUserDisconnected = (userId: string) => {
			ActionsDispatcher.Actions.DeleteUser(userId);
		};

		$.connection.hub.disconnected(() => {
			if ($.connection.hub.lastError) {
				console.log('Disconnected. Reason: ' +  $.connection.hub.lastError.message);
			}
			if (this.ConnectionRetryingCounter > 10) {
				console.log('maximum connection attempts reached');
				return;
			}
			setTimeout(() => {
				root.ConnectionRetryingCounter++;
				root.connect(true);
			}, 2000);
		});

		$.connection.hub.reconnecting(() => {
			console.log('reconnecting');
		});

		$.connection.hub.reconnected(() => {
			this.RequsetParameters = { isReconnecting: false };
		});

		this.connect();
	}
}

window.onbeforeunload = (e) => {
	// $.connection.notificationHub.connection.stop(true);
};