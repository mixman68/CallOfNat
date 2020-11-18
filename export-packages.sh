#!/bin/sh

#Ensure we are in CallOfNat dir
SOURCE="${BASH_SOURCE[0]}"
while [ -h "$SOURCE" ]; do # resolve $SOURCE until the file is no longer a symlink
  DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
  SOURCE="$(readlink "$SOURCE")"
  [[ $SOURCE != /* ]] && SOURCE="$DIR/$SOURCE" # if $SOURCE was a relative symlink, we need to resolve it relative to the path where the symlink file was located
done
DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
cd $DIR

mkdir dist

dotnet clean
dotnet restore
dotnet publish -c Release -r win10-x64
cp -R $DIR/CallOfNat/bin/Release/netcoreapp3.1/win10-x64/publish $DIR/dist/CallOfNat

cp $DIR/CallOfNat.Game.Warzone/bin/Release/netstandard2.0/win10-x64/CallOfNat.Game.Warzone.dll $DIR/dist/CallOfNat/Games

7z a -tzip $DIR/dist/CallOfNat.zip $DIR/dist/CallOfNat
rm -rf $DIR/dist/CallOfNat

7z a -tzip $DIR/dist/CallOfNat.Game.Warzone.zip $DIR/CallOfNat.Game.Warzone/bin/Release/netstandard2.0/win10-x64/CallOfNat.Game.Warzone.dll