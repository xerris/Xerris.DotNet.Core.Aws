const gulp = require('gulp');
const zip = require('gulp-zip');
const del = require('del');
const mkdirp = require('mkdirp');
const path = require('path');
const { exec } = require('child_process');
const {clean, restore, build, test, pack, publish, run} = require('gulp-dotnet-cli');
const version = `0.0.` + (process.env.BUILD_NUMBER || '0' + '-prerelease');
const configuration = process.env.BUILD_CONFIGURATION || 'Debug';
const targetProject = 'src/Xerris.AWS.Hello/Xerris.AWS.Hello.csproj';
const runProject = 'src/Xerris.AWS.WebApi/Xerris.AWS.WebApi.csproj';
const cdkProject = 'src/infrastructure';

function _clean() {
    return gulp.src('*.sln', {read: false})
        .pipe(clean());
}

function _restore () {
    return gulp.src('*.sln', {read: false})
        .pipe(restore());
}

function _build() {
    return gulp.src('*.sln', {read: false})
        .pipe(build({configuration: configuration, version: version}));
}

function _test() {
    return gulp.src('**/*Tests.csproj', {read: false})
        .pipe(test({logger: `junit;LogFileName=${__dirname}/TestResults/xunit/TestOutput.xml`}))
}

function _distDir() {
    return new Promise((resolve, error) => {
        del(['dist'], {force: true}).then(
            () => { mkdirp('dist', resolve);
            });
    });
}

function _publish() {
    return gulp.src(targetProject, {read: false})
        .pipe(publish({
            configuration: configuration, version: version,
            output: path.join(process.cwd(), 'dist'),
        }));
}

function _publishMigration() {
    return gulp.src(migrationProject, {read: false})
        .pipe(publish({
            configuration: configuration, version: version,
            output: path.join(process.cwd(), 'migrations'),
        }));
}

function _run() {
    return gulp.src(runProject, {read: false})
        .pipe(run());
}

function _package() {
    return gulp.src('dist/**/*')
        .pipe(zip('LambdaPackage.zip'))
        .pipe(gulp.dest('dist'));
}

function _deploy() {
    return exec('npm run deploy', {cwd: cdkProject},(error, stdout, stderr) => {
        if (error) {
            console.error(`exec error: ${error}`);
            return;
        }
        console.log(`stdout: ${stdout}`);

        if(stderr)
            console.error(`stderr: ${stderr}`);
    });
}

exports.Build = gulp.series(_clean, _restore, _build);
exports.Test = gulp.series(_clean, _restore, _build, _test);
exports.Default = gulp.series(_clean, _restore, _build, _test);
exports.Publish = gulp.series(_distDir, _clean, _build, _publish);
exports.Run = gulp.series(_distDir, _clean, _build, _run);
exports.Package = gulp.series(_distDir, _clean, _build, _publish, _package);
exports.Deploy = gulp.series(_distDir, _clean, _build, _test, _publish, _package, _deploy);
exports.PublishMigrations = gulp.series(_distDir, _clean, _build, _publishMigration);
