namespace Core;

public abstract class Entity<T> where T: struct
{
    public T Id { get; protected set; }
}