
// frameworks
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { connect } from 'react-redux';
// components
import UserView from '../UserView/UserView';
// interfaces
import IUserInfo from '../UserView/IUserInfo';
// styling
import * as styles from '../../css/usersList.scss';
// utils
import i18next from 'i18next';
import { IReduxStoreState } from '../../store/redux/reduxStore';
import isEqual from 'lodash-es/isEqual';
import isEmpty from 'lodash-es/isEmpty';

interface IUsersListProps {
	usersCollection: IUserInfo[];
}

interface IUsersListState {
	usersCollection: IUserInfo[];
}

class UsersList extends React.Component<IUsersListProps, IUsersListState> {

	public state: Readonly<IUsersListState> = {
		usersCollection: []
	};

	public static getDerivedStateFromProps(nextProps: IUsersListProps, prevState: IUsersListState) {
		const stateChange: any = {};
		if (!isEqual(nextProps.usersCollection, prevState.usersCollection)) {
			stateChange.usersCollection = nextProps.usersCollection;
		}
		return !isEmpty(stateChange) ? stateChange : null;
	}

	public render() {
		const usersList = (
			<React.Fragment>
				<div className={styles.users_list}>
					<p className={styles.user_list_header}><b>{i18next.t('UserListHeader')}</b></p>
					<hr className={styles.user_list_head_splitter}/>
				</div>
				{this.state.usersCollection.map((user) => <UserView key={user.Id} user={user}/>)}
			</React.Fragment>
		);

		return ReactDOM.createPortal(usersList, document.getElementById('usersList'));
	}
}

const mapStateToProps = (state: IReduxStoreState, ownProps: any) => {
	return {
		usersCollection: state.activeUsers
	};
};

export default connect(mapStateToProps)(UsersList);
