@echo off

SET FILES=MarshalingTests InheritanceTests FieldTests ManglingTests
SET CLASSES=HasField NumberClass AdderClass AdderClassWithVirtualBase MultiplierClass MultiplierClassWithVirtualBase ClassWithNonVirtualBases ClassWithVirtualBases ClassThatOverridesStuff ClassThatRoundtrips Compression Namespaced Namespaced2 ClassWithCopyCtor ClassWithDtor ClassWithoutCopyCtor Class
SET top_srcdir=..\
SET BUILD_DIR=%top_srcdir%bin\Debug\
SET INTEROP_DLL=%BUILD_DIR%Mono.Cxxi.dll
SET TEST_DLL=%BUILD_DIR%Test.dll

SET NUNIT_DIR=%top_srcdir%packages\NUnit.2.5.10.11092\
SET NUNIT_FRAMEWORK=%NUNIT_DIR%lib\nunit.framework.dll
SET NUNIT_CONSOLE=%NUNIT_DIR%tools\nunit-console-x86.exe

SET REFERENCES=-r:%NUNIT_FRAMEWORK% -r:%INTEROP_DLL%


SET GCCXML=gccxml.exe




IF "%VCINSTALLDIR%" == "" (
	IF "%VS100COMNTOOLS%" == "" (
		echo "Visual Studio not detected. Please run vcvarsall.bat manually"
		goto :eof
	) ELSE (
		call "%VS100COMNTOOLS%..\..\VC\vcvarsall.bat"
	)	
)


call :build_native
call :generateall
call :test_dll
call :nunit
goto :eof

:generateall	
	echo Generating all
	del /s/q/f generated > NUL 2>&1
	call :generatelist "%FILES%"
	del /q generated\__*.cs
goto :eof


:generatelist	
	setlocal
	set list=%1
	set list=%list:"=%
	FOR /f "tokens=1* delims= " %%a IN ("%list%") DO (
	  if not "%%a" == "" call :generate %%a
	  if not "%%b" == "" call :generatelist "%%b"
	)
	endlocal
goto :eof


:generate
	setlocal
	SET NAME=%1
	echo.
	echo Generating %NAME%
	%GCCXML% -fxml=%NAME%.xml Native\%NAME%.h
	%BUILD_DIR%generator.exe -o=generated -ns=Tests -lib=libtest -inline=present -abi=msvc %NAME%.xml
	del /q %NAME%.xml
	endlocal
goto :eof	

:build_native
	SET CPPFILES=Native\%FILES: =.cpp Native\%.cpp
	SET LAYOUT_OPTIONS=/d1reportAllClassLayout
	SET LAYOUT_OPTIONS=/d1reportSingleClassLayout%CLASSES: = /d1reportSingleClassLayout%
	
	cl /nologo /Fe%BUILD_DIR%libtest.dll /LD %CPPFILES% %LAYOUT_OPTIONS% > VSClassLayouts.txt
goto :eof

:test_dll
	SET MANAGED=%FILES: =.cs %.cs
	csc /debug /target:library /platform:x86 /unsafe %REFERENCES% /out:%TEST_DLL% generated\*.cs %MANAGED%
	copy %NUNIT_FRAMEWORK% %BUILD_DIR%
goto :eof

:nunit
	%NUNIT_CONSOLE% -nologo %TEST_DLL%
goto :eof