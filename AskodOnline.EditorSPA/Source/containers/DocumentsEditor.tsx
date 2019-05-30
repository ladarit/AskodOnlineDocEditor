import * as React from 'react';
import * as styles from '../css/app.scss';
import UsersList from '../components/UsersList/UsersList';
import NotificationDispatcher from '../components/notification/notificationDispatcher';
import { connect } from 'react-redux';
import { IReduxStoreState } from '../store/redux/reduxStore';
import Overlay from '../components/UI/overlay';
import PageNumber from '../components/PageNumber/PageNumber';
import window from '../interfaces/IWindow';
import { IDialogSettings } from '../components/Dialogs/ConfirmDialog';
import DialogPortal from '../components/Portals/DialogPortal';
import ApiNotificator from '../components/ApiNotification/apiNotificator';

type IDocumentsEditorProps = IStateFromProps;

interface IDocumentsEditorState {
	messageQueue: any[];
	dialog: IDialogSettings;
}

class DocumentsEditor extends React.Component<IDocumentsEditorProps, IDocumentsEditorState> {

	constructor(props: IDocumentsEditorProps) {
		super(props);
		window.ShowModalDialog = this.ShowModalDialog;
	}

	public state: Readonly<IDocumentsEditorState> = {
		messageQueue: [],
		dialog: null
	};

	public ShowModalDialog = (dialogSettings: IDialogSettings): void => {
		this.setState({ dialog: dialogSettings });
	}

	public render(): JSX.Element {
		return (
			<React.Fragment>
				{this.props.documentId &&
					<div className={styles.app_container}>
						<UsersList/>
						<NotificationDispatcher/>
						{this.props.showOverlay &&
							<Overlay/>
						}
						{<PageNumber
							startVisiblePageIndex={this.props.startVisiblePageIndex}
							documentId={this.props.documentId}
							pagesCount={this.props.pagesCount}
						/>}
						{this.state.dialog &&
							<DialogPortal settings={this.state.dialog}/>
						}
						<ApiNotificator/>
					</div>
				}
				{!this.props.documentId && null}
			</React.Fragment>
		);
	}
}

interface IStateFromProps {
	documentId: string;
	showOverlay: boolean;
	startVisiblePageIndex: number;
	pagesCount: number;
}

const mapStateToProps = (state: IReduxStoreState): IStateFromProps => {
	return {
		documentId: state.documentId,
		showOverlay: state.showOverlay,
		startVisiblePageIndex: state.startVisiblePageIndex,
		pagesCount: state.pagesCount
	};
};

export default connect(mapStateToProps)(DocumentsEditor);