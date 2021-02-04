#!/bin/sh
cd ~/src || exit

dotnet restore

/bin/bash -l
