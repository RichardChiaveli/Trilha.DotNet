namespace Trilha.DotNet.Shared.Components;

/// <summary>
/// Example
///     [ModelBinder(BinderType = typeof(UploadModelBinder))]
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class UploadModelComponent<TModel> : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var produtoImagemViewModel = bindingContext.ValueProvider.GetValue("Model").First()
            .Deserialize<IFormFileJson<TModel>>();

        produtoImagemViewModel.Upload = bindingContext.ActionContext.HttpContext.Request.Form.Files;

        bindingContext.Result = ModelBindingResult.Success(produtoImagemViewModel);
        return Task.CompletedTask;
    }
}
