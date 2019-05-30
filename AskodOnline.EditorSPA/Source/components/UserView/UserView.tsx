import * as React from 'react';
import * as styles from '../../css/usersList.scss';
import IUserInfo from './IUserInfo';

interface IUserViewProps {
	user: IUserInfo;
}

const UserView = (props: IUserViewProps) => {
	return (
		<div className={styles.user_record} id={props.user.Id}>
			<span className={styles.img_text_wrapper}>
				<img className={styles.avatar} src={props.user.Avatar}/>
			</span>
			<p>
				<b>{props.user.Name}</b>
				{props.user.isTyping &&
					<img src='/Content/Images/typingAnimation.gif'/>
				}
			</p>
		</div>
	);
};

export default UserView;
