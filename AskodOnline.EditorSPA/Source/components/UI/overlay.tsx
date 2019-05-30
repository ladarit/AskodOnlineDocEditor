import * as React from 'react';
import * as ReactDOM from 'react-dom';
import * as styles from '../../css/overlay.scss';

class Overlay extends React.Component<any, any> {

	private getOverlayContainer = (): HTMLElement => {
		let container = $('#spinnerContainer')[0];
		if (!container) {
		   container = document.createElement('div');
		   container.id = 'spinnerContainer';
		   $('.dxreView')[0].appendChild(container);
		}
		return container;
	}

	public render(): JSX.Element {
		const overlay = (
			<div className={styles.overlay_container}>
				<div className={[styles.overlay,styles.loader].join(' ')}/>
			</div>
		);
		const targetElement = this.getOverlayContainer();
		return ReactDOM.createPortal(overlay, targetElement);
	}
}

export default Overlay;
