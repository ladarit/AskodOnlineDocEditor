import * as React from 'react';
import Admin from './Admin';
import DocumentsEditor from './DocumentsEditor';

class AppsContainer extends React.Component {

	public render(): JSX.Element {
		const isAdminPage = location.pathname.toLowerCase() === '/admin';
		return (
			<React.Fragment>
				{isAdminPage &&
					<Admin/>
				}
				{!isAdminPage &&
					<DocumentsEditor/>
				}
			</React.Fragment>
		);
	}
}

export default AppsContainer;