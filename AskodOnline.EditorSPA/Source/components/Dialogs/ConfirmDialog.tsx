import * as React from 'react';
import * as ReactModal from 'react-modal';
import IModalButton from './IModalButton';
import * as styles from '../../css/modal.scss';
import i18next from 'i18next';
import uniqid = require('uniqid');
import isEmpty from 'lodash-es/isEmpty';
import isEqual from 'lodash-es/isEqual';

ReactModal.setAppElement('#ReactModal');

interface IDialogProps {
	dialogSettings: IDialogSettings;
	isOpen: boolean;
	closePortal: (...args: any[]) => void;
}

interface IDialogState {
	dialogSettings: IDialogSettings;
	isOpen: boolean;
}

export interface IDialogSettings {
	onDeclineCloseModal?: (...args: any[]) => void;
	title: string;
	contentText: string;
	showCancelButton: boolean;
	buttonsCollection: IModalButton[];
	style?: any;
}

export default class ConfirmDialog extends React.Component<IDialogProps, IDialogState> {

	public state: Readonly<IDialogState> = {
		dialogSettings: undefined,
		isOpen: false
	};

	public static getDerivedStateFromProps(nextProps: IDialogProps, prevState: IDialogState) {
		const stateChange: any = {};
		if (!isEqual(nextProps.dialogSettings, prevState.dialogSettings)) {
			stateChange.dialogSettings = nextProps.dialogSettings;
			stateChange.isOpen = nextProps.isOpen;
		}
		return !isEmpty(stateChange) ? stateChange : null;
	}

	public onCloseModal = (closeFn: () => void) => {
		const root = this;
		this.setState({ isOpen: false }, () => root.props.closePortal(closeFn));
	}

	public render() {
		let { content } = modalContainer;
		content = this.state.dialogSettings.style ? { ...content, ...this.state.dialogSettings.style } : content;
		const containerStyle = { content };
		return (
			<ReactModal isOpen={this.state.isOpen} style={containerStyle} overlayClassName={styles.modal_overlay}>
				<div className={styles.flex_container}>
					<h4 className={styles.modal_header}>
						{this.state.dialogSettings.title}
					</h4>
					<button
						className={styles.modal_close}
						onClick={() => this.onCloseModal(this.state.dialogSettings.onDeclineCloseModal)}
					/>
				</div>
				<div className={styles.modal_content}>
					{this.state.dialogSettings.contentText}
				</div>
				<div className={styles.action_container}>
					{this.state.dialogSettings.buttonsCollection.map((button: IModalButton) => {
						const closeFn = () => this.onCloseModal(button.onBtnClick);
						return <button key={uniqid()} onClick={closeFn} className={styles.action}>{button.caption.toUpperCase()}</button>;
					})}
					{this.state.dialogSettings.showCancelButton &&
						<button
							key={uniqid()}
							onClick={() => this.onCloseModal(this.state.dialogSettings.onDeclineCloseModal)}
							className={styles.action}
						>
							{i18next.t('cancelBtn').toUpperCase()}
						</button>
					}
				</div>
			</ReactModal>
		);
	}
}

const modalContainer = {
	content: {
		top: '50%',
		left: '50%',
		right: 'auto',
		bottom: 'auto',
		marginRight: '-50%',
		transform: 'translate(-50%, -50%)',
		overflow: 'hidden !important',
		minWidth: '500px',
		maxWidth: '1000px',
		borderRadius: '10px',
		padding: '20px 20px 20px 30px',
		border: '1px solid rgb(162, 162, 162)',
		boxShadow: '0 0 2.5rem rgba(0,0,0,0.5)'
	}
};
