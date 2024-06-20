using Assets.Scripts;

public class UnhighlightFieldRequest : Request
{
    public Field field;
    public UnhighlightFieldRequest(Field f)
    {
        field = f;
    }
}
