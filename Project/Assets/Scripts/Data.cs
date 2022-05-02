using System.Collections.Generic;

public class Data
{
    public List<Values> Stats { get; set; }
}

public class Values
{
    public string Name { get; set; }
    public int Electrons { get; set; }
    public int Neutrons { get; set; }
    public int Protons { get; set; }
    public string Radius { get; set; }
    public string Mass { get; set; }
    public string Oxidation { get; set; }
    public string Negativity { get; set; }
    public string Meltingpoint { get; set; }
    public string Boilingpoint { get; set; }
}
