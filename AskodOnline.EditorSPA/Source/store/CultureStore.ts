import CookieStore from './CookieStore';
import i18next from 'i18next';
import isNil from 'lodash-es/isNil';
import isEqual from 'lodash-es/isEqual';

export default class CultureStore {
	public static initializeCulture() {
		let culture: string = CookieStore.GetCookie('AODECulture');
		culture = !isNil(culture) ? (isEqual(culture, 'ru-RU') ? 'ru-RU' : isEqual(culture, 'uk-UA') ? 'uk-UA' : 'en-US' )  : 'uk-UA';
		i18next.init({
			lng: culture,
			debug: true,
			resources: require(`../internationalization/${culture}.json`)
			}, (err, t) => {
				console.log('Current culture: ' + culture);
			});
		}
}