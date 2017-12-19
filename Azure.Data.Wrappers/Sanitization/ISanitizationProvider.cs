namespace Azure.Data.Wrappers.Sanitization
{
    public interface ISanitizationProvider
    {
        string Sanitize(string input);
    }
}