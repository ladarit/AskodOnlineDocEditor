import 'core-js/fn/promise'; // for IE 11 promise support
import * as React from 'react';
import { render } from 'react-dom';
import RichEditStore from './store/document/RichEditStore';
import SpreadSheetStore from './store/document/SpreadSheetStore';
import SignalRManager from './store/signalR/signalRStore';
import UserClaimsManager from './store/userClaimsManager';
import DocumentStore from './store/document/DocumentStore';
import CultureStore from './store/CultureStore';
import { store } from './store/redux/reduxStore';
import { Provider } from 'react-redux';
import window from './interfaces/IWindow';
import AdministrationStore from './store/administration/AdministrationStore';
import AppsContainer from './containers/AppsContainer';
import WebConfigStore from './store/webConfigStore';

CultureStore.initializeCulture();

const Administration: AdministrationStore = new AdministrationStore();

// use for Material-UI typography v2
window.__MUI_USE_NEXT_TYPOGRAPHY_VARIANTS__ = true;

render(
	<Provider store={store}>
		<AppsContainer/>
	</Provider>, document.getElementById('ReactApp')
);

export {
	DocumentStore,
	RichEditStore,
	SpreadSheetStore,
	SignalRManager,
	UserClaimsManager,
	WebConfigStore,
	Administration
};