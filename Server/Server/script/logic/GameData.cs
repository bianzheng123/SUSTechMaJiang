using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 用来存放游戏进行中的数据
/// </summary>
public enum Major
{
    None = 0,
    Math = 1,
    Chemistry = 2,
    ComputerScience = 3
}
/// <summary>
/// 每一局游戏中可以使用的最大技能次数
/// </summary>
public enum MaxSkillTime
{
    None = 0,
    Math = 3,
    Chemistry = 3,
    ComputerScience = 10
}