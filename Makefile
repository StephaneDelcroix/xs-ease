CONFIGURATION=Release

all: pack

debug: CONFIGURATION = Debug
debug: all

build:
	xbuild Apperian.Ease.Publisher/Apperian.Ease.Publisher.csproj /property:Configuration="$(CONFIGURATION)"

pack: build
	mautil p Apperian.Ease.Publisher/bin/$(CONFIGURATION)/Apperian.Ease.Publisher.dll
