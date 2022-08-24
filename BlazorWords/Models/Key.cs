using BlazorWords.Models.Enums;
using BlazorWords.Constants;

namespace BlazorWords.Models
{
    public class Key
    {
        public Key(string keyText = "", KeyType keyType = KeyType.Letter)
        {
            KeyText = keyText;
            KeyType = keyType;
            switch (keyType)
            {
                case KeyType.Enter:
                    ClassList.Add(StyleClasses.KEY_ENTER);
                    break;
                case KeyType.Delete:
                    ClassList.Add(StyleClasses.KEY_DELETE);
                    break;
            }
            KeyColor = Colors.KEY_DEFAULT;
        }
        public string KeyText { get; set; } = string.Empty;
        public KeyType KeyType { get; set; }
        public List<string> ClassList { get; set; } = new();
        public string KeyColor { get; set; } = string.Empty;
    }
}
