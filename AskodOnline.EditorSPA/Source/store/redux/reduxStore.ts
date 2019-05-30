import { createStore } from 'redux';
import cloneDeep from 'lodash-es/cloneDeep';
import remove from 'lodash-es/remove';
import find from 'lodash-es/find';
import isNil from 'lodash-es/isNil';
import merge from 'lodash-es/merge';
import UserInfo from '../../components/UserView/UserInfo';
import IUserInfo from '../../components/UserView/IUserInfo';
import IMessage from '../../components/notification/IMessage';
import IsAuthenticated from '../authentication/IsAuthenticated';

declare var window: any;

export interface IReducerAction {
	type: string;
	payload?: any;
}

export interface IWebConfigVariable {
	name: string;
	value: number | string;
}

export interface IReduxStoreState {
	documentId: string;
	teamworkId: number;
	activeUsers: IUserInfo[];
	messageQueue: IMessage[];
	showOverlay: boolean;
	startVisiblePageIndex: number;
	pagesCount: number;
	adminPageVisible: boolean;
	adminAuthenticated: boolean;
	webConfigVariables: IWebConfigVariable[];
}

const initialState: IReduxStoreState = {
	documentId: undefined,
	teamworkId: undefined,
	activeUsers: [],
	messageQueue: [],
	showOverlay: false,
	startVisiblePageIndex: undefined,
	pagesCount: undefined,
	adminPageVisible: false,
	adminAuthenticated: IsAuthenticated(),
	webConfigVariables: []
};

const documentEditorReducer = (state: IReduxStoreState = initialState, action: IReducerAction) => {
	switch (action.type) {
		case 'SET_DOCUMENT_ID': {
			let docId = cloneDeep(state.documentId);
			docId = action.payload as string;
			return { ...state, documentId: docId };
		}
		case 'SET_TEAMWORK_ID': {
			let teamworkId = cloneDeep(state.teamworkId);
			teamworkId = action.payload as number;
			return { ...state, teamworkId };
		}
		// work with users actions
		case 'ADD_USER': {
			const users = cloneDeep(state.activeUsers);
			const newUserObj = action.payload as IUserInfo;
			const newUser = new UserInfo(newUserObj.Id, newUserObj.Avatar, newUserObj.Name, newUserObj.isTyping);
			users.push(newUser);
			return { ...state, activeUsers: users };
		}
		case 'DELETE_USER': {
			const users = cloneDeep(state.activeUsers);
			const userId = action.payload;
			remove(users, (user) => user.Id === userId);
			return { ...state, activeUsers: users };
		}
		case 'DELETE_ALL_USERS': {
			let users = cloneDeep(state.activeUsers);
			users = [];
			return { ...state, activeUsers: users };
		}
		// work with typing animation actions
		case 'ENABLE_USER_TYPING_ANIMATION': {
			const users = cloneDeep(state.activeUsers);
			const changedUser = find(users, (user: IUserInfo) => user.Id === action.payload);
			if (!isNil(changedUser)) {
				changedUser.isTyping = true;
			}
			merge(users, changedUser);
			return { ...state, activeUsers: users };
		}
		case 'DISABLE_USER_TYPING_ANIMATION': {
			const users = cloneDeep(state.activeUsers);
			const changedUser = find(users, (user: IUserInfo) => user.Id === action.payload);
			if (!isNil(changedUser)) {
				changedUser.isTyping = false;
			}
			merge(users, changedUser);
			return { ...state, activeUsers: users };
		}
		// message queue actions
		case 'ADD_MESSAGE': {
			const messages = cloneDeep(state.messageQueue);
			const newMessage = action.payload as IMessage;
			messages.push(newMessage);
			return { ...state, messageQueue: messages };
		}
		case 'DELETE_MESSAGE': {
			const messages = cloneDeep(state.messageQueue);
			const messageId = action.payload;
			remove(messages, (message) => message.id === messageId);
			return { ...state, messageQueue: messages };
		}
		// work with block UI actions
		case 'SHOW_OVERLAY': {
			const showOverlayValue = action.payload as boolean;
			return { ...state, showOverlay: showOverlayValue };
		}
		// work with pages count actions
		case 'SET_START_PAGE_INDEX': {
			const value = action.payload as number + 1;
			return { ...state, startVisiblePageIndex: value };
		}
		case 'SET_TOTAL_PAGES_COUNT': {
			const value = action.payload as number;
			return { ...state, pagesCount: value };
		}
		// work with admin page
		case 'SET_ADMIN_PAGE_VISIBLE': {
			const value = action.payload as boolean;
			return { ...state, adminPageVisible: value };
		}
		case 'SET_AUTHENTICATED_STATE': {
			const value = action.payload as boolean;
			return { ...state, adminAuthenticated: value };
		}
		// work with webConfig variables
		case 'ADD_WEB_CONFIG_VARIABLE': {
			const variable = action.payload as IWebConfigVariable;
			const webConfigVariables = cloneDeep(state.webConfigVariables);
			if (find(webConfigVariables, (v) => v.name === variable.name)) {
				remove(webConfigVariables, (v) => v.name === variable.name);
			}
			webConfigVariables.push(variable);
			return { ...state, webConfigVariables };
		}
		default:
			return state;
	}
};

const store = createStore(
	documentEditorReducer,
	window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()
);

export { store };