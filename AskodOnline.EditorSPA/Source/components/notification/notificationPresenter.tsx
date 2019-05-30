import * as React from 'react';
import isEqual from 'lodash-es/isEqual';
import isEmpty from 'lodash-es/isEmpty';
import IMessage, { MessageShowType } from './IMessage';
import Snackbar from '../UI/snackbar';

interface INotificationPresenterProps {
	onDialogClosed: (messageId: string) => void;
	message: IMessage;
}

interface INotificationPresenterState {
	message: IMessage;
	open: boolean;
}

export default class NotificationPresenter extends React.Component<INotificationPresenterProps, INotificationPresenterState> {
	constructor(props: INotificationPresenterProps) {
		super(props);
	}

	public state: Readonly<INotificationPresenterState> = {
		message: null,
		open: false
	};

	public static getDerivedStateFromProps(nextProps: INotificationPresenterProps, prevState: INotificationPresenterState) {
		const stateChange: any = {};
		if (!isEqual(nextProps.message, prevState.message)) {
			stateChange.message = nextProps.message;
			stateChange.open = true;
		}
		return !isEmpty(stateChange) ? stateChange : null;
	}

	public handleClose = (event: any, reason: string) => {
		const root = this;
		if (reason === 'clickaway') {
		  return;
		}
		this.setState({ open: false }, () => {
			root.props.onDialogClosed(root.state.message.id);
		});
	}

	public render() {
	   return (
			<React.Fragment>
				{this.state.message &&
				this.state.message.showtype === MessageShowType.Snackbar &&
					<Snackbar
						message={this.state.message}
						open={this.state.open}
						handleClose={this.handleClose}
					/>
				}
			</React.Fragment>
		);
	}
}