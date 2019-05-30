import axios, { AxiosError } from 'axios';
import AuthStore from '../authentication/AuthStore';
import TokenStore from '../authentication/TokenStore';

const axiosInstance = axios.create({
	baseURL: window.location.origin,
	method: 'get',
	headers: {
		'Content-Type': 'application/json'
	}
});

axiosInstance.interceptors.request.use(
	(request) => {
		request.headers.Authorization = TokenStore.getJwtToken();
		return request;
	},
	(failReason: AxiosError) => {
		return Promise.reject(failReason);
	}
);

axiosInstance.interceptors.response.use(
	(response) => {
		return response;
	},
	async (failReason: AxiosError) => {
		if (failReason.response.status === 401) {
			if (failReason.response.statusText === 'Token-Expired') {
				const refreshPayload = {
					Token: TokenStore.getJwtToken(),
					RefreshToken: TokenStore.getRefreshToken()
				};
				const response = await axiosInstance.post('/api/refreshtoken', refreshPayload);
				TokenStore.saveJwtToken('Bearer ' + response.data.Token);
				TokenStore.saveRefreshToken(response.data.RefreshToken);
				return await axiosInstance.request(failReason.config);
			} else {
				TokenStore.saveJwtToken('');
				TokenStore.saveRefreshToken('');
				AuthStore.SetAuthenticatedState(false);
				return Promise.reject(failReason);
			}
		}
	}
);

export default axiosInstance;