namespace ProductStorage.BLL.Exceptions;

public class ProductNotFoundException(Guid id) : Exception($"Товар с Id {id} не найден.")
{
    public Guid Id { get; } = id;
}
