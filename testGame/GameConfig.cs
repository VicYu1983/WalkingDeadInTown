using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class GameConfig
{
    public static List<object[]> WeaponConfig = new List<object[]>()
    {
        /* age, size, dragable, count, offset, expand_speed, delay, startSize, clearWhenRelease */
        new object[] { "步槍(半自動)", 10, .3f, false, 3, 30.0f, 0.05f, false, 0.0f, false },
        new object[] { "高性能狙擊槍", 60, 2.0f, false, 1, 0.0f, 0.1f, false, 1.0f, true },
        new object[] { "雙管散彈槍", 6, .2f, false, 10, 20.0f, 1.0f, false, 0.0f, false },
        new object[] { "智慧型狙擊槍", 300, 1.0f, true, 1, 0.0f, 0.02f, true, .5f, true },
        new object[] { "雷射加農砲", 300, 1.0f, true, 1, 0.0f, 0.1f, true, 0.0f, false },
        new object[] { "步槍:全自動", 6, .1f, false, 1, 30.0f, 0.02f, false, 0.0f, false }
    };
}