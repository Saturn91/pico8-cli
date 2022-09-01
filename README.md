# pico8-cli (windows only for now!)

This is a cli to pack and unpack pico.p8 cartride source code into seperat lua per tab defined.

What it provides
- a cli which allows you to pack and unpack your Lua code from a pico8 project into seperate 01_tab1.lua, 02_tab2.lua files

# TLDR (After installation :P)
1. `pico8-cli init` within a seperat folder like `saturn91-dungeon-crawler` -> lua files get unpacked
2. make your changes in code
3. `pico8-cli run` runs your packed code within Pico8
4. For editing everything but code, you still have to do it in pico8, after a `pico8-cli run` hit `ESC`within pico8 and edit i.e. the sprites
5. Save your changes within pico8 `CTRL+S` as usual
6. End Pico8
7. run cmd `pico8-cli unpack override` to apply the changes to the unpacked file
9. run cmd `pico8-cli build` to setup (first time) and build your game into binaries and web executables
10. run cmd `pico8-cli deploy itch -b` generates a prefilled deploy.config file in which you have to enter your username and the id of an already existing itch.io project. Run command again to build and deploy your game on itch
8. run cmd `pico8-cli test` to setup (first time) and run tests
10. run cmd `pico8-cli status` to see if you are currently within a pico8-cli project or not
11. run cmd `pico8-cli help` to see all possible commands and their parameters

# Dependencies
Before you can use you have to make sure that Pico8 is installed on your loacla machine. The whole init/unpack/pack stuff will work, but to use the run cmd you need to have pico8 locally at the default path `C:\\Program Files (x86)\\PICO-8\\pico8.exe\` installed.

# Installation
To use the file globaly on your machine you need to install it manually.
1. download all the build files from the [latest release](https://github.com/Saturn91/pico8-cli/releases)
2. unzip the provided 'pico8-cli.zip' somewhere in `C:\Program Files (x86)` i.e. in `C:\Program Files (x86)\my_cmd_tools`
3. this gives you the following structure:
```
C:\Program Files (x86)\my_cmd_tools
  |-pico8-cli <-folder not zip!
      |-butler <- this holds a local copy of itch.io's butler, replace with newer version or delete completly if you wont deploy to itch
      |-test   <- folder which holds the testFramework build
      |-build.p8
      |-pico8.cli.config
      |-pico8-cli.deps.json
      |-pico8-cli.dll
      |-pico8-cli.exe
      |-pico8-cli.pdb
      |-pico8-cli.runtimeconfig.dev
      |-pico8-cli.runtimeconfig.json
```
4. add the path of the parent folder `C:\Program Files (x86)\my_cmd_tools\pico8-cli` to the Environment variables either manually via the settings or using the cmd: `SetX PATH "%PATH%;C:\Program Files (x86)\my_cmd_tools\pico8-cli"`
5. now open a new CMD and type `pico8-cli` and if you see the following you succeded:
```
C:\Users\saturn91>pico8-cli
[info]: please provide one of the options: init, unpack, pack, run, test,
```

# Using the cli
## Test installation and see all commands
```
C:\Users\saturn91>pico8-cli
[info]: please provide one of the options: init, unpack, pack, run,
```
## Setup a new project
1. create a Folder i.e. `saturn91-dungeon-crawler`
2. open a cmd within the Folder and type `pico8-cli init`
```
C:\Users\saturn91\Desktop\saturn91-dungeon-crawler>pico8-cli init
[info]: init
```
3. open the folder in your file explorer and see all the files generated
```
saturn91-dungeon-crawler
|-.pico8-cli                              <- eventually hidden as it has a dot (you do not need it most of the time)
|   |-backups                             <- each pack / unpack will generate a packup of the `saturn91-dungeon-crawler.p8` file here (max file number = 10 by default)
|   |-saturn91-dungeon-crawler.p8.config  <- config file, have a look at all the settings
|-lua                                     <- work with the lua files included here
|   |-01_main.lua                         <- tab 1 of pico8 project        
|-meta
|   |-restOfFile.p8                       <- this file holds the header which each pico8 file has, just ignore it
|-resources                               <- all other data then lua code will be stored within this folder
|   |-__gfx__.txt                         <- sprite data
|-saturn91-dungeon-crawler.p8             <- the runnable pico8 from which all tabs file where extracted from into the lua folder
|-.gitignore                              <- initialized gitignore
|-README.md                               <- initialized README
```
4. now your project is setup correctly

## Pack
```
C:\Users\saturn91\Desktop\saturn91-dungeon-crawler>pico8-cli pack
[info]: pack
```
The Pack cmd is used to pack all the (in the meantime probably edited by you) lua files back into the existing `saturn91-dungeon-crawler.p8` file so you can run it in Pico8.
But for that also check out the `RUN` cmd first.

## Run
```
C:\Users\saturn91\Desktop\saturn91-dungeon-crawler>pico8-cli run
[info]: run
```
If pico8 is installed as described in [Dependencies](#Dependencies) this command will 
1. pack the lua files into the `saturn91-dungeon-crawler.p8` file
2. run it in pico8
3. unpack again (in case you edited something within pico8)
4. provide the parameter `-t` along and prevent setup tests from firing each time
5. provide the parameter `-u` along and prevent pico8-cli from unpacking again afterwards. You usally will us this if you do NOT edit code, graphics, map or sound Data within pico8 but only test your last changes from the external editor you are using. 

## Unpack
```
C:\Users\saturn91\saturn91-dungeon-crawler>pico8-cli unpack override
[Prop]: enabled: override
[info]: unpack
```
The unpack cmd is mainly used after a `pico8-cli run` in which you escaped into the project and ALTERED code, sound, music, graphics or sound effects... basically after any change you made in the cartridge within pico8.
So in short it is a methode to get changes from pcio8 into the `.pico8-cli/restOfFile.p8` file which holds this data.

To prevent you from accidentaly override pending changes in your lua/* files you have to provide the `override` option to aknoledge that you might override pending changes within the lua files. If you forget it you will see this:
```
C:\Users\saturn91\Desktop\saturn91-dungeon-crawler>pico8-cli unpack
[info]: unpack
[info]: The directory 'lua' already exists, by unpacking it will get overriden! if you are sure, run 'unpack override'
```

## Build
Building was never easier with pico8, just run the cmd `pico8-cli build` and get promted with the following:

You have to make sure to capture a label before you start a build if you get the following error description:
```
C:\Users\manue\Documents\GitHub\wowie_jam_4_0>pico8-cli build
[info]: build
[err.]: you didn't capture a cratridge label yet, please do so by pressing F7 in the running pico8 card and then save (ctrl+S)
[err.]: not able to build...
```

Successfull build:
```
C:\Users\saturn91\Documents\GitHub\wowie_jam_4_0>pico8-cli build
[info]: build
RUNNING: ./.pico8-cli/build.p8
EXPORT: wowie_jam_4_0/wowie_jam_4_0.bin -i 10 -s 2 -c 0 wowie_jam_4_0/wowie_jam_4_0.p8
EXPORT: -f wowie_jam_4_0/wowie_jam_4_0.html wowie_jam_4_0/wowie_jam_4_0.p8

[info]: Build succeded, find your files here: C:\Users\saturn91\AppData\Roaming\pico-8\carts/wowie_jam_4_0
``` 
### Defining the icon of the .exe and other
After running the build command the first time you find a file "buildIconConfig.txt" in your repository.
You can define there which sprites within your spritesheet should be used as a icon for your builds.

```
this params are used to describe the icon for binary builds <- this is a comment
i: 10	<- use sprite 10
s: 2	<- display 16x16 pixel (so in this case a 2x2 sprite
c: 0	<- the transparency color, default is 0 (black)
```

## Deploying

At the moment I only support the deploy automation Butler which allows you to upload to itch.io.
Please feel free to suggest/request further Platforms (like steam google play and so on...). I can not promise that I will implement them myself, but as this project is open source, please let me know (open a topic on this repo)

### Deploy to Itch
For deploying I use a local installation of [itch.io's butler](https://itch.io/docs/butler/) within the projects installation folder ./butler.
Deploying is only possible after [building](#build) your project.

1. run cmd `deploy itch` if you have already build your project
2. run cmd `deploy itch -b` to build project and then deploy it
3. if this is your first time deploying a file "deploy.config" will get generated in your project
4. Make sure you already have an itch project setup for example: https://saturn91.itch.io/best-game-ever
5. Replace the string "youruser/yourgame" with YOUR Username and your game in my case it would be: "saturn91/best-game-ever"

## Unit Testing setup and usage
### I do not know what Unit testing is
If you do not know what Unit tests are and are interested in the topic, have a look here: [Wikipedia/Unittests](https://en.wikipedia.org/wiki/Unit_testing). It allows you to test certain functions of your code, to make sure the always work as expected. It allows you to test fast.

### Disclaimer
My Testframework is using regular lua to run its tests... therefor certain Pico8 specific functions like "+=" do not run nativly in my Testingframwork. I did translate the follwoing already into the engine, but be aware that in the whole lua file which you want to test you can only use native lua. An Exception are the bellow shown pico8 specific rules which I translate into default lua when you start a test:

1. `if (x) doB()`
2. `+=`
3. `!=`

**TLDR: only use default lua in tabs which should be tested in aswell as in the test file** 

### What the integrated Testframework provides
I set up a simplified Test framework within pico8-cli which allows you to write tests for certain functions. The results will be displayed in a very simple index.html file within your browser.

### Set up a test
In this section I will explain to you how you have to setup everything for testing using a simple and a more complex topic.
1. the basics -> write and test a xor function
2. the complex -> expand the test file with a get_rnd_from_array function

#### The basics
Ok so you decided during development of your pico8-cli project, that you need to test a xor function. An XOR function is basically an or function with the exception that the result of `true xor true` is `false`. So in your seperat lua file (e.g. 05_utils.lua) you write the following code:

```lua
--utils

function xor(a,b)
    if (a and b) return false
    return a or b
end
``` 

This function should now be tested.

#### Testfunktion for xor
To write a test in general we have to use the function `assert_equal(obj, obj, string)` which is specific to my test framework. It allows you to check if two statements are the same. I.e. to test if xor(true,true) results in false as expected, we would write the bellow test. Please also note the text at the end, this is the description to allows us to identify which test case has failed.

```lua
-- assert that  1.object == 2.object + description
assert_equal(xor(true,true),false,"xor 1-1")
```

1. Copy the tab in which your xor function is specified (e.g. 05_utils.lua) and replace the `.` at the end with `.test.lua` (so you get 05_utils.test.lua) this will tell 1. pico8-cli to not use it in the build/pack, and 2. the testframwork to use it as a test.

2. For xor we want to test 4 cases: `false, false` `true, false` `false, true` `true, true`
3. Write the 4 cases as follows into the .test file
```lua
--xor
assert_equal(xor(true,false),true,"xor 1-0")
assert_equal(xor(false,true),true,"xor 0-1")
assert_equal(xor(false,false),false,"xor 0-0")
assert_equal(xor(true,true),false,"xor 1-1")
```
4. open your terminal and run pico8-cli test
5. your browser will automatically open and show the results as follows:
```
[RUN ]: lua
_utils.test.lua
[PASS] -> xor 1-0
[PASS] -> xor 0-1
[PASS] -> xor 0-0
[PASS] -> xor 1-1
[ OK ]: test succeded: 4/4 passed
```

Congrats you just wrote your own unit test, to see how a failed test would look like, you can either play with the expected values in the test, or alter the XOR function. Then jsut simply rerun the code.

### Complex example
In order to test a function which uses Pico8 specific functions like rnd() or ceil() we have to mock them. The bellow example shows how you would do that. I will not explain it further but let you try arround:

1. code to test (at the bottom of 05_utils.lua)
```lua
function get_rnd_from_arr(arr)
    local index = ceil(rnd(#arr))
    return arr[index]
end
```

2. Unit test at the bottom of 05_utils.test.lua)
```lua
--get_rnd_from_arr
-- mock rnd -> will just increase the number by one each time
local counter = 0
function rnd() 
	counter+=1
	return counter
end

--mock ceil
function ceil(i)
	return i
end

local _1
local _2
local _3
local _4
for i=1, 4 do
	local rnd_num = get_rnd_from_arr{1,2,3,4}
	if (rnd_num == 1) _1 = true
	if (rnd_num == 2) _2 = true
	if (rnd_num == 3) _3 = true
	if (rnd_num == 4) _4 = true
end

assert_equal(_1 and _2 and _3 and _4, true, "get rnd from array")
```

### Setup a Github Page on master so you can always see the state of your tests
If you want to always see the latest state of your test on your github repo, as the index.html gets saved within your github repo, you can host your project as a github page and always see if all tests are passing on your current master.
Learn [here](https://pages.github.com/) hwo you can setup a Github page from your Github repository.
