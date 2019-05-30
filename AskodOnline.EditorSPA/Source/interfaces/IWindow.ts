interface IWindow {
	[key: string]: any; // Add index signature
}

declare var window: IWindow;

export default window;