copy npm-shrinkwrap.json src/npm-shrinkwrap.json /Y
call ng build versions-netcore-angular
call ng build --prod --build-optimizer

(robocopy dist\StankinsAliveAngular ..\StankinsStatusWeb\wwwroot  /MIR /XD) ^& IF %ERRORLEVEL% LSS 8 SET ERRORLEVEL = 0

(robocopy dist\StankinsAliveAngular ..\StankinsCordova\www\dist  /MIR /XD) ^& IF %ERRORLEVEL% LSS 8 SET ERRORLEVEL = 0
