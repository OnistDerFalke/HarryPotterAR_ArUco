using Assets.Scripts;

public class HighlightFieldRequest : Request
{
    public Field field;
    public HighlightFieldRequest(Field f)
    {
        field = f;
    }
}
