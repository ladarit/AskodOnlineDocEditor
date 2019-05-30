import isNil from 'lodash-es/isNil';
import UserClaimsManager from '../userClaimsManager';
import IModalButton from '../../components/Dialogs/IModalButton';
import DocumentStore from './DocumentStore';
import i18next from 'i18next';

export default class SpreadSheetStore extends DocumentStore {
	static set DocumentChangedStatus(value: boolean) {
		DocumentStore.documentChangedStatus = value;
		if (value === true) {
			DocumentStore.SendDocChangedNotification(10000, SpreadSheetStore.SendDocChangedNotification);
		}
	}

	public static SendDocChangedNotification = async () => {
		if (!isNil(DocumentStore.CallbackDocument)) {
			DocumentStore.AllowSendNotification = true;
			DocumentStore.sendMessageToClients('documentChanged');
		}
	}

	public static OnCustomBtnClick(event: any) {
		if (event.commandName === 'saveButton') {
			SpreadSheetStore.OnSaveBtnClick();
		}
		if (event.commandName === 'undoAllButton') {
			super.OnUndoAllBtnClick();
		}
	}

	public static OnSaveBtnClick() {
		const doc = DocumentStore.CallbackDocument;
		const user = UserClaimsManager.Claims;
		if (!isNil(doc) && !isNil(user)) {
			const buttons: IModalButton[] = [];
			const saveAsNewVersionBtn: IModalButton = {
				caption: i18next.t('saveNewVersion'),
				onBtnClick: () => { DocumentStore.Document.PerformCallback({ command: 'saveNewVersion' }); }
			};
			buttons.push(saveAsNewVersionBtn);

			if (doc.IsDocSign === false && user.Counter === doc.AuthorId) {
				const saveAsCurrentVersionBtn: IModalButton = {
					caption: i18next.t('saveCurrentVersion'),
					onBtnClick: () => { DocumentStore.Document.PerformCallback({ command: 'saveCurrentVersion' }); }
				};
				buttons.push(saveAsCurrentVersionBtn);
			}

			DocumentStore.OnSaveBtnClick(buttons);
		}
	}
}