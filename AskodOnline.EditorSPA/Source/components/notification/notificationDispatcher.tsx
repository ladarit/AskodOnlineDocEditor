import * as React from 'react';
import NotificationPresenter from './notificationPresenter';
import isEqual from 'lodash-es/isEqual';
import isEmpty from 'lodash-es/isEmpty';
import head from 'lodash-es/head';
import some from 'lodash-es/some';
import { IReduxStoreState } from '../../store/redux/reduxStore';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import * as actions from '../../store/redux/actions';
import IMessage from './IMessage';

interface INotificationDispatcherProps {
	messageQueue: IMessage[];
	actions: any;
}

interface INotificationDispatcherState {
	messageQueue: IMessage[];
	currentMessage: IMessage;
}

class NotificationDispatcher extends React.Component<INotificationDispatcherProps, INotificationDispatcherState> {
	constructor(props: INotificationDispatcherProps) {
		super(props);
	}

	public state: Readonly<INotificationDispatcherState> = {
		messageQueue: [],
		currentMessage: null
	};

	public static getDerivedStateFromProps(nextProps: INotificationDispatcherProps, prevState: INotificationDispatcherState) {
		const stateChange: any = {};
		if (!isEqual(nextProps.messageQueue, prevState.messageQueue)) {
			stateChange.messageQueue = nextProps.messageQueue;
			if (some(stateChange.messageQueue, Boolean)) {
				stateChange.currentMessage = head(nextProps.messageQueue);
			} else {
				stateChange.currentMessage = null;
			}
		}
		return !isEmpty(stateChange) ? stateChange : null;
	}

	private OnMessageClosed = (messageId: string): void => {
		this.props.actions.DeleteMessageFromQueue(messageId);
	}

	public render() {
		return (
			<React.Fragment>
				{this.state.currentMessage &&
					<NotificationPresenter
						message={this.state.currentMessage}
						onDialogClosed={this.OnMessageClosed}
					/>
				}
				{!this.state.currentMessage && null}
			</React.Fragment>
		);
	}
}

const mapStateToProps = (state: IReduxStoreState) => {
	return {
		messageQueue: state.messageQueue
	};
};

const mapDispatchToProps = (dispatch: any) => {
	return {
		actions: bindActionCreators(actions, dispatch)
	};
};

export default connect(mapStateToProps, mapDispatchToProps)(NotificationDispatcher);