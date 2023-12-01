using g4;

public struct Implicit {
    public double Distance { get; set; }
    public Vector3d Gradient { get; set; }

    public Implicit(double distance, Vector3d gradient) {
        Distance = distance;
        Gradient = gradient;
    } 
}