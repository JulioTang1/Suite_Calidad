using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Search
/// </summary>
public class Search
{
    public Search()
    {

    }

    public Search(string val, string reg)
    {
        //
        // TODO: Add constructor logic here
        //
        this.value = val;
        this.regex = reg;
    }

    public string value { get; set; }
    public string regex { get; set; }
}