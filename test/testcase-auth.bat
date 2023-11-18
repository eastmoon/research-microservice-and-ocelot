@rem Test method

echo ^> Auth GET Method for default username
curl -s --request GET "http://localhost:5001/auth"
echo.
echo.
echo ^> Auth GET Method for default username using ocelot redirect
curl -s --request GET "http://localhost:5000/auth"
echo.
echo.
echo ^> Auth POST Method for custom username "customuser"
curl -s --request POST -d '{}' "http://localhost:5001/auth/token/customuser" > .token
more .token
echo.
echo ^> Auth POST Method for custom username "customuser" using ocelot redirect
curl -s --request POST -d '{}' "http://localhost:5001/auth/token/customuser" > .token
more .token
echo.
echo.
set /p TOKEN=<.token
echo ^> Auth POST Method for check authorization token
echo ^> No pass token in header
curl -s --request POST -d '{}' "http://localhost:5000/auth/check"
echo.
echo ^> Pass token in header
curl -s --request POST --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/auth/check"
echo.
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/auth/RootRole"
echo.
echo ^> Check Role : Root
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/check/RootRole"
echo.
echo ^> Check UserType : Admin
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/check/AdminRole"
echo.
echo ^> Check UserType : Developer ( It will failed, because token did not add developer roles)
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/check/DeveloperRole"
echo.
echo ^> Check UserType : User
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/check/UserRole"
echo.
echo ^> Check AccessLevel : 1
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/check/Level1"
echo.
echo ^> Check AccessLevel : 2 ( It will failed, because token only have level 1)
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/check/Level2"
echo.
echo ^> Check Role = Admin, AccessLevel = 1
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/check/MultiRule1"
echo.
echo ^> Check Role = Admin, AccessLevel = 2 ( It will failed )
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/check/MultiRule2"
echo.
echo ^> Check Role = Developer, AccessLevel = 1 ( It will failed )
curl -s --request GET --header "Authorization: Bearer %TOKEN%" -d '{}' "http://localhost:5000/check/MultiRule3"
