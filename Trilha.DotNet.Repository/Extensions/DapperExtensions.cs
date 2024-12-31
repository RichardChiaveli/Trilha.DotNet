namespace Trilha.DotNet.Repository.Extensions;

public static class DapperExtensions
{
    public static void Map<T>()
    {
        var typeClass = typeof(T);

        if (!typeClass.IsClass) return;
        var map = new CustomPropertyTypeMap(typeClass, (type, columnName)
            => type.GetAttributeFromAnnotattion<ColumnAttribute>(columnName));

        SqlMapper.SetTypeMap(typeClass, map);
    }
}
