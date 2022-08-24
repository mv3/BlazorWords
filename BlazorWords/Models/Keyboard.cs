namespace BlazorWords.Models
{
    public class Keyboard
    {
        public Keyboard()
        {
            KeyRows.Add(new KeyRow
            {
                Keys = new List<Key>
                {
                    new Key{KeyText= "Q"},
                    new Key{KeyText= "W"},
                    new Key{KeyText= "E"},
                    new Key{KeyText= "R"},
                    new Key{KeyText= "T"},
                    new Key{KeyText= "Y"},
                    new Key{KeyText= "U"},
                    new Key{KeyText= "I"},
                    new Key{KeyText= "O"},
                    new Key{KeyText= "P"}
                }
            });
            KeyRows.Add(new KeyRow
            {
                Keys = new List<Key>
                {
                    new Key{KeyText= "A"},
                    new Key{KeyText= "S"},
                    new Key{KeyText= "D"},
                    new Key{KeyText= "F"},
                    new Key{KeyText= "G"},
                    new Key{KeyText= "H"},
                    new Key{KeyText= "J"},
                    new Key{KeyText= "K"},
                    new Key{KeyText= "L"}
                }
            });
            KeyRows.Add(new KeyRow
            {
                Keys = new List<Key>
                {
                    new Key("ENTER", Enums.KeyType.Enter),
                    new Key{KeyText= "Z"},
                    new Key{KeyText= "X"},
                    new Key{KeyText= "C"},
                    new Key{KeyText= "V"},
                    new Key{KeyText= "B"},
                    new Key{KeyText= "N"},
                    new Key{KeyText= "M"},
                    new Key("⌫", Enums.KeyType.Delete)
                }
            });
        }
        public List<KeyRow> KeyRows { get; set; } = new();

        public Key GetKey(string key)
        {
            foreach(var row in KeyRows)
            {
                if (row.Keys.Any(k=>k.KeyText == key))
                {
                    return row.Keys.First(k => k.KeyText == key);
                }
            }
            return new Key();
        }
    }
}
