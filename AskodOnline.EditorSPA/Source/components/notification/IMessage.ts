export default interface IMessage {
	id: string;
	content: string;
	readed: boolean;
	showtype: MessageShowType;
	position?: IMessagePosition;
	messageType?: string;
}

export enum MessageShowType {
	Dialog = 1,
	Snackbar = 2
}

export enum MessageHorizontalPosition {
	Left = 'left',
	Center = 'center',
	Right = 'right'
}

export enum MessageVerticalPosition {
	Top = 'top',
	Bottom = 'bottom'
}

export interface IMessagePosition {
	horizontal: MessageHorizontalPosition;
	vertical: MessageVerticalPosition;
}