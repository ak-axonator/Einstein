namespace WebMagic
{
    public static class GlobalPaths
{
    private static string _systemFolder;
    public static string SystemFolder
    {
        get { return _systemFolder; }
        set { _systemFolder = value; }
    }

    private static string _projectFolder;
    public static string ProjectFolder
    {
        get { return _projectFolder; }
        set { _projectFolder = value; }
    }

    private static string _outputFolder;

    public static string OutputFolder
    {
        get { return _outputFolder; }
        set { _outputFolder = value; }
    }
    private static string _assetFolder;

    public static string AssetsFolder
    {
        get { return _assetFolder; }
        set { _assetFolder = value; }
    }

    private static string _GPTFolder;

    public static string GPTFolder
    {
        get { return _GPTFolder; }
        set { _GPTFolder = value; }
    }
}
}