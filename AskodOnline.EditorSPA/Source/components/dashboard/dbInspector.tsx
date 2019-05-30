import * as React from 'react';
import withStyles from '@material-ui/core/styles/withStyles';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import * as importedStyles from '../../css/dashboard.scss';
import Button from '@material-ui/core/Button';
import HelpRounded from '@material-ui/icons/HelpRounded';
import CheckCircleRounded from '@material-ui/icons/CheckCircleRounded';
import RemoveCircleRounded from '@material-ui/icons/RemoveCircleRounded';
import axios from '../../store/axios/axiosInstance';

const styles: any = (theme: any) => ({
  paper: {
		display: 'flex',
		flexDirection: 'column',
		alignItems: 'center',
		padding: `${theme.spacing.unit * 2}px ${theme.spacing.unit * 3}px ${theme.spacing.unit * 3}px`,
		borderRadius: '0px'
	  }
  }
);

enum ConnectionStateEnum {
	successful,
	unSuccessful,
	unknown
}
interface IDbInspectorProps {
	classes: any;
}

interface IDbInspectorState {
	connectionState: ConnectionStateEnum;
	testFinished: boolean;
}

class DbInspector extends React.Component<IDbInspectorProps, IDbInspectorState> {

	constructor(props: IDbInspectorProps) {
		super(props);
		this.classes = props.classes;
	}

	public state: Readonly<IDbInspectorState> = {
		connectionState: ConnectionStateEnum.unknown,
		testFinished: true
	};

	private classes: any;

	private handleClick = async () => {
		const root = this;
		this.setState({ testFinished: false }, async () => {
			try {
				const response = await axios.get('/api/dashboard/dbInspector');
				const connectionCanBeOpened = response.data.ConnectionCanBeOpened ? ConnectionStateEnum.successful : ConnectionStateEnum.unSuccessful;
				root.setState({ connectionState: connectionCanBeOpened, testFinished: true });
			} catch (failReason) {
				console.log(failReason.response.data.Message);
				if (failReason.response.status !== 401) {
					root.setState({ connectionState: ConnectionStateEnum.unknown, testFinished: true });
				}
			}
		});
	}

	public render(): JSX.Element {
		let connectionSuccessful = null;
		let connectionUnSuccessful = null;
		let connectionUnknown = null;
		let connectionTesting = <img src='/Content/Images/Gear-2.7s-200px.gif'/>;
		if (this.state.testFinished) {
			connectionSuccessful = this.state.connectionState === ConnectionStateEnum.successful ? <CheckCircleRounded className={importedStyles.dashboard_icon_checkcircle}/> : null;
			connectionUnSuccessful = this.state.connectionState === ConnectionStateEnum.unSuccessful ? <RemoveCircleRounded className={importedStyles.dashboard_icon_removecircle}/> : null;
			connectionUnknown = this.state.connectionState === ConnectionStateEnum.unknown ? <HelpRounded/> : null;
			connectionTesting = null;
		}
		return (
			<Paper className={this.classes.paper + ' ' + importedStyles.dashboard_component}>
				<Typography component='p'>
					соединение с БД
				</Typography>
				{connectionSuccessful}
				{connectionUnSuccessful}
				{connectionUnknown}
				{connectionTesting}
				<Button onClick={this.handleClick} variant='contained' color='primary'>
					Проверить
				</Button>
			</Paper>
		);
	}
}

export default withStyles(styles)(DbInspector);