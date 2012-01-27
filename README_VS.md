# Getting Started with CXXI on Windows Platform

## GCC_XML

For GCC_XML you have two options:

  1. Download GCC_XML source and use CMake to compile it (You must use source as 0.6.0 does not support VS 2010)
  2. Download a precompiled version from here: http://goo.gl/CbVPx and install it. Make sure its on your path

## GIT

Since most files are in source control with LF endings you need to enable core.autocrlf
	
	git config core.autocrlf true
	
## Testing the Generator

In order to test the generator there are a few parts.

  1. Open cxxi.sln and Compile Generator and Mono.Cxxi
  2. Run *tests\build.cmd* from the command prompt
  
Once you have done the above two steps you could opt to "Include in project" all of the files that were put in the *generated* folder which resides under the **Tests** project and Compile/Test from Visual Studio.

GLHF