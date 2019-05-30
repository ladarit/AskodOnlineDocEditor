const path = require('path');
var WebpackNotifierPlugin = require('webpack-notifier');
var FriendlyErrorsWebpackPlugin = require('friendly-errors-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const webpack = require('webpack');
// const WebpackCleanupPlugin = require('webpack-cleanup-plugin');
var LodashModuleReplacementPlugin = require('lodash-webpack-plugin');
const production = process.env.NODE_ENV === 'production';
const cssFolderPath = path.join(__dirname, './Source/css');
const StyleLintPlugin = require('stylelint-webpack-plugin');

module.exports = [
	{
		entry: {
			index: './Source/index.tsx'
		},
		output: {
			filename: '[name].js',
			path: path.resolve(__dirname, '../AskodOnline.Editor/Scripts/build'),
			libraryTarget: 'var',
			library: ['Global'],
		},
		resolve: {
			extensions: ['.js', '.jsx', '.ts', '.tsx'],
			modules: ['Scripts', 'node_modules'],
			alias: {
				root: path.resolve(__dirname, './'),
			}
		},
		module: {
			rules: [
				// js files
				{
					test: /\.js$/,
					exclude: /(node_modules)/,
					use: {
						loader: 'babel-loader',
						options: {
							presets: ['env', 'react', 'stage-0']
						}
					}
				},
				// ts | tsx files
				{
					test: /\.(ts|tsx)$/,
					enforce: 'pre',
					use: [
								{
									loader: 'tslint-loader',
									options: {
										emitErrors: false,
										failOnHint: true
									}
								}
					]
				},
				{
					test: /\.(ts|tsx)$/,
					exclude: /(node_modules)/,
					loader: ['babel-loader', 'ts-loader']
				},
				// sass|scss|css files
				{
					test:  /\.(sa|sc|c)ss$/,
					include: cssFolderPath,
					use: [
						MiniCssExtractPlugin.loader,
						{
							loader: 'typings-for-css-modules-loader',
							options: {
								modules: true,
								namedExport: true,
								importLoaders: 1,
								modules: true,
								namedExport: true,
								localIdentName: production ? '[name]_[local]_[hash:base64:5]' : '[local]'
							}
						},
						{ loader: "sass-loader" }
					]
				}
			]
		},
		optimization: {
			splitChunks: {
				cacheGroups: {
					vendor: {
						test: /[\\/]node_modules[\\/]/,
						name: 'vendors',
						chunks: 'all'
					}
				}
			}
		},
		// optimization: {
		// 	//runtimeChunk: 'single',
		// 	splitChunks: {
		// 	  chunks: 'all',
		// 	  maxInitialRequests: Infinity,
		// 	  minSize: 0,
		// 	  cacheGroups: {
		// 		vendor: {
		// 		  test: /[\\/]node_modules[\\/]/,
		// 		  name(module) {
		// 			// get the name. E.g. node_modules/packageName/not/this/part.js
		// 			// or node_modules/packageName
		// 			const packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1];

		// 			// npm package names are URL-safe, but some servers don't like @ symbols
		// 			return `npm.${packageName.replace('@', '')}`;
		// 		  },
		// 		},
		// 	  },
		// 	},
		// },
		plugins: [
			new StyleLintPlugin({
				emitErrors: false,
				failOnError: true,
				syntax: "sass"|"scss"|"css"
			}),
			new LodashModuleReplacementPlugin,
			new MiniCssExtractPlugin({filename:  "../../Content/SPA_bundle.css" }),
			new WebpackNotifierPlugin({alwaysNotify: true}),
			new FriendlyErrorsWebpackPlugin(),
			new webpack.HashedModuleIdsPlugin()
		]
	},
	{
		entry: {
			pingFileEditingWorker: './Source/workers/pingFileEditingWorker.ts'
		},
		output: {
			filename: '[name].js',
			path: path.resolve(__dirname, '../AskodOnline.Editor/Scripts/build'),
		},
		resolve: {
			extensions: ['.js','.ts'],
			modules: [path.resolve(__dirname, './Source/workers'), 'node_modules'],
		},
		module: {
			rules: [
				// js files
				{
					test: /\.js$/,
					exclude: /(node_modules)/,
					use: {
						loader: 'babel-loader',
						options: {
							presets: ['env', 'react', 'stage-0']
						}
					}
				},
				// ts | tsx files
				{
					test: /\.(ts|tsx)$/,
					enforce: 'pre',
					use: [
						{
							loader: 'tslint-loader',
							options: {
								emitErrors: false,
								failOnHint: true
							}
						}
					]
				},
				{
					test: /\.(ts|tsx)$/,
					exclude: /(node_modules)/,
					loader: ['babel-loader', 'ts-loader']
				}				
			]
		},
		plugins: [
			new LodashModuleReplacementPlugin,
			new FriendlyErrorsWebpackPlugin(),
			// new WebpackCleanupPlugin()
		]
	}
];
