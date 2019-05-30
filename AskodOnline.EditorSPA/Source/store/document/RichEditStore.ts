import isNil from 'lodash-es/isNil';
import UserClaimsManager from '../userClaimsManager';
import IModalButton from '../../components/Dialogs/IModalButton';
import DocumentStore from './DocumentStore';
import i18next from 'i18next';
import { IRichEditVendorDocument, ICommand } from './interfaces/IVendorDocument';
import ActionsDispatcher from '../redux/actionsDispatcher';
import { TimeoutBasedPromise } from '../../helper/timeoutBasedPromise';
import 'root/Source/interfaces/IJqueryExtended';

export default class RichEditStore extends DocumentStore {
	private static isSynchronized: boolean = false;
	private static canSetScrollPosition: boolean = false;
	private static scrollPosition: number = 0;
	private static startVisiblePageIndex: number = undefined;
	private static vendorFormatter: any = null;
	private static documentFormattingFinished: boolean = false;

	static set DocumentChangedStatus(value: boolean) {
		DocumentStore.documentChangedStatus = value;
		if (value === true) {
			const sendDocChangedNotification = () => {
				DocumentStore.SendDocChangedNotification(10000, RichEditStore.SendDocChangedNotification);
			};
			DocumentStore.SendTypingNotification(2000, sendDocChangedNotification);
		}
	}

	private static get VendorFormatter(): any {
		return RichEditStore.vendorFormatter;
	}

	private static set VendorFormatter(value: any) {
		RichEditStore.vendorFormatter = value;
	}

	private static get SynchronizedState(): boolean {
		return RichEditStore.isSynchronized;
	}

	private static set SynchronizedState(value: boolean) {
		RichEditStore.isSynchronized = value;
	}

	private static get ScrollPosition(): number {
		return RichEditStore.scrollPosition;
	}

	private static set ScrollPosition(value: number) {
		RichEditStore.scrollPosition = value;
	}

	private static get ScrollPositionNative(): number {
		return (DocumentStore.Document as IRichEditVendorDocument).core.getScrollTop();
	}

	private static set ScrollPositionNative(value: number) {
		(DocumentStore.Document as IRichEditVendorDocument).core.setScrollTop(value);
	}

	private static get DocumentFormattingState(): boolean {
		return RichEditStore.documentFormattingFinished;
	}

	private static set DocumentFormattingState(value: boolean) {
		RichEditStore.documentFormattingFinished = value;
	}

	private static ForceSyncWithServer() {
		RichEditStore.Document.commands.forceSyncWithServer.execute();
	}

	private static async SaveDocument(commandObject: ICommand) {
		try {
			RichEditStore.SynchronizedState = false;
			await TimeoutBasedPromise(RichEditStore.ForceSyncWithServer, () => RichEditStore.SynchronizedState === true);
			RichEditStore.Document.PerformCallback(commandObject);
		} catch (failReason) {
			console.log(failReason);
		}
	}

	public static OnSaveBtnClick() {
		const doc = RichEditStore.CallbackDocument;
		const user = UserClaimsManager.Claims;
		if (!isNil(doc) && !isNil(user)) {
			const buttons: IModalButton[] = [];
			const saveAsNewVersionBtn: IModalButton = {
				caption: i18next.t('saveNewVersion'),
				onBtnClick: () => { RichEditStore.SaveDocument({ command: 'saveNewVersion' }); }
			};
			buttons.push(saveAsNewVersionBtn);

			if (doc.IsDocSign === false && user.Counter === doc.AuthorId) {
				const saveAsCurrentVersionBtn: IModalButton = {
					caption: i18next.t('saveCurrentVersion'),
					onBtnClick: () => { RichEditStore.SaveDocument({ command: 'saveCurrentVersion' }); }
				};
				buttons.push(saveAsCurrentVersionBtn);
			}

			DocumentStore.OnSaveBtnClick(buttons);
		}
	}

	public static OnBeginCallBack(e: any): void {
		DocumentStore.IsNeedRestoreView = true;
		DocumentStore.setCustomAttributes(e);
		RichEditStore.RememberScrollPosition();
	}

	public static OnDocumentLoaded(): void {
		RichEditStore.documentFormattingFinished = false;
		RichEditStore.SynchronizedState = true;
		this.IsUndoOperationActive = false;
		super.OverrideVendorSessionExpiredAlert();
		RichEditStore.OverrideVendorFormattingFn();
		RichEditStore.OverrideUpdatePageIndexesInfo();
		DocumentStore.CheckForIncomingNotifications();
		if (RichEditStore.ScrollPosition !== 0 && DocumentStore.IsNeedRestoreView) {
			RichEditStore.SetScrollPosition();
		}
	}

	public static OnEndSynchronization(): void {
		RichEditStore.SynchronizedState = true;
		DocumentStore.sendMessageToClients('documentChanged');
	}

	public static RememberScrollPosition(): void {
		RichEditStore.ScrollPosition = RichEditStore.ScrollPositionNative;
	}

	public static SetScrollPosition(): void {
		ActionsDispatcher.Actions.ShowOverlay(true);
		$('.dxreView').css('overflow', 'hidden');
		RichEditStore.canSetScrollPosition = true;
	}

	public static OverrideVendorFormattingFn() {
		RichEditStore.VendorFormatter = (DocumentStore.Document as IRichEditVendorDocument).core.layoutFormatterManager.mainFormatter;
		const vendorFnQueue = RichEditStore.VendorFormatter.stateMap;
		const vendorEndFormattingFn = vendorFnQueue[8];
		const overridenEndFormattingFn = () => {
			RichEditStore.documentFormattingFinished = true;
			vendorEndFormattingFn.call(RichEditStore.VendorFormatter);
			if (RichEditStore.canSetScrollPosition) {
				RichEditStore.canSetScrollPosition = false;
				$('.dxreView').css('overflow', 'auto');
				RichEditStore.ScrollPositionNative = !isNil(RichEditStore.ScrollPosition) ? RichEditStore.ScrollPosition : 0;
				ActionsDispatcher.Actions.ShowOverlay(false);
			}
		};
		vendorFnQueue[8] = overridenEndFormattingFn;
	}

	public static OverrideUpdatePageIndexesInfo() {
		try {
			const root = this;
			const canvasManager = (DocumentStore.Document as IRichEditVendorDocument).core.canvasManager;
			const context = canvasManager.scroll;
			const vendorFn = canvasManager.scroll.updatePageIndexesInfo;
			const overridenVendorFn = async () => {
				vendorFn.call(context, canvasManager.layout.pages);
				await TimeoutBasedPromise(null, () => RichEditStore.documentFormattingFinished === true);
				if (context.startVisiblePageIndex !== root.startVisiblePageIndex) {
					ActionsDispatcher.Actions.setStartPageIndex(context.startVisiblePageIndex);
					ActionsDispatcher.Actions.setTotalPagesCount(canvasManager.layout.pages.length);
				}
			};
			canvasManager.scroll.updatePageIndexesInfo = overridenVendorFn;
		} catch (failReason) {
			console.log(failReason);
		}
	}

	public static OverrideSpellCheker(): void {
		const context = (DocumentStore.Document as IRichEditVendorDocument).core.spellChecker;
		const vendorFn = (DocumentStore.Document as IRichEditVendorDocument).core.spellChecker.checkCore;
		const overridenVendorFn = () => {
			TimeoutBasedPromise(null, () => $.connection.hub.state === 1)
			.then(() => {
				vendorFn.call(context);
			}).catch((error: any) => {
				console.log(error);
			});
		};
		(DocumentStore.Document as IRichEditVendorDocument).core.spellChecker.checkCore = overridenVendorFn;
	}

	public static SendDocChangedNotification = async () => {
		if (!isNil(RichEditStore.CallbackDocument)) {
			DocumentStore.AllowSendNotification = true;
			RichEditStore.ForceSyncWithServer();
		}
	}

	public static OnCustomBtnClick(event: any) {
		if (event.commandName === 'saveButton') {
			RichEditStore.OnSaveBtnClick();
		}
		if (event.commandName === 'undoAllButton') {
			super.OnUndoAllBtnClick();
		}
	}
}