#!/bin/sh -e
uname -a

dotnet test -v n CodeInjection.Tests/
