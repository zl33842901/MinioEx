using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.MinioEx
{
    public enum DirectoryPolicy : byte
    {
        None = 0,
        ByMonth = 1,
        ByDay = 2,
        ByYear = 3
    }
}
