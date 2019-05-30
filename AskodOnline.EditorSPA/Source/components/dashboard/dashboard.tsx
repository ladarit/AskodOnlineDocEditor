import * as React from 'react';
import withStyles from '@material-ui/core/styles/withStyles';
import * as importedStyles from '../../css/dashboard.scss';
import Button from '@material-ui/core/Button';
import DnsOutlined from '@material-ui/icons/DnsOutlined';
import classNames from 'classnames';
import DbInspector from './dbInspector';

const styles: any = (theme: any) => ({
	root: {
	  ...theme.mixins.gutters(),
	  paddingTop: theme.spacing.unit * 2,
	  paddingBottom: theme.spacing.unit * 2
	},
	main: {
		width: 'auto',
		display: 'block', // Fix IE 11 issue.
		marginLeft: theme.spacing.unit * 3,
		marginRight: theme.spacing.unit * 3,
		[theme.breakpoints.up(600 + theme.spacing.unit * 3 * 2)]: {
		  width: 600,
		  marginLeft: 'auto',
		  marginRight: 'auto'
		}
	  },
	  paper: {
		display: 'flex',
		flexDirection: 'column',
		alignItems: 'center',
		padding: `${theme.spacing.unit * 2}px ${theme.spacing.unit * 3}px ${theme.spacing.unit * 3}px`,
		borderRadius: '0px'
	  },
	  avatar: {
		margin: theme.spacing.unit,
		backgroundColor: theme.palette.secondary.main
	  },
	  form: {
		width: '100%', // Fix IE 11 issue.
		marginTop: theme.spacing.unit
	  },
	  submit: {
		marginTop: theme.spacing.unit * 3
	  },
	  button: {
		width: '100%',
		color: 'white',
		backgroundColor: 'transparent'
	  },
	  rightIcon: {
		marginLeft: theme.spacing.unit
	  },
	  iconSmall: {
		fontSize: 20
	  }
  });

enum CategoryEnum {
	Database
}
interface IDashboardProps {
	classes: any;
}

interface IDashboardState {
	category: CategoryEnum;
}

class Dashboard extends React.Component<IDashboardProps, IDashboardState> {

	constructor(props: IDashboardProps) {
		super(props);
		this.classes = props.classes;
	}

	public state: Readonly<IDashboardState> = {
		category: CategoryEnum.Database
	};

	private classes: any;

	private getSelectedStyle = (category: CategoryEnum) => {
		return this.state.category === category ? importedStyles.dashboard_side_panel_selected_button : importedStyles.dashboard_side_panel_button;
	}

	public render(): JSX.Element {
		return (
			<div className={importedStyles.dashboard_container}>
				<div className={importedStyles.dashboard_side_panel}>
					<div className={importedStyles.dashboard_side_panel_catregory}>
						<span>Категории</span>
					</div>
					<Button size='small' className={this.getSelectedStyle(CategoryEnum.Database)}>
						<span>База данных</span>
						<DnsOutlined className={classNames(this.classes.rightIcon, this.classes.iconSmall)} />
					</Button>
				</div>
				<div className={importedStyles.dashboard_components}>
					<div className={importedStyles.dashboard_components_header}>
						Администрирование
					</div>
					<div className={importedStyles.dashboard_components_container}>
						{this.state.category === CategoryEnum.Database && <DbInspector/>}
					</div>
				</div>
			</div>
		);
	}
}

export default  withStyles(styles)(Dashboard);