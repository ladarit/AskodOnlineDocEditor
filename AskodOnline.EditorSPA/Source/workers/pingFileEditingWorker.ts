import 'core-js/fn/promise'; // for IE 11 promise support
import { IWorkerCommand, IWorkerState } from '../store/worker/workerStore';
import axios, { AxiosError } from 'axios';

const axiosInstance = axios.create({
	baseURL: location.origin,
	method: 'get',
	headers: {
		'Content-Type': 'application/json'
	}
});

axiosInstance.interceptors.request.use(
	(request) => {
		return request;
	},
	(failReason: AxiosError) => {
		return Promise.reject(failReason);
	}
);

// tslint:disable-next-line:only-arrow-functions
onmessage = (messageEvent: any) => {
	let timeOut: any;
	try {
		if (messageEvent.data.command === IWorkerCommand.ACTIVATE) {
			const startApiNotification = () => {
				timeOut = setTimeout(async () => {
					try {
						debugger;
						const requestData = messageEvent.data.options.requestData;
						const response = await axiosInstance.post('/api/pingfileediting', requestData);
						if (response.statusText.toLowerCase() === 'ok') {
							postMessage(response.data, messageEvent.currentTarget);
							startApiNotification();
						}
					} catch (failReason) {
						clearTimeout(timeOut);
						console.log('ERROR IN SERVERWORKER');
						console.log(failReason.response.data);
						postMessage(IWorkerState.ERROR, messageEvent.currentTarget);
					}
				}, messageEvent.data.options.requestData.interval);
			};
			startApiNotification();
			postMessage(IWorkerState.ACTIVATED, messageEvent.currentTarget);
		}
	} catch (failReason) {
		clearTimeout(timeOut);
		console.log('ERROR IN SERVERWORKER');
		console.log(failReason);
		postMessage(IWorkerState.ERROR, messageEvent.currentTarget);
	}
};