namespace Trilha.DotNet.DependencyResolver;

public static class JsonResolver
{
    public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private readonly string? _dateFormat = "yyyy-MM-dd";

        public DateOnlyJsonConverter() { }

        public DateOnlyJsonConverter(string dateFormat)
        {
            if (!string.IsNullOrWhiteSpace(dateFormat))
                _dateFormat = dateFormat;
        }

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.AsString();

            if (_dateFormat != null
                && DateOnly.TryParseExact(dateString, _dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateConverted))
                return dateConverted;

            if (DateOnly.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateConverted))
                return dateConverted;

            throw new InvalidDataException($"'{dateString}' invalid value");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_dateFormat, CultureInfo.InvariantCulture));
        }
    }

    public sealed class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private readonly string? _timeFormat = "HH:mm:ss";

        public TimeOnlyJsonConverter() { }

        public TimeOnlyJsonConverter(string timeFormat)
        {
            if (!string.IsNullOrWhiteSpace(timeFormat))
                _timeFormat = timeFormat;
        }

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var timeString = reader.AsString();

            if (_timeFormat != null
                && TimeOnly.TryParseExact(timeString, _timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var timeConverted))
                return timeConverted;

            if (TimeOnly.TryParse(timeString, CultureInfo.InvariantCulture, DateTimeStyles.None, out timeConverted))
                return timeConverted;

            throw new InvalidDataException($"'{timeString}' invalid value");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_timeFormat, CultureInfo.InvariantCulture));
        }
    }

    public sealed class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        private readonly string? _dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public DateTimeJsonConverter() { }

        public DateTimeJsonConverter(string dateTimeFormat)
        {
            if (!string.IsNullOrWhiteSpace(dateTimeFormat))
                _dateTimeFormat = dateTimeFormat;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateTimeString = reader.AsString();

            if (_dateTimeFormat != null
                && DateTime.TryParseExact(dateTimeString, _dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateConverted))
                return dateConverted;

            if (DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, out dateConverted))
                return dateConverted;

            throw new InvalidDataException($"'{dateTimeString}' invalid value");
        }
        
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_dateTimeFormat ?? _dateTimeFormat, CultureInfo.InvariantCulture));
        }
    }

    public class EnumJsonConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonValue = reader.AsString().Trim().ToUpper();
            var typeNotNull = typeToConvert;

            if (typeToConvert.GenericTypeArguments.Length != 0)
                typeNotNull = typeToConvert.GenericTypeArguments[0];

            if (long.TryParse(jsonValue, out var index))
            {
                var parsedValue = Enum.ToObject(typeNotNull, index);

                if (typeNotNull.IsEnumDefined(parsedValue))
                    return parsedValue;
            }

            foreach (var field in typeNotNull.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.GetCustomAttribute(field, typeof(JsonEnumValueAttribute)) is JsonEnumValueAttribute attribute)
                {
                    if (attribute.JsonValue.Trim().Equals(jsonValue, StringComparison.CurrentCultureIgnoreCase))
                        return field.GetValue(null);
                }
                else
                {
                    if (field.Name.Trim().Equals(jsonValue, StringComparison.CurrentCultureIgnoreCase))
                        return field.GetValue(null);
                }
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var text = value.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                writer.WriteNullValue();
                return;
            }

            var typeToConvert = value.GetType();

            var valueToCompare = value.ToString()?.Trim().ToUpper();

            foreach (var field in typeToConvert.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (!field.Name.Trim().Equals(valueToCompare, StringComparison.CurrentCultureIgnoreCase)) continue;

                if (Attribute.GetCustomAttribute(field, typeof(JsonEnumValueAttribute)) is JsonEnumValueAttribute attribute)
                    text = attribute.JsonValue;
                else
                    text = field.Name;

                break;
            }

            writer.WriteStringValue(text);
        }
    }

    public static IServiceCollection AddJson(this IServiceCollection services)
    {
        services
            .AddControllers(opt =>
            {
                opt.AllowEmptyInputInBodyModelBinding = true;
            }).
            AddJsonOptions(settings =>
            {
                settings.JsonSerializerOptions.ConfigureSerializeOptions(
                    new DateOnlyJsonConverter(),
                    new DateTimeJsonConverter(),
                    new TimeOnlyJsonConverter(),
                    new EnumJsonConverter());
            });

        return services;
    }
}