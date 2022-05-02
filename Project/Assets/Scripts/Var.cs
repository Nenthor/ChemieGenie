public static class Var
{
    public static DisplayState State { get; set; }
    public static ModelState Model { get; set; }

    public static Element[] GmElements { get; set; }

    public static Data Data { get; set; }

    public const int lenghtX = 8, lenghtY = 4;
    public static readonly int[] maxShellSize = { 2, 8, 8, 18 };

    public enum Elements
    {
        H, He = 7,
        Li, Be, B, C, N, O, F, Ne,
        Na, Mg, Al, Si, P, S, Cl, Ar,
        K, Ca, Ga, Ge, As, Se, Br, Kr
    }

    public enum DisplayState
    {
        displayElements, displaySettings, displayDetails
    }

    public enum ModelState
    {
        shellModel, energyModel, formModel, cloudModel
    }
}
