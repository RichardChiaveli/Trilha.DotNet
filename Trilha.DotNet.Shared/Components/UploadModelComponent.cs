namespace Trilha.DotNet.Shared.Components;

/// <summary>
/// Example
///     [ModelBinder(BinderType = typeof(UploadModelComponent))]
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class UploadModelComponent<TModel> : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var model = bindingContext.ValueProvider.GetValue("Model").First()
            .ParseJson<IFormFileJson<TModel>>();

        model.Upload = bindingContext.ActionContext.HttpContext.Request.Form.Files;

        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}
