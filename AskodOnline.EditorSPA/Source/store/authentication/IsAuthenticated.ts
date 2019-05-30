import isEmpty from 'lodash-es/isEmpty';

const IsAuthenticated = (): boolean => {
	const accessToken = localStorage.getItem('access_token');
	const refreshToken = localStorage.getItem('refresh_token');
	return !isEmpty(accessToken) && !isEmpty(refreshToken);
};

export default IsAuthenticated;