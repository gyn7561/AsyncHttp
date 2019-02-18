using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncHttp.Extension
{
    static class BinaryFind
    {
        public static int IndexOf(this byte[] arrayToSearchThrough, byte[] patternToFind, int startIndex = 0)
        {
            if (patternToFind.Length > arrayToSearchThrough.Length)
                return -1;
            for (int i = startIndex; i < arrayToSearchThrough.Length - patternToFind.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++)
                {
                    if (arrayToSearchThrough[i + j] != patternToFind[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
