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
        majors.Add(0,"通识通修");
        majors.Add(1, "化学系");
        majors.Add(2, "数学系");
        majors.Add(3, "计算机系");
    }
}
public enum Major
{
    None = 0,
    Chemistry = 1,
    Math = 2,
    ComputerScience = 3
}
public enum Gender
{
    Male = 0,
    Female = 1
}
