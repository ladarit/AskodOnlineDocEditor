export default class CookieStore {
	public static GetCookie(name: string) {
		const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
		if (match) {
			return match[2];
		}
		return null;
	}
}