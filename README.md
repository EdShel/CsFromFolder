# CsFromFolder
A tiny program I use to autogenerate C# class and populate it with members representing all the files in a directory 
(e.g. when you add a new file to a folder, you can refer to it using IntelliSense's autocompletion).

## Example

Imagine that you have __a folder with resources__ C:\Proj\Sprites (png and aseprite or other metadata files)
- C:\Proj\Sprites\Player\idle.png
- C:\Proj\Sprites\Player\run.png
- C:\Proj\Sprites\Player\player.aseprite
- C:\Proj\Sprites\Enemy\idle.png

And you want to refer to your files kinda like that

```C#
...
  Sprite playerSprite = Sprites.playerIdle;
...
```

So you just create the template file C:\Proj\Template.cs __once__

```C#
using System;

namespace Example
{
    public static class Sprites
    {
        #region FOR EACH FILE

        public static string FILE_PATH_PASCAL_CASE => @"FILE_PATH_RELATIVE";

        #endregion

        #region FOR EACH FILE

        public static Sprite FILE_PATH_CAMEL_CASE => GetSprite(@"FILE_PATH_RELATIVE");

        #endregion

        private static Sprite GetSprite(string spriteName)
        {
            return YourContentManager.ContentLoad<Sprite>(spriteName);
        }

        #region FOR EACH FILE

        public static void UnloadFILE_PATH_PASCAL_CASE(string spritePath)
        {
            YourContentManager.ContentUnload<Sprite>(@"FILE_PATH_RELATIVE");
        }

        #endregion
    }
}
```
Every time you think it's necessary, just run the program:

```bash
CsFromFolder.exe -t "C:\Proj\Template.cs" -r C:\Proj\Sprites -s *.png > "C:\Proj\Sprites.cs"
```

and the file C:\Proj\Sprites.cs will be generated or updated into this:

```C#
using System;

namespace Example
{
    public static class Sprites
    {
        public static string PlayerIdle => @"Player\idle.png";

        public static string PlayerRun => @"Player\run.png";
        
        public static string EnemyIdle => @"Enemy\idle.png";
        
        
        public static Sprite playerIdle => GetSprite(@"Player\idle.png");
        
        public static Sprite playerRun => GetSprite(@"Player\run.png");
        
        public static Sprite enemyIdle => GetSprite(@"Enemy\idle.png");
        
        
        private static Sprite GetSprite(string spriteName)
        {
            return YourContentManager.ContentLoad<Sprite>(spriteName);
        }

        public static void UnloadPlayerIdle(string spritePath)
        {
            YourContentManager.ContentUnload<Sprite>(@"Player\idle.png");
        }

        public static void UnloadPlayerRun(string spritePath)
        {
            YourContentManager.ContentUnload<Sprite>(@"Player\run.png");
        }
        
        public static void UnloadEnemyIdle(string spritePath)
        {
            YourContentManager.ContentUnload<Sprite>(@"Enemy\idle.png");
        }
    }
}
```

# How to use

## Create a template file.

This file may be of any size, extension and so on. But it has to contain a region __#region FOR EACH FILE__ with specific content and the ending for it __#endregion__.

This region will be repeated for every discovered file and must contain keywords
which will be replaced with a properly formatted file names (ones discovered in the folder). 

E.g. if the file is located in

```
C:\Project\Sprites\Player\idle.png
```

and the input folder is

```
C:\Project\Sprites\
```

the replaced values will be such as:

- __PlayerIdle__ (for FILE_PATH_PASCAL_CASE)

- __playerIdle__ (for FILE_PATH_CAMEL_CASE)

- __Player\idle.png__ (for FILE_PATH_RELATIVE)



## Run the app

The program must be run with the specific command line paramerers:

- __-t or --template__ - it's path to the template file.
- __-r or --resources__ - a folder with files to be inserted into the template. 
- __-s or --selector__ - selector of files in the --resources folder (it's optional and by default is equals to "\*" - all files).
E.g. it can be "*.png" to select all the PNG images.
- __\>__ - default output stream, it's where to output the created C# file.


