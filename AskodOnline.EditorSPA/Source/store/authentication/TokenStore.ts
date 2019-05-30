export default class TokenStore {

	public static getJwtToken = () => {
		return localStorage.getItem('access_token');
	}

	public static getRefreshToken = () => {
		return localStorage.getItem('refresh_token');
	}

	public static saveJwtToken = (token: string) => {
		localStorage.setItem('access_token', token);
	}

	public static saveRefreshToken = (refreshToken: string) => {
		localStorage.setItem('refresh_token', refreshToken);
	}
}