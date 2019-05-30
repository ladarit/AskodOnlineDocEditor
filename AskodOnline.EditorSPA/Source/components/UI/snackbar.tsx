import * as React from 'react';
import Snackbar from '@material-ui/core/Snackbar';
import IconButton from '@material-ui/core/IconButton';
import withStyles from '@material-ui/core/styles/withStyles';
import CloseIcon from '@material-ui/icons/Close';
import IMessage from '../notification/IMessage';
import * as styles from '../../css/snackbar.scss';

const overridenClasses = (theme: any) => ({
	close: {
		padding: theme.spacing.unit / 2
	}
});

interface ISnackbarProps {
	message: IMessage;
	open: boolean;
	handleClose: (e: any, reason: string) => void;
}

const snackbar = (props: ISnackbarProps) => {
	const snackbarReverseClass = props.message.content.length > 75 ? styles.snackbar_reverse : null;

	return (
		<Snackbar
			anchorOrigin={{
				vertical: props.message.position ? props.message.position.vertical : 'bottom',
				horizontal: props.message.position ? props.message.position.horizontal : 'left'
			}}
			className={[styles.snackbar_container, snackbarReverseClass].join(' ')}
			open={props.open}
			autoHideDuration={6000}
			onClose={(e) => props.handleClose(e, 'timeOut')}
			ContentProps={{ 'aria-describedby': props.message.id }}
			message={<span id={props.message.id}>{props.message.content}</span>}
			action={
				<IconButton
					key={props.message.id + '_close'}
					aria-label='Close'
					color='inherit'
					onClick={(e) => props.handleClose(e, 'close')}
				>
					<CloseIcon style={{ color: 'white' }} />
				</IconButton>
			}
		/>
	);
};

export default withStyles(overridenClasses)(snackbar);