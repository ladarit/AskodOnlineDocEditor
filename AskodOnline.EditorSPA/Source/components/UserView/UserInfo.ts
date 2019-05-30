import IUserInfo from './IUserInfo';

export default class UserInfo implements IUserInfo {
	constructor(id: string, avatar: string, name: string, isTyping?: boolean) {
		this.Id = id;
		this.Avatar = avatar;
		this.Name = name;
		this.isTyping = isTyping ? isTyping : false;
	}

	public Id: string;
	public Avatar: string;
	public Name: string;
	public isTyping: boolean;
}