const common = require('./webpack.config.js');
const merge = require('webpack-merge');

module.exports = common.map(serverConfig => {
	return merge(serverConfig, {
		mode: 'development',
		devtool: 'eval-source-map'	
	})
});