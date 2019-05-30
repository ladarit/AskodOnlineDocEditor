import isFunction from 'lodash-es/isFunction';
import isNil from 'lodash-es/isNil';

const TimeoutBasedPromise = (fn: () => any, resolveFn: () => boolean, rejectFn?: () => any, timeout?: number) => {
	return new Promise((resolve, reject) => {
		if (isFunction(fn)) {
			fn();
		}
		let retryCount = 0;
		let timeOut: any;
		const setWait = () => {
			timeOut = setTimeout(() => {
				if (rejectFn && rejectFn()) {
					reject();
				}
				if (resolveFn()) {
					clearTimeout(timeOut);
					resolve(true);
				} else {
					if (retryCount === 10) {
						reject('maximum retriyng count!');
					} else {
						retryCount++;
						setWait();
					}
				}
			},
			timeout ? timeout : 200);
		};
		setWait();
	});
};

const AllowOperationByTimeoutPromise = (timeOut: number, currentTimeoutId: any, returnNewTimeoutIdToCaller?: (timeoutId: any) => void) => {
	return new Promise((resolve: any, reject: any) => {
		if (!isNil(currentTimeoutId)) {
			clearTimeout(currentTimeoutId);
		}

		const newTimeoutId = setTimeout(() => {
			clearTimeout(newTimeoutId);
			resolve();
		}, timeOut);

		if (isFunction(returnNewTimeoutIdToCaller)) {
			returnNewTimeoutIdToCaller(newTimeoutId);
		}
	});
};

export { TimeoutBasedPromise, AllowOperationByTimeoutPromise };