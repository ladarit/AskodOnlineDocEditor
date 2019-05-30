const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const common = require('./webpack.config.js');
const merge = require('webpack-merge');
const CompressionPlugin = require('compression-webpack-plugin');
const webpack = require('webpack');
var BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
var WebpackAutoInject = require('webpack-auto-inject-version');

module.exports = common.map(serverConfig => {
	return merge(serverConfig, {
		mode: 'production',
	plugins: [
		new WebpackAutoInject({
			PACKAGE_JSON_PATH: './package.json',
			components: {
				AutoIncreaseVersion: false,
				InjectAsComment: true
			},
			componentsOptions: {
				InjectAsComment: {
					tag: 'Build {version} - {date}',
					dateFormat: 'mmmm dS, yyyy, HH:MM:ss',
					multiLineCommentType: false
				}
		}}),
		new CompressionPlugin({
			asset: '[path].gz[query]',
			algorithm: 'gzip',
			test: /\.(js|html)$/,
			threshold: 10240,
			minRatio: 0.8
		}),
		new webpack.optimize.AggressiveMergingPlugin(),
		new BundleAnalyzerPlugin(
			//     {
			// generateStatsFile: true,
			// statsFilename: 'bundleStats'
			// }
		)
	],
	optimization: {
		minimizer: [
			new UglifyJsPlugin({
				parallel: true
			}),
			new OptimizeCSSAssetsPlugin({})
		]
	}
	})
});