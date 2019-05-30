import { IReducerAction, IWebConfigVariable } from './reduxStore';
import IUserInfo from '../../components/UserView/IUserInfo';
import IMessage from '../../components/notification/IMessage';

export const SetDocumentId = (documentId: string): IReducerAction => {
	return {
		type: 'SET_DOCUMENT_ID',
		payload: documentId
	  };
};

export const SetTeamworkId = (teamworkId: number): IReducerAction => {
	return {
		type: 'SET_TEAMWORK_ID',
		payload: teamworkId
	  };
};

export const AddUser = (user: IUserInfo): IReducerAction => {
	return {
		type: 'ADD_USER',
		payload: user
	  };
};

export const DeleteUser = (userId: string): IReducerAction => {
	return {
		type: 'DELETE_USER',
		payload: userId
	  };
};

export const DeleteAllUsers = (): IReducerAction => {
	return {
		type: 'DELETE_ALL_USERS'
	  };
};

export const EnableUserTypingAnimation = (userId: string): IReducerAction => {
	return {
		type: 'ENABLE_USER_TYPING_ANIMATION',
		payload: userId
	  };
};

export const DisableUserTypingAnimation = (userId: string): IReducerAction => {
	return {
		type: 'DISABLE_USER_TYPING_ANIMATION',
		payload: userId
	  };
};

export const AddMessageToQueue = (message: IMessage): IReducerAction => {
	return {
		type: 'ADD_MESSAGE',
		payload: message
	  };
};

export const DeleteMessageFromQueue = (messageId: string): IReducerAction => {
	return {
		type: 'DELETE_MESSAGE',
		payload: messageId
	  };
};

export const ShowOverlay = (isShow: boolean): IReducerAction => {
	return {
		type: 'SHOW_OVERLAY',
		payload: isShow
	  };
};

export const setStartPageIndex = (pageIndex: number): IReducerAction => {
	return {
		type: 'SET_START_PAGE_INDEX',
		payload: pageIndex
	  };
};

export const setTotalPagesCount = (pagesCount: number): IReducerAction => {
	return {
		type: 'SET_TOTAL_PAGES_COUNT',
		payload: pagesCount
	  };
};

export const setAdminPageVisible = (visible: boolean): IReducerAction => {
  return {
	  type: 'SET_ADMIN_PAGE_VISIBLE',
	  payload: visible
	};
};

export const setAuthenticatedState = (value: boolean): IReducerAction => {
	return {
		type: 'SET_AUTHENTICATED_STATE',
		payload: value
	};
};

export const AddWebConfigVariable = (variable: IWebConfigVariable): IReducerAction => {
	return {
		type: 'ADD_WEB_CONFIG_VARIABLE',
		payload: variable
	  };
};