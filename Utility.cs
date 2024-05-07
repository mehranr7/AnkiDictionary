namespace AnkiDictionary
{
    public static class Utility
    {
        public static string FixFrontText(string front)
        {
            if(string.IsNullOrEmpty(front))
                return front;

            var original = front;
            try
            {
                while (front[0] == ' ')
                {
                    front = front.Substring(1, front.Length-1);
                }
                front = front.Replace("\n", "");
                front = front.Replace("\r", "");
                while (front[^1] == ' ')
                {
                    front = front.Substring(0, front.Length - 1);
                }
                front = front[0].ToString().ToUpper() + front.Substring(1, front.Length - 1);
                
                return front;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return original;
            }
        }

        public static string PrintSpaces(int prevWordLength, int maxWordLength=50)
        {
            string spaces = "";
            for (int i = 0; i < maxWordLength - prevWordLength; i++)
                spaces += " ";
            return spaces;
        }
    }
}
