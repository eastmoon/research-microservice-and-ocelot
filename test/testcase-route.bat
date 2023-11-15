echo ^> Check Ocelot server at localhost:5000, it will not give any response
curl http://localhost:5000/
echo.

echo ^> Check Core server at localhost:5002
curl http://localhost:5002/WeatherForecast
echo.

echo ^> Check Utils server at localhost:5003
curl http://localhost:5003/WeatherForecast
echo.

echo ^> Check Core server at localhost:5000/core
curl http://localhost:5000/core
echo.

echo ^> Check Utils server at localhost:5000/utils
curl http://localhost:5000/utils
echo.

echo ^> Check Core server at localhost:5050/core
curl http://localhost:5050/core
echo.

echo ^> Check Utils server at localhost:5050/utils
curl http://localhost:5050/utils
echo.
