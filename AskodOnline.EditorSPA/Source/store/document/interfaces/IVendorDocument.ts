import ICallbackDocument from './ICallbackDocument';

export default interface IVendorDocument {
	HasUnsavedChanges: () => void;
	PerformCallback: (commandObject?: ICommand) => void;
	throwSessionExpiredAlert: () => void;
	cpFileModel: ICallbackDocument;
	cpTransactionResult: ITransactionResult;
	uniqueID: string;
	commands: any;
	isInitialized: boolean;
}

export interface ICommand {
	command: string;
}

export interface IRichEditVendorDocument extends IVendorDocument {
	core: any;
}

export interface ITransactionResult {
	ErrorCode: number;
	ErrorMessage: string;
	IsNewFile: boolean;
	NewFileCounter: number;
	Command: string;
}