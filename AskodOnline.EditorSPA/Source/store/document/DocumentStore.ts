//#region imports
// stores
import SignalRManager from '../signalR/signalRStore';
import UserClaimsManager from '../userClaimsManager';
// components
import { IDialogSettings } from '../../components/Dialogs/ConfirmDialog';
// interfaces
import IModalButton from '../../components/Dialogs/IModalButton';
import IVendorDocument, { ITransactionResult } from './interfaces/IVendorDocument';
import ICallbackDocument from './interfaces/ICallbackDocument';
import { MessageHorizontalPosition, MessageVerticalPosition } from '../../components/notification/IMessage';
// utils
import i18next from 'i18next';
import isNil from 'lodash-es/isNil';
import isFunction from 'lodash-es/isFunction';
import cloneDeep from 'lodash-es/cloneDeep';
import NotificationStore from '../notification/NotificationStore';
import { TimeoutBasedPromise, AllowOperationByTimeoutPromise } from '../../helper/timeoutBasedPromise';
import window from '../../interfaces/IWindow';
//#endregion

export default class DocumentStore {
	private static document: IVendorDocument = undefined;
	private static sendMessageToOtherClientsTimeoutId: any = undefined;
	private static sendClientTypingNotificationTimeoutId: any = undefined;
	private static allowSendNotification: boolean = false;
	private static isUndoOperationActive: boolean = false;
	protected static documentChangedStatus: boolean = false;
	protected static isNeedRestoreView: boolean = true;
	protected static dismissAllOperations: boolean = false;

	static get IsNeedRestoreView(): boolean {
		return DocumentStore.isNeedRestoreView;
	}

	static set IsNeedRestoreView(value: boolean) {
		DocumentStore.isNeedRestoreView = value;
	}

	static get Document(): IVendorDocument {
		return DocumentStore.document;
	}

	static set Document(value: IVendorDocument) {
		DocumentStore.document = value;
	}

	static get CallbackDocument(): ICallbackDocument {
		return DocumentStore.Document.cpFileModel;
	}

	static set CallbackDocument(value: ICallbackDocument) {
		DocumentStore.Document.cpFileModel = value;
	}

	static get IncomingNotification(): string {
		return DocumentStore.Document.cpTransactionResult &&
				DocumentStore.Document.cpTransactionResult.Command ? DocumentStore.Document.cpTransactionResult.Command : null;
	}

	static set IncomingNotification(value: string) {
		if (DocumentStore.Document.cpTransactionResult) {
			DocumentStore.Document.cpTransactionResult.Command = value;
		}
	}

	static get TransactionResult(): ITransactionResult {
		return DocumentStore.Document.cpTransactionResult;
	}

	private static get SendMessageToOtherClientsTimeoutId(): any {
		return DocumentStore.sendMessageToOtherClientsTimeoutId;
	}

	private static set SendMessageToOtherClientsTimeoutId(value: any) {
		DocumentStore.sendMessageToOtherClientsTimeoutId = value;
	}

	private static get SendClientTypingNotificationTimeoutId(): any {
		return DocumentStore.sendClientTypingNotificationTimeoutId;
	}

	private static set SendClientTypingNotificationTimeoutId(value: any) {
		DocumentStore.sendClientTypingNotificationTimeoutId = value;
	}

	static get AllowSendNotification(): boolean {
		return DocumentStore.allowSendNotification;
	}

	static set AllowSendNotification(value: boolean) {
		DocumentStore.allowSendNotification = value;
	}

	static get DocumentChangedStatus(): boolean {
		return DocumentStore.documentChangedStatus;
	}

	static set DocumentChangedStatus(value: boolean) {
		DocumentStore.documentChangedStatus = value;
	}

	static set IsUndoOperationActive(value: boolean) {
		DocumentStore.isUndoOperationActive = value;
	}

	static get IsUndoOperationActive(): boolean {
		return DocumentStore.isUndoOperationActive;
	}

	static set DismissAllOperations(value: boolean) {
		DocumentStore.dismissAllOperations = value;
	}

	static get DismissAllOperations(): boolean {
		return DocumentStore.dismissAllOperations;
	}

	public static SendDocChangedNotification = async (timeOut: number, callbackFn?: () => void) => {
		try {
			const getTimeoutIdFromPromise = (timeoutId: any) => {
				DocumentStore.SendMessageToOtherClientsTimeoutId = timeoutId;
			};
			await AllowOperationByTimeoutPromise(timeOut, DocumentStore.SendMessageToOtherClientsTimeoutId, getTimeoutIdFromPromise);
			// clearTimeout(DocumentStore.SendMessageToOtherClientsTimeoutId);
			DocumentStore.SendMessageToOtherClientsTimeoutId = null;
			if (!DocumentStore.DismissAllOperations) {
				if (isFunction(callbackFn)) {
					DocumentStore.IsNeedRestoreView = false;
					callbackFn();
				}
			} else {
				DocumentStore.DismissAllOperations = false;
			}
		} catch (failReason) {
			console.log(failReason);
		}
	}

	public static SendTypingNotification = async (timeOut: number, FnRunAfterHideTypingAnimation: () => void) => {
		try {
			if (isNil(DocumentStore.SendClientTypingNotificationTimeoutId)) {
				SignalRManager.sendTypingNotificationToClients({ command: 'showTypingAnimation' });
			}
			const getTimeoutIdFromPromise = (timeoutId: any) => {
				DocumentStore.SendClientTypingNotificationTimeoutId = timeoutId;
			};
			await AllowOperationByTimeoutPromise(timeOut, DocumentStore.SendClientTypingNotificationTimeoutId, getTimeoutIdFromPromise);
			// clearTimeout(DocumentStore.SendClientTypingNotificationTimeoutId);
			DocumentStore.SendClientTypingNotificationTimeoutId = null;
			SignalRManager.sendTypingNotificationToClients({ command: 'hideTypingAnimation' });
			if (isFunction(FnRunAfterHideTypingAnimation)) {
				FnRunAfterHideTypingAnimation();
			}
		} catch (failReason) {
			console.log(failReason);
		}
	}

	public static CheckForIncomingNotifications(): void {
		if (!isNil(DocumentStore.IncomingNotification)) {
			const position = {
				horizontal: MessageHorizontalPosition.Center,
				vertical: MessageVerticalPosition.Bottom
			};
			const notification = cloneDeep(DocumentStore.IncomingNotification);
			DocumentStore.IncomingNotification = null;
			NotificationStore.AddMessageToQueue(notification, position);
		}
	}

	public static setCustomAttributes(e: any) {
		e.customArgs['file'] = DocumentStore.CallbackDocument;
		e.customArgs['user'] = UserClaimsManager.Claims;
	}

	public static sendMessageToClients(outgoingCommand: string) {
		if (DocumentStore.AllowSendNotification) {
			SignalRManager.sendMessageToClients({ command: outgoingCommand });
			DocumentStore.AllowSendNotification = false;
		}
	}

	public static openModal = () => {
	}

	public static afterOpenModal = () => {
	}

	public static closeModal = () => {
	}

	public static OnSaveBtnClick(buttons: IModalButton[]) {
		const dialogSettings: IDialogSettings = {
			title: i18next.t('attention'),
			contentText: i18next.t('documentChanged'),
			showCancelButton: true,
			buttonsCollection: buttons
		};
		window.ShowModalDialog(dialogSettings);
	}

	protected static OnUndoAllBtnClick() {
		const buttons: IModalButton[] = [];
		const revertDocumentBtn: IModalButton = {
			caption: i18next.t('Continue'),
			onBtnClick: () => this.RevertDocument()
		};
		buttons.push(revertDocumentBtn);

		const dialogSettings: IDialogSettings = {
			title: i18next.t('attention'),
			contentText: i18next.t('DocumentLostChanges'),
			showCancelButton: true,
			buttonsCollection: buttons,
			style: { maxWidth: '600px' }
		};
		window.ShowModalDialog(dialogSettings);
	}

	protected static async RevertDocument() {
		try {
			const root = this;
			this.IsUndoOperationActive = true;
			this.Document.PerformCallback({ command: 'loadLastSavedVersion' });
			await TimeoutBasedPromise(null, () => root.IsUndoOperationActive === false);
			const position = {
				horizontal: MessageHorizontalPosition.Center,
				vertical: MessageVerticalPosition.Bottom
			};
			root.DismissAllOperations = true;
			root.AllowSendNotification = true;
			root.DocumentChangedStatus = false;
			NotificationStore.AddMessageToQueue('loadLastSavedVersion', position);
			DocumentStore.sendMessageToClients('openLastSavedVersion');
		} catch (failReason) {
			console.log(failReason);
		}
	}

	protected static OverrideVendorSessionExpiredAlert() {
		const overridenFn = () => {};
		DocumentStore.Document.throwSessionExpiredAlert = overridenFn;
	}
}