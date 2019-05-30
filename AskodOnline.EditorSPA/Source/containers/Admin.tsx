import * as React from 'react';
import { connect } from 'react-redux';
import { IReduxStoreState } from '../store/redux/reduxStore';
import LoginForm from '../components/LoginForm/LoginForm';
import Dashboard from '../components/dashboard/dashboard';

interface IAdminAppProps {
	adminPageVisible: boolean;
	authenticated: boolean;
}

interface IAdminAppState {
	showLoginForm: boolean;
}

class AdminApp extends React.Component<IAdminAppProps, IAdminAppState> {

	public state: Readonly<IAdminAppState> = {
		showLoginForm: true
	};

	public render(): JSX.Element {
		return (
			<React.Fragment>
				{this.props.adminPageVisible &&
					<React.Fragment>
						{!this.props.authenticated && <LoginForm/>}
						{this.props.authenticated && <Dashboard/>}
					</React.Fragment>
				}
				{!this.props.adminPageVisible && null}
			</React.Fragment>
		);
	}
}

const mapStateToProps = (state: IReduxStoreState) => {
	return {
		adminPageVisible: state.adminPageVisible,
		authenticated: state.adminAuthenticated
	};
};

export default connect(mapStateToProps)(AdminApp);