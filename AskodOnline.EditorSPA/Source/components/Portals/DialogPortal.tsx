import * as React from 'react';
import * as ReactDOM from 'react-dom';
import isEqual from 'lodash-es/isEqual';
import isNil from 'lodash-es/isNil';
import isEmpty from 'lodash-es/isEmpty';
import isFunction from 'lodash-es/isFunction';
import ConfirmDialog, { IDialogSettings } from '../../components/Dialogs/ConfirmDialog';

interface IDialogPortalProps {
	settings: IDialogSettings;
}

interface IDialogPortalState {
	show: boolean;
	settings: IDialogSettings;
}

export default class DialogPortal extends React.Component<IDialogPortalProps, IDialogPortalState> {
	constructor(props: IDialogPortalProps) {
		super(props);
	}

	public state: Readonly<IDialogPortalState> = {
		show: false,
		settings: null
	};

	public static getDerivedStateFromProps(nextProps: IDialogPortalProps, prevState: IDialogPortalState) {
		const stateChange: any = {};
		if (!isNil(nextProps.settings) && !isEqual(nextProps.settings, prevState.settings)) {
			stateChange.settings = nextProps.settings;
			stateChange.show = true;
		}
		return !isEmpty(stateChange) ? stateChange : null;
	}

	public shouldComponentUpdate(nextProps: IDialogPortalProps, nextState: IDialogPortalState) {
		if (isEqual(nextProps.settings, this.state.settings) || isNil(nextProps.settings)) {
			return false;
		}
		return true;
	}

	public ClosePortal = (dialogCloseCallback: (...args: any[]) => void) => {
		this.setState({ show: false }, isFunction(dialogCloseCallback) ? () => dialogCloseCallback() : () => {});
	}

	public render() {
		if (this.state.show && this.state.settings) {
			return ReactDOM.createPortal(
				<ConfirmDialog
					isOpen={true}
					closePortal={this.ClosePortal}
					dialogSettings={{ ...this.state.settings }}
				/>,
				$('#ReactModal')[0]
			);
		} else {
			return <div/>;
		}
	}
}