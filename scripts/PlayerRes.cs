using Godot;

public struct PlayerRes
{
    private Godot.Collections.Dictionary<string, Variant> _dict;
    public string Name
    {
        get { return (string)_dict["Name"]; }
        set { _dict["Name"] = value; }
    }
    public int Id
    {
        get { return (int)_dict["Id"]; }
        set { _dict["Id"] = value; }
    }

    public PlayerRes(string playerName, int id)
    {
        _dict = new Godot.Collections.Dictionary<string, Variant>();
        this.Name = playerName; 
        this.Id = id; 
    }
    public PlayerRes(Godot.Collections.Dictionary<string, Variant> dict)
    {
        _dict = dict;
    }
    public Godot.Collections.Dictionary<string, Variant> GetDict()
    {
        return _dict;
    }
}