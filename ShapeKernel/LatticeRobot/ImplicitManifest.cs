using System.ComponentModel.Design;

public class ImplicitManifest {
    public string description = "";
    public IList<string> references = new List<string>();
    public string createdBy = "";
    public string cSharpCode = "";
    public IList<string> tags = new List<string>();
    public string cSharpLibrary = "";
    public string name = "LatticeRobot Lattice";
    public IList<ImplicitParameter> parameters = new List<ImplicitParameter>();
    public string profileImage = "";
    public LatticeVariant defaultVariant = LatticeVariant.thin;
}