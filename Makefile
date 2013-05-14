CONFIGURATION=Release

all: mrep doc

debug: CONFIGURATION = Debug
debug: all

build:
	xbuild Apperian.Ease.Publisher/Apperian.Ease.Publisher.csproj /property:Configuration="$(CONFIGURATION)"

pack: build
	mkdir -p repo
	cd repo && mautil p ../Apperian.Ease.Publisher/bin/$(CONFIGURATION)/Apperian.Ease.Publisher.dll && cd ..

mrep: pack
	mautil rb repo

clean:
	rm -f *.mpack

clean-all: clean
	rm -rf repo/

doc:
	cd docs && make
