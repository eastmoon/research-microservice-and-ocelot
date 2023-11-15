@rem Test method

echo ^> Auth GET Method for default username
curl -s --request GET "http://localhost:5001/auth"

echo.
echo.

echo ^> Auth POST Method for custom username "customuser"
curl -s --request POST -d '{}' "http://localhost:5001/auth/token/customuser" > .token
more .token

echo.
echo.
