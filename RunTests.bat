CD .\artifacts\bin\King.Azure.Unit.Test\Release\app

dnvm use 1.0.0-rc1-update1 -r clr -a x64
dnu restore -f ../../../King.Azure/Release
./King.Azure.Unit.Test.cmd

CD ../../../../../