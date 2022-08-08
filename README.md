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

# Dependencies
Before you can use you have to make sure that Pico8 is installed on your loacla machine. The whole init/unpack/pack stuff will work, but to use the run cmd you need to have pico8 locally at the default path `C:\\Program Files (x86)\\PICO-8\\pico8.exe\` installed.

# Installation
To use the file globaly on your machine you need to install it manually.
1. download all the build files from the latest release
2. unzip the provided 'pico8-cli.zip' somewhere in `C:\Program Files (x86)` i.e. in `C:\Program Files (x86)\my_cmd_tools`
3. this gives you the following structure:
```
C:\Program Files (x86)\my_cmd_tools
  |-pico8-cli <-folder not zip!
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
[info]: please provide one of the options: init, unpack, pack, run,
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
|   |-backups                             <- each pack / unpack will generate a packup of the `saturn91-dungeon-crawler.p8` file here
|   |-restOfFile.p8                       <- holds all Data which is not lua (I advise not to mess with this file manually)
|   |-saturn91-dungeon-crawler.p8.config  <- holds some information and data how to pack / unpack the file
|-lua                                     <- work with the lua files included here
|   |-01_main.lua                         <- tab 1/16 is always called main
|   |-02_my_other_tab                     <- seccond tab which has a --my_other_tab comment on top                     
|-saturn91-dungeon-crawler.p8             <- the runnable pico8 from which all tabs file where extracted into the lua folder
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

