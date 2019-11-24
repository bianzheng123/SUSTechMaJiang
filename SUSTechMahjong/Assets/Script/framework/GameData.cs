using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Gamedata
{
    /// <summary>
    /// 用来存放不同的camp对应的系
    /// </summary>
    public static Dictionary<int, string> majors;
    public static void Init()
    {
        majors = new Dictionary<int, string>();
        majors.Add(1, "计算机系");
        majors.Add(2, "化学系");
        majors.Add(3, "生物系");
    }
}

