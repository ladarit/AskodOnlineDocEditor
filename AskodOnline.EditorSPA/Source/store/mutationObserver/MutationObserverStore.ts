import isNil from 'lodash-es/isNil';
import IObserverConfiguration from './IObserverConfiguration';

export default class MutationObserverStore {
	constructor(targetElement: string, handler: () => void, config?: IObserverConfiguration) {
		this.targetElement = targetElement;
		this.config = config ? config : this.config;
		this.handler = handler ? handler : this.handler;
	}

	private observer: MutationObserver = undefined;
	private targetElement: any = undefined;
	private handler: () => void;
	private config: IObserverConfiguration = {
		attributes: false,
		characterData: false,
		childList: false,
		subtree: false
	};

	public start() {
		this.observer = new MutationObserver(this.handler);
		const elTarget = document.querySelector(this.targetElement);
		if (!isNil(elTarget)) {
			this.observer.observe(elTarget, this.config);
		} else {
			console.log('MutationObserver: can`t find target element in DOM');
		}
	}

	public disconnect() {
		this.observer.disconnect();
		this.observer = undefined;
	}
}