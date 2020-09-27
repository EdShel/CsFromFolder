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
