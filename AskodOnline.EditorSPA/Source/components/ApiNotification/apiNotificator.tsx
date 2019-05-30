import * as React from 'react';
import WorkersStore, { IWorkerState } from 'root/Source/store/worker/workerStore';
import { connect } from 'react-redux';
import { IReduxStoreState, IWebConfigVariable } from 'root/Source/store/redux/reduxStore';
import UserClaimsManager from 'root/Source/store/userClaimsManager';
import find from 'lodash-es/find';
import { isNumber } from 'util';

type IApiNotificatorProps = IStateFromProps;

interface IApiNotificatorState {
	notificationStarted: boolean;
}

class ApiNotificator extends React.Component<IApiNotificatorProps, IApiNotificatorState> {

	public state: Readonly<IApiNotificatorState> = {
		notificationStarted: false
	};

	private workerStoreInstanse: WorkersStore;
	private scriptPath: string = location.origin + '/Scripts/build/pingFileEditingWorker.js';

	public componentDidMount() {
		const pingFileEditingVariable = find(this.props.webConfigVariables, (v) => v.name.toLowerCase() === 'pingfileeditinginterval');
		if (pingFileEditingVariable && isNumber(pingFileEditingVariable.value) && pingFileEditingVariable.value > 0) {
			const interval = pingFileEditingVariable.value;
			if (!this.state.notificationStarted) {
				this.workerStoreInstanse = new WorkersStore();
				const options = {
					requestData: {
						file: { counter: this.props.teamworkId },
						user: UserClaimsManager.Claims,
						interval
					}
				};
				this.workerStoreInstanse.startWorker(this.scriptPath, options, this.onMessageFromWorkerReceived);
			}
		}
	}

	public onMessageFromWorkerReceived = (messageEvent: any) => {
		const root = this;
		if (messageEvent.data === IWorkerState.ACTIVATED) {
			this.setState({ notificationStarted: true });
		}
		if (messageEvent.data === IWorkerState.ERROR) {
			this.setState({ notificationStarted: false }, () => {
				root.workerStoreInstanse.terminateWorker(root.scriptPath);
			});
		}
	}

	public render(): JSX.Element {
		return null;
	}
}

interface IStateFromProps {
	teamworkId: number;
	webConfigVariables: IWebConfigVariable[];
}

const mapStateToProps = (state: IReduxStoreState): IStateFromProps => {
	return {
		teamworkId: state.teamworkId,
		webConfigVariables: state.webConfigVariables
	};
};

export default connect(mapStateToProps)(ApiNotificator);