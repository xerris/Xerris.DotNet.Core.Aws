const gulp = require('gulp');
const del = require('del');
const mkdirp = require('mkdirp');
const path = require('path');
const fs = require('fs');
const { exec } = require('child_process');
const {clean, restore, build, test, pack, publish, run} = require('gulp-dotnet-cli');
const configuration = process.env.BUILD_CONFIGURATION || 'Debug';
const targetProject = 'src/Xerris.DotNet.Core.Aws/Xerris.DotNet.Core.Aws.csproj';
//Nuget specific settings
const packageName = 'Xerris.DotNet.Core.Aws';
const nugetApiUrl = '-s https://api.nuget.org/v3/index.json';
const versionFile = './version.json';
const release = '-prerelease';
let version = `0.0.1${release}`;

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
    return gulp.src('**/*Test.csproj', {read: false})
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

function _package() {
    return gulp.src(targetProject)
            .pipe(pack({
                output: path.join(process.cwd(), 'dist') ,
                version: version
            }));
}

async function _version() {
    fs.readFile(versionFile, (err, data) => {
        version = data.toString().split('\n')[0].substring(1);
        console.log(`new version: ${version}`);
    });
}

async function _push() {
    console.log(`pushing to nuget for verion: ${version}`);
    var cmd = `dotnet nuget push ./dist/${packageName}.${version}.nupkg`;
    var nugetApi = `-k ${process.env.NUGET_APIKEY}`
    var execCmd = `${cmd} ${nugetApiUrl} ${nugetApi}`
    console.log(execCmd);
    exec(execCmd).on('exit', () => {
        console.log(`pushed to nuget for verion: ${version}`);
    });
}

exports.Build = gulp.series(_clean, _restore, _build);
exports.Test = gulp.series(_clean, _restore, _build, _test);
exports.Default = gulp.series(_clean, _restore, _build, _test);
exports.Publish = gulp.series(_distDir, _clean, _build, _publish);
exports.Package = gulp.series(_distDir, _clean, _build, _publish, _package);
exports.Push    = gulp.series(_distDir, _clean, _version, _build, _test, _publish, _package, _push);
