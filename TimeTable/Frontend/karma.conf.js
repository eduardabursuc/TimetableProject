module.exports = function(config) {
    console.log('Using karma.conf.js file');
    config.set({
      frameworks: ['jasmine', '@angular-devkit/build-angular'],
      plugins: [
        require('karma-jasmine'),
        require('karma-chrome-launcher'),
        require('karma-coverage'),
        require('karma-jasmine-html-reporter'),
        require('@angular-devkit/build-angular/plugins/karma')
      ],
      client: {
        clearContext: false 
      },
      coverageReporter: {
        dir: require('path').join(__dirname, './coverage/lcov-info'),
        subdir: '.',
        reporters: [
          { type: 'html' },
          { type: 'lcovonly' },
          { type: 'text-summary' }
        ]
      },
      reporters: ['progress', 'kjhtml', 'coverage'],
      port: 9876,
      colors: true,
      logLevel: config.LOG_INFO,
      autoWatch: true,
      browsers: ['Chrome'],
      singleRun: false,
      restartOnFileChange: true,
      files: [
        'src/**/*.spec.ts',  // Include all .spec.ts files
      ],
    });
  };
  