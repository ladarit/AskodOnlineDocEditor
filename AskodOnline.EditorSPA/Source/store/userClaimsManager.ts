interface IUserClaims {
	Counter: number;
}

export default class UserClaimsManager {
	private static userInfo: IUserClaims = undefined;

	static get Claims() {
		return UserClaimsManager.userInfo;
	}

	static set Claims(value: IUserClaims) {
		UserClaimsManager.userInfo = value;
	}
}