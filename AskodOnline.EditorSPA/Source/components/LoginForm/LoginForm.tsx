import * as React from 'react';
import Avatar from '@material-ui/core/Avatar';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import FormControl from '@material-ui/core/FormControl';
import Input from '@material-ui/core/Input';
import InputLabel from '@material-ui/core/InputLabel';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Paper from '@material-ui/core/Paper';
import withStyles from '@material-ui/core/styles/withStyles';
import isNil from 'lodash-es/isNil';
import Typography from '@material-ui/core/Typography';
import TokenStore from '../../store/authentication/TokenStore';
import axios from '../../store/axios/axiosInstance';
import AuthStore from '../../store/authentication/AuthStore';

const styles: any = (theme: any) => ({
  main: {
	width: 'auto',
	display: 'block', // Fix IE 11 issue.
	marginLeft: theme.spacing.unit * 3,
	marginRight: theme.spacing.unit * 3,
	[theme.breakpoints.up(400 + theme.spacing.unit * 3 * 2)]: {
	  width: 400,
	  marginLeft: 'auto',
	  marginRight: 'auto'
	}
  },
  paper: {
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'center',
	padding: `${theme.spacing.unit * 2}px ${theme.spacing.unit * 3}px ${theme.spacing.unit * 3}px`
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
  }
});

interface ILoginFormProps {
	classes: any;
}

interface ILoginFormState {
	loginValue: string;
	passwordValue: string;
	error: string;
}

export class LoginForm extends React.Component<ILoginFormProps, ILoginFormState> {

	constructor(props: ILoginFormProps) {
		super(props);
		this.classes = props.classes;
	}

	public state: Readonly<ILoginFormState> = {
		loginValue: '',
		passwordValue: '',
		error: null
	};

	private classes: any;

	private handleSubmit = async (e: any) => {
		e.preventDefault();
		try {
			const response = await axios.post('/api/token', { username: this.state.loginValue, password: this.state.passwordValue });
			if (response.data) {
				TokenStore.saveJwtToken('Bearer ' + response.data.Token);
				TokenStore.saveRefreshToken(response.data.RefreshToken);
				AuthStore.SetAuthenticatedState(true);
			}
		} catch (failReason) {
			this.setState({ error: failReason.response.data });
		}
	}

	private handleChange = (key: string, value: string) => {
		this.setState({ [key]: value } as Pick<ILoginFormState, keyof ILoginFormState>);
	}

	private handleBlur = (key: string, event: any) => {
		const updatedValue = isNil(event.target.value) ? '' : event.target.value.trim();
		this.setState({ [key]: updatedValue } as Pick<ILoginFormState, keyof ILoginFormState>);
	}

	public render(): JSX.Element {
		return (
			<div className={this.classes.main}>
				<CssBaseline />
				<Paper className={this.classes.paper}>
					<Avatar className={this.classes.avatar}>
						<LockOutlinedIcon />
					</Avatar>
					<form className={this.classes.form} onSubmit={(e) => this.handleSubmit(e)}>
						<FormControl margin='normal' required={true} fullWidth={true}>
							<InputLabel>Логин</InputLabel>
							<Input
								id='login'
								name='login'
								onBlur={(e) => this.handleBlur('loginValue', e)}
								value={this.state.loginValue}
								onChange={(e) => this.handleChange('loginValue', e.target.value)}
								autoFocus={true}
							/>
						</FormControl>
						<FormControl margin='normal' required={true} fullWidth={true}>
							<InputLabel>Пароль</InputLabel>
							<Input
								id='password'
								name='password'
								type='password'
								onBlur={(e) => this.handleBlur('passwordValue', e)}
								value={this.state.passwordValue}
								onChange={(e) => this.handleChange('passwordValue', e.target.value)}
							/>
						</FormControl>
						<Button
							type={'submit'}
							fullWidth={true}
							variant={'contained'}
							color={'primary'}
							className={this.classes.submit}
						>
							Вход
						</Button>
						<h1/>
						{this.state.error &&
							<Typography style={{ textAlign: 'center' }} component='h1' variant='h5'>
								{this.state.error}
							</Typography>
						}
					</form>
			  </Paper>
			</div>
		  );
	}
}

export default withStyles(styles)(LoginForm);