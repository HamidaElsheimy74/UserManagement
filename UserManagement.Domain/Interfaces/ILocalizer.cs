namespace UserManagement.Domain.Interfaces;
public interface ILocalizer
{
    string this[string key] { get; }
    string GetString(string key);
}
