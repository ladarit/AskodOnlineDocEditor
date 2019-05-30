import find from 'lodash-es/find';
import remove from 'lodash-es/remove';
import cloneDeep from 'lodash-es/cloneDeep';

export enum IWorkerCommand {
	ACTIVATE,
	TERMINATE
}

export enum IWorkerState {
	INITIALIZING,
	ACTIVATED,
	STOPPED,
	ERROR
}

interface IWorkerEntity {
	workerInstanse: Worker;
	scriptPath: string;
	state: IWorkerState;
}

export default class WorkersStore {

	private static workersPool: IWorkerEntity[] = [];

	private getWorkerEntity = (scriptPath: string): IWorkerEntity => {
		return find(WorkersStore.workersPool, (w: IWorkerEntity) => w.scriptPath === scriptPath);
	}

	public startWorker = (scriptPath: string, options: any, onMessageFromWorkerReceived: (...args: any[]) => any): void => {
		const existedWorkerEntity = this.getWorkerEntity(scriptPath);
		if (!existedWorkerEntity) {
			const worker = new Worker(scriptPath);
			worker.onmessage = (messageEvent: any) => {
				if (messageEvent.data === IWorkerState.ACTIVATED) {
					const workerEntity = this.getWorkerEntity(scriptPath);
					if (workerEntity) {
						workerEntity.state = IWorkerState.ACTIVATED;
					}
				}
				onMessageFromWorkerReceived(messageEvent);
			};
			const workerSettings = {
				command: IWorkerCommand.ACTIVATE,
				options: cloneDeep(options)
			};
			worker.postMessage(workerSettings);
			WorkersStore.workersPool.push({ workerInstanse: worker, scriptPath, state: IWorkerState.INITIALIZING });
		}
	}

	public terminateWorker = (scriptPath: string): void => {
		const existedWorkerEntity = this.getWorkerEntity(scriptPath);
		if (existedWorkerEntity) {
			existedWorkerEntity.workerInstanse.terminate();
			remove(WorkersStore.workersPool, (w) => w.scriptPath === existedWorkerEntity.scriptPath);
			console.log(`worker with script ${existedWorkerEntity.scriptPath} was terminated`);
		}
	}
}