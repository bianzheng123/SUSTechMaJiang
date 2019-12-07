using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class XMLMajor
{
    public string name;
    public int priceNoMajor;
    public int priceHaveMajor;
    public override string ToString()
    {
        return name + "," + priceNoMajor + "," + priceHaveMajor;
    }
}
