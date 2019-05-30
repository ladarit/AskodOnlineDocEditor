import * as React from 'react';
import * as ReactDOM from 'react-dom';
import isEqual from 'lodash-es/isEqual';
import isEmpty from 'lodash-es/isEmpty';
import isNil from 'lodash-es/isNil';
import * as styles from '../../css/pageNumber.scss';

interface IPageNumberProps {
	startVisiblePageIndex: number;
	documentId: string;
	pagesCount: number;
}

interface IPageNumberState {
	startVisiblePageIndex: number;
}

export default class PageNumber extends React.Component<IPageNumberProps, IPageNumberState> {
	constructor(props: IPageNumberProps) {
		super(props);
	}

	public state: Readonly<IPageNumberState> = {
		startVisiblePageIndex: undefined
	};

	public static getDerivedStateFromProps(nextProps: IPageNumberProps, prevState: IPageNumberState) {
		const stateChange: any = {};
		if (!isEqual(nextProps.startVisiblePageIndex, prevState.startVisiblePageIndex)) {
			stateChange.startVisiblePageIndex = nextProps.startVisiblePageIndex;
		}
		return !isEmpty(stateChange) ? stateChange : null;
	}

	private getOverlayContainer = (): HTMLElement => {
		let container = $('#pageNumberContainer')[0];
		if (!container) {
			container = document.createElement('div');
			container.id = 'pageNumberContainer';
			container.className = styles.page_number_container;
			$('#' + this.props.documentId + '_Bar')[0].appendChild(container);
		}
		return container;
	}

	public render() {
		if (isNil(this.state.startVisiblePageIndex)) {
			return null;
		}

		const content = (
			<div className={styles.page_number}>
				<span>
					{this.state.startVisiblePageIndex}/{this.props.pagesCount}
				</span>
			</div>
		);
		const targetElement = this.getOverlayContainer();

		return ReactDOM.createPortal(content, targetElement);
	}
}