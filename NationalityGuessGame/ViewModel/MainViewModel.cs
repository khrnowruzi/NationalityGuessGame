using NationalityGuessGame.HelpClasses;
using System.Collections.Generic;

namespace NationalityGuessGame.ViewModel
{
    public class MainViewModel
    {
        public List<string> GetPath()
        {
            return Images.ImagesPath;
        }
    }
}
