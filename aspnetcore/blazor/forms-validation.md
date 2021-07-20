---
title: ASP.NET Core Blazor forms and validation
author: guardrex
description: Learn how to use forms and field validation scenarios in Blazor.
monikerRange: '>= aspnetcore-3.1'
ms.author: riande
ms.custom: mvc
ms.date: 04/27/2021
no-loc: [Home, Privacy, Kestrel, appsettings.json, "ASP.NET Core Identity", cookie, Cookie, Blazor, "Blazor Server", "Blazor WebAssembly", "Identity", "Let's Encrypt", Razor, SignalR]
uid: blazor/forms-validation
---
# ASP.NET Core Blazor forms and validation

The Blazor framework supports webforms with validation using the <xref:Microsoft.AspNetCore.Components.Forms.EditForm> component bound to a model that uses [data annotations](xref:mvc/models/validation).

To demonstrate how an <xref:Microsoft.AspNetCore.Components.Forms.EditForm> component works with [data annotations](xref:mvc/models/validation) validation, consider the following `ExampleModel` type. The `Name` property is marked required with the <xref:System.ComponentModel.DataAnnotations.RequiredAttribute> and specifies a <xref:System.ComponentModel.DataAnnotations.StringLengthAttribute> maximum string length limit and error message.

`ExampleModel.cs`:

::: moniker range=">= aspnetcore-5.0"

[!code-csharp[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/ExampleModel.cs?highlight=5-6)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-csharp[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/ExampleModel.cs?highlight=5-6)]

::: moniker-end

A form is defined using the Blazor framework's <xref:Microsoft.AspNetCore.Components.Forms.EditForm> component. The following Razor component demonstrates typical elements, components, and Razor code to render a webform using an <xref:Microsoft.AspNetCore.Components.Forms.EditForm> component, which is bound to the preceding `ExampleModel` type.

`Pages/FormExample1.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample1.razor)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample1.razor)]

::: moniker-end

In the preceding `FormExample1` component:

* The <xref:Microsoft.AspNetCore.Components.Forms.EditForm> component is rendered where the `<EditForm>` element appears.
* The model is created in the component's `@code` block and held in a private field (`exampleModel`). The field is assigned to  <xref:Microsoft.AspNetCore.Components.Forms.EditForm.Model?displayProperty=nameWithType>'s attribute (`Model`) of the `<EditForm>` element.
* The <xref:Microsoft.AspNetCore.Components.Forms.InputText> component (`id="name"`) is an input component for editing string values. The `@bind-Value` directive attribute binds the `exampleModel.Name` model property to the <xref:Microsoft.AspNetCore.Components.Forms.InputText> component's <xref:Microsoft.AspNetCore.Components.Forms.InputBase%601.Value%2A> property.
* The `HandleValidSubmit` method is assigned to <xref:Microsoft.AspNetCore.Components.Forms.EditForm.OnValidSubmit>. The handler is called if the form passes validation.
* The data annotations validator (<xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component&dagger;) attaches validation support using data annotations:
  * If the `<input>` form field is left blank when the **`Submit`** button is selected, an error appears in the validation summary (<xref:Microsoft.AspNetCore.Components.Forms.ValidationSummary> component&Dagger;) ("`The Name field is required.`") and `HandleValidSubmit` is **not** called.
  * If the `<input>` form field contains more than ten characters when the **`Submit`** button is selected, an error appears in the validation summary ("`Name is too long.`") and `HandleValidSubmit` is **not** called.
  * If the `<input>` form field contains a valid value when the **`Submit`** button is selected, `HandleValidSubmit` is called.

&dagger;The <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component is covered in the [Validator component](#validator-components) section. &Dagger;The <xref:Microsoft.AspNetCore.Components.Forms.ValidationSummary> component is covered in the [Validation Summary and Validation Message components](#validation-summary-and-validation-message-components) section. For more information on property binding, see <xref:blazor/components/data-binding#binding-with-component-parameters>.

## Binding a form

An <xref:Microsoft.AspNetCore.Components.Forms.EditForm> creates an <xref:Microsoft.AspNetCore.Components.Forms.EditContext> based on the assigned model instance as a [cascading value](xref:blazor/components/cascading-values-and-parameters) for other components in the form. The <xref:Microsoft.AspNetCore.Components.Forms.EditContext> tracks metadata about the edit process, including which fields have been modified and the current validation messages. Assigning to either an <xref:Microsoft.AspNetCore.Components.Forms.EditForm.Model?displayProperty=nameWithType> or an <xref:Microsoft.AspNetCore.Components.Forms.EditForm.EditContext?displayProperty=nameWithType> can bind a form to data.

::: moniker range=">= aspnetcore-5.0"

Assignment to <xref:Microsoft.AspNetCore.Components.Forms.EditForm.Model?displayProperty=nameWithType>:

```razor
<EditForm Model="@exampleModel" ...>

@code {
    private ExampleModel exampleModel = new() { ... };
}
```

Assignment to <xref:Microsoft.AspNetCore.Components.Forms.EditForm.EditContext?displayProperty=nameWithType>:

```razor
<EditForm EditContext="@editContext" ...>

@code {
    private ExampleModel exampleModel = new() { ... };
    private EditContext editContext;

    protected override void OnInitialized()
    {
        editContext = new(exampleModel);
    }
}
```

::: moniker-end

::: moniker range="< aspnetcore-5.0"

Assignment to <xref:Microsoft.AspNetCore.Components.Forms.EditForm.Model?displayProperty=nameWithType>:

```razor
<EditForm Model="@exampleModel" ...>

@code {
    private ExampleModel exampleModel = new ExampleModel() { ... };
}
```

Assignment to <xref:Microsoft.AspNetCore.Components.Forms.EditForm.EditContext?displayProperty=nameWithType>:

```razor
<EditForm EditContext="@editContext" ...>

@code {
    private ExampleModel exampleModel = new ExampleModel() { ... };
    private EditContext editContext;

    protected override void OnInitialized()
    {
        editContext = new EditContext(exampleModel);
    }
}
```

::: moniker-end

Assign **either** an <xref:Microsoft.AspNetCore.Components.Forms.EditForm.EditContext> **or** a <xref:Microsoft.AspNetCore.Components.Forms.EditForm.Model> to an <xref:Microsoft.AspNetCore.Components.Forms.EditForm>. Assignment of both isn't supported and generates a runtime error:

> Unhandled exception rendering component: EditForm requires a Model parameter, or an EditContext parameter, but not both.

## Handle form submission

The <xref:Microsoft.AspNetCore.Components.Forms.EditForm> provides the following callbacks for handling form submission:

* Use <xref:Microsoft.AspNetCore.Components.Forms.EditForm.OnValidSubmit> to assign an event handler to run when a form with valid fields is submitted.
* Use <xref:Microsoft.AspNetCore.Components.Forms.EditForm.OnInvalidSubmit> to assign an event handler to run when a form with invalid fields is submitted.
* Use <xref:Microsoft.AspNetCore.Components.Forms.EditForm.OnSubmit> to assign an event handler to run regardless of the form fields' validation status. The form is validated by calling <xref:Microsoft.AspNetCore.Components.Forms.EditContext.Validate%2A?displayProperty=nameWithType> in the event handler method. If <xref:Microsoft.AspNetCore.Components.Forms.EditContext.Validate%2A> returns `true`, the form is valid.

## Built-in form components

The Blazor framework provides built-in form components to receive and validate user input. Inputs are validated when they're changed and when a form is submitted. Available input components are shown in the following table.

::: moniker range=">= aspnetcore-5.0"

| Input component | Rendered as&hellip; |
| --------------- | ------------------- |
| <xref:Microsoft.AspNetCore.Components.Forms.InputCheckbox> | `<input type="checkbox">` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputDate%601> | `<input type="date">` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputFile> | `<input type="file">` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputNumber%601> | `<input type="number">` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputRadio%601> | `<input type="radio">` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputRadioGroup%601> | Group of child <xref:Microsoft.AspNetCore.Components.Forms.InputRadio%601> |
| <xref:Microsoft.AspNetCore.Components.Forms.InputSelect%601> | `<select>` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputText> | `<input>` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputTextArea> | `<textarea>` |

For more information on the <xref:Microsoft.AspNetCore.Components.Forms.InputFile> component, see <xref:blazor/file-uploads>.

::: moniker-end

::: moniker range="< aspnetcore-5.0"

| Input component | Rendered as&hellip; |
| --------------- | ------------------- |
| <xref:Microsoft.AspNetCore.Components.Forms.InputCheckbox> | `<input type="checkbox">` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputDate%601> | `<input type="date">` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputNumber%601> | `<input type="number">` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputSelect%601> | `<select>` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputText> | `<input>` |
| <xref:Microsoft.AspNetCore.Components.Forms.InputTextArea> | `<textarea>` |

> [!NOTE]
> <xref:Microsoft.AspNetCore.Components.Forms.InputRadio%601> and <xref:Microsoft.AspNetCore.Components.Forms.InputRadioGroup%601> components are available in ASP.NET Core 5.0 or later. For more information, select a 5.0 or later version of this article.

::: moniker-end

All of the input components, including <xref:Microsoft.AspNetCore.Components.Forms.EditForm>, support arbitrary attributes. Any attribute that doesn't match a component parameter is added to the rendered HTML element.

Input components provide default behavior for validating when a field is changed, including updating the field CSS class to reflect the field's state as valid or invalid. Some components include useful parsing logic. For example, <xref:Microsoft.AspNetCore.Components.Forms.InputDate%601> and <xref:Microsoft.AspNetCore.Components.Forms.InputNumber%601> handle unparseable values gracefully by registering unparseable values as validation errors. Types that can accept null values also support nullability of the target field (for example, `int?` for a nullable integer).

## Example form

The following `Starship` type, which is used in several of this article's examples, defines a diverse set of properties with data annotations:

* `Identifier` is required because it's annotated with the <xref:System.ComponentModel.DataAnnotations.RequiredAttribute>. `Identifier` requires a value of at least one character but no more than 16 characters using the <xref:System.ComponentModel.DataAnnotations.StringLengthAttribute>.
* `Description` is optional because it isn't annotated with the <xref:System.ComponentModel.DataAnnotations.RequiredAttribute>.
* `Classification` is required.
* The `MaximumAccommodation` property defaults to zero but requires a value from one to 100,000 per its <xref:System.ComponentModel.DataAnnotations.RangeAttribute>.
* `IsValidatedDesign` requires that the property have a `true` value, which matches a selected state when the property is bound to a checkbox in the UI (`<input type="checkbox">`).
* `ProductionDate` is a <xref:System.DateTime> and required.

`Starship.cs`:

::: moniker range=">= aspnetcore-5.0"

[!code-csharp[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Starship.cs)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-csharp[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Starship.cs)]

::: moniker-end

The following form accepts and validates user input using:

* The properties and validation defined in the preceding `Starship` model.
* Several of Blazor's [built-in form components](#built-in-form-components).

`Pages/FormExample2.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample2.razor)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample2.razor)]

::: moniker-end

The <xref:Microsoft.AspNetCore.Components.Forms.EditForm> in the preceding example creates an <xref:Microsoft.AspNetCore.Components.Forms.EditContext> based on the assigned `Starship` instance (`Model="@starship"`) and handles a valid form. The next example (`FormExample3` component) demonstrates how to assign an <xref:Microsoft.AspNetCore.Components.Forms.EditContext> to a form and validate when the form is submitted.

In the following example:

* A shortened version of the preceding `Starfleet Starship Database` form (`FormExample2` component) is used that only accepts a value for the starship's identifier. The other `Starship` properties receive valid default values when an instance of the `Starship` type is created.
* The `HandleSubmit` method executes when the **`Submit`** button is selected.
* The form is validated by calling <xref:Microsoft.AspNetCore.Components.Forms.EditContext.Validate%2A?displayProperty=nameWithType> in the `HandleSubmit` method.
* Logging is executed depending on the validation result.

> [!NOTE]
> `HandleSubmit` in the `FormExample3` component is demonstrated as an asynchronous method because storing form values often uses asynchronous calls (`await ...`). If the form is used in a test app as shown, `HandleSubmit` merely runs synchronously. For testing purposes, ignore the following build warning:
>
> > This async method lacks 'await' operators and will run synchronously. ...

`Pages/FormExample3.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample3.razor?highlight=5,39,44)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample3.razor?highlight=5,39,44)]

::: moniker-end

> [!NOTE]
> Changing the <xref:Microsoft.AspNetCore.Components.Forms.EditContext> after its assigned is **not** supported.

::: moniker range=">= aspnetcore-6.0"

## Multiple option selection with the `InputSelect` component

Binding supports [`multiple`](https://developer.mozilla.org/docs/Web/HTML/Attributes/multiple) option selection with the <xref:Microsoft.AspNetCore.Components.Forms.InputSelect%601> component. The [`@onchange`](xref:mvc/views/razor#onevent) event provides an array of the selected options via [event arguments (`ChangeEventArgs`)](xref:blazor/components/event-handling#event-arguments). The value must be bound to an array type, and binding to an array type makes the [`multiple`](https://developer.mozilla.org/docs/Web/HTML/Attributes/multiple) attribute optional on the <xref:Microsoft.AspNetCore.Components.Forms.InputSelect%601> tag.

In the following example, the user must select at least two starship classifications but no more than three classifications.

`Pages/BindMultipleWithInputSelect.razor`:

```razor
@page "/bind-multiple-with-inputselect"
@using System.ComponentModel.DataAnnotations 
@using Microsoft.Extensions.Logging
@inject ILogger<BindMultipleWithInputSelect> Logger 

<h1>Bind Multiple <code>InputSelect</code>Example</h1>

<EditForm EditContext="@editContext" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <p>
        <label>
            Select one or more classifications (Minimum: 2, Maximum: 3):
            <InputSelect @bind-Value="starship.SelectedClassification">
                <option value="@Classification.Exploration">Exploration</option>
                <option value="@Classification.Diplomacy">Diplomacy</option>
                <option value="@Classification.Defense">Defense</option>
                <option value="@Classification.Research">Research</option>
            </InputSelect>
        </label>
    </p>

    <button type="submit">Submit</button>
</EditForm>

<p>
    Selected Classifications: 
    @string.Join(", ", starship.SelectedClassification)
</p>

@code {
    private EditContext editContext;
    private Starship starship = new();

    protected override void OnInitialized()
    {
        editContext = new(starship);
    }

    private void HandleValidSubmit()
    {
        Logger.LogInformation("HandleValidSubmit called");
    }

    private class Starship
    {
        [Required, MinLength(2), MaxLength(3)]
        public Classification[] SelectedClassification { get; set; } =
            new[] { Classification.Diplomacy };
    }

    private enum Classification { Exploration, Diplomacy, Defense, Research }
}
```

For information on how empty strings and `null` values are handled in data binding, see the [Binding `InputSelect` options to C# object `null` values](#binding-inputselect-options-to-c-object-null-values) section.

::: moniker-end

## Binding `InputSelect` options to C# object `null` values

For information on how empty strings and `null` values are handled in data binding, see <xref:blazor/components/data-binding#binding-select-element-options-to-c-object-null-values>.

::: moniker range=">= aspnetcore-5.0"

## Display name support

Several built-in components support display names with the <xref:Microsoft.AspNetCore.Components.Forms.InputBase%601.DisplayName%2A?displayProperty=nameWithType> parameter.

In the `Starfleet Starship Database` form (`FormExample2` component) of the [Example form](#example-form) section, the production date of a new starship doesn't specify a display name:

```razor
<label>
    Production Date:
    <InputDate @bind-Value="starship.ProductionDate" />
</label>
```

If the field contains an invalid date when the form is submitted, the error message doesn't display a friendly name. The field name, "`ProductionDate`" doesn't have a space between "`Production`" and "`Date`" when it appears in the validation summary:

> The ProductionDate field must be a date.

Set the <xref:Microsoft.AspNetCore.Components.Forms.InputBase%601.DisplayName%2A> property to a friendly name with a space between the words "`Production`" and "`Date`":

```razor
<label>
    Production Date:
    <InputDate @bind-Value="starship.ProductionDate" 
               DisplayName="Production Date" />
</label>
```

The validation summary displays the friendly name when the field's value is invalid:

> The Production Date field must be a date.

::: moniker-end

## Error message template support

::: moniker range=">= aspnetcore-5.0"

<xref:Microsoft.AspNetCore.Components.Forms.InputDate%601> and <xref:Microsoft.AspNetCore.Components.Forms.InputNumber%601> support error message templates:

* <xref:Microsoft.AspNetCore.Components.Forms.InputDate%601.ParsingErrorMessage%2A?displayProperty=nameWithType>
* <xref:Microsoft.AspNetCore.Components.Forms.InputNumber%601.ParsingErrorMessage%2A?displayProperty=nameWithType>

In the `Starfleet Starship Database` form (`FormExample2` component) of the [Example form](#example-form) section with a [friendly display name](#display-name-support) assigned, the `Production Date` field produces an error message using the following default error message template:

```css
The {0} field must be a date.
```

The position of the `{0}` placeholder is where the value of the <xref:Microsoft.AspNetCore.Components.Forms.InputBase%601.DisplayName%2A> property appears when the error is displayed to the user.

```razor
<label>
    Production Date:
    <InputDate @bind-Value="starship.ProductionDate" 
               DisplayName="Production Date" />
</label>
```

> The Production Date field must be a date.

Assign a custom template to <xref:Microsoft.AspNetCore.Components.Forms.InputDate%601.ParsingErrorMessage%2A> to provide a custom message:

```razor
<label>
    Production Date:
    <InputDate @bind-Value="starship.ProductionDate" 
               DisplayName="Production Date" 
               ParsingErrorMessage="The {0} field has an incorrect date value." />
</label>
```

> The Production Date field has an incorrect date value.

::: moniker-end

::: moniker range="< aspnetcore-5.0"

<xref:Microsoft.AspNetCore.Components.Forms.InputDate%601> and <xref:Microsoft.AspNetCore.Components.Forms.InputNumber%601> support error message templates:

* <xref:Microsoft.AspNetCore.Components.Forms.InputDate%601.ParsingErrorMessage%2A?displayProperty=nameWithType>
* <xref:Microsoft.AspNetCore.Components.Forms.InputNumber%601.ParsingErrorMessage%2A?displayProperty=nameWithType>

In the `Starfleet Starship Database` form (`FormExample2` component) of the [Example form](#example-form) section uses a default error message template:

```css
The {0} field must be a date.
```

The position of the `{0}` placeholder is where the value of the <xref:Microsoft.AspNetCore.Components.Forms.InputBase%601.DisplayName%2A> property appears when the error is displayed to the user.

```razor
<label>
    Production Date:
    <InputDate @bind-Value="starship.ProductionDate" />
</label>
```

> The ProductionDate field must be a date.

Assign a custom template to <xref:Microsoft.AspNetCore.Components.Forms.InputDate%601.ParsingErrorMessage%2A> to provide a custom message:

```razor
<label>
    Production Date:
    <InputDate @bind-Value="starship.ProductionDate" 
               ParsingErrorMessage="The {0} field has an incorrect date value." />
</label>
```

> The ProductionDate field has an incorrect date value.

::: moniker-end

## Basic validation

In basic form validation scenarios, an <xref:Microsoft.AspNetCore.Components.Forms.EditForm> instance can use declared <xref:Microsoft.AspNetCore.Components.Forms.EditContext> and <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessageStore> instances to validate form fields. A handler for the <xref:Microsoft.AspNetCore.Components.Forms.EditContext.OnValidationRequested> event of the <xref:Microsoft.AspNetCore.Components.Forms.EditContext> executes custom validation logic. The handler's result updates the <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessageStore> instance.

Basic form validation is useful in cases where the form's model is defined within the component hosting the form, either as members directly on the component or in a subclass. Use of a [validator component](#validator-components) is recommended where an independent model class is used across several components.

In the following `FormExample4` component, the `HandleValidationRequested` handler method clears any existing validation messages by calling <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessageStore.Clear%2A?displayProperty=nameWithType> before validating the form.

`Pages/FormExample4.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample4.razor?highlight=38,42-53,70)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample4.razor?highlight=38,42-53,70)]

::: moniker-end

## Data Annotations Validator component and custom validation

The <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component attaches data annotations validation to a cascaded <xref:Microsoft.AspNetCore.Components.Forms.EditContext>. Enabling data annotations validation requires the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component. To use a different validation system than data annotations, use a custom implementation instead of the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component. The framework implementations for <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> are available for inspection in the reference source:

* [`DataAnnotationsValidator`](https://github.com/dotnet/AspNetCore/blob/main/src/Components/Forms/src/DataAnnotationsValidator.cs)
* [`AddDataAnnotationsValidation`](https://github.com/dotnet/AspNetCore/blob/main/src/Components/Forms/src/EditContextDataAnnotationsExtensions.cs).

[!INCLUDE[](~/blazor/includes/aspnetcore-repo-ref-source-links.md)]

Blazor performs two types of validation:

* *Field validation* is performed when the user tabs out of a field. During field validation, the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component associates all reported validation results with the field.
* *Model validation* is performed when the user submits the form. During model validation, the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component attempts to determine the field based on the member name that the validation result reports. Validation results that aren't associated with an individual member are associated with the model rather than a field.

## Validator components

Validator components support form validation by managing a <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessageStore> for a form's <xref:Microsoft.AspNetCore.Components.Forms.EditContext>.

The Blazor framework provides the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component to attach validation support to forms based on [validation attributes (data annotations)](xref:mvc/models/validation#validation-attributes). You can create custom validator components to process validation messages for different forms on the same page or the same form at different steps of form processing (for example, client-side validation followed by server-side validation). The validator component example shown in this section, `CustomValidation`, is used in the following sections of this article:

* [Business logic validation with a validator component](#business-logic-validation-with-a-validator-component)
* [Server validation with a validator component](#server-validation-with-a-validator-component)

> [!NOTE]
> Custom data annotation validation attributes can be used instead of custom validator components in many cases. Custom attributes applied to the form's model activate with the use of the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component. When used with server-side validation, any custom attributes applied to the model must be executable on the server. For more information, see <xref:mvc/models/validation#alternatives-to-built-in-attributes>.

Create a validator component from <xref:Microsoft.AspNetCore.Components.ComponentBase>:

* The form's <xref:Microsoft.AspNetCore.Components.Forms.EditContext> is a [cascading parameter](xref:blazor/components/cascading-values-and-parameters) of the component.
* When the validator component is initialized, a new <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessageStore> is created to maintain a current list of form errors.
* The message store receives errors when developer code in the form's component calls the `DisplayErrors` method. The errors are passed to the `DisplayErrors` method in a [`Dictionary<string, List<string>>`](xref:System.Collections.Generic.Dictionary`2). In the dictionary, the key is the name of the form field that has one or more errors. The value is the error list.
* Messages are cleared when any of the following have occurred:
  * Validation is requested on the <xref:Microsoft.AspNetCore.Components.Forms.EditContext> when the <xref:Microsoft.AspNetCore.Components.Forms.EditContext.OnValidationRequested> event is raised. All of the errors are cleared.
  * A field changes in the form when the <xref:Microsoft.AspNetCore.Components.Forms.EditContext.OnFieldChanged> event is raised. Only the errors for the field are cleared.
  * The `ClearErrors` method is called by developer code. All of the errors are cleared.

`CustomValidation.cs` (if used in a test app, change the namespace, `BlazorSample`, to match the app's namespace):

::: moniker range=">= aspnetcore-5.0"

[!code-csharp[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/CustomValidation.cs)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-csharp[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/CustomValidation.cs)]

::: moniker-end

> [!IMPORTANT]
> Specifying a namespace is **required** when deriving from <xref:Microsoft.AspNetCore.Components.ComponentBase>. Failing to specify a namespace results in a build error:
>
> > Tag helpers cannot target tag name '\<global namespace>.{CLASS NAME}' because it contains a ' ' character.
>
> The `{CLASS NAME}` placeholder is the name of the component class. The custom validator example in this section specifies the example namespace `BlazorSample`.

> [!NOTE]
> Anonymous lambda expressions are registered event handlers for <xref:Microsoft.AspNetCore.Components.Forms.EditContext.OnValidationRequested> and <xref:Microsoft.AspNetCore.Components.Forms.EditContext.OnFieldChanged> in the preceding example. It isn't necessary to implement <xref:System.IDisposable> and unsubscribe the event delegates in this scenario. For more information, see <xref:blazor/components/lifecycle#component-disposal-with-idisposable-and-iasyncdisposable>.

## Business logic validation with a validator component

For general business logic validation, use a [validator component](#validator-components) that receives form errors in a dictionary.

[Basic validation](#basic-validation) is useful in cases where the form's model is defined within the component hosting the form, either as members directly on the component or in a subclass. Use of a validator component is recommended where an independent model class is used across several components.

In the following example:

* A shortened version of the `Starfleet Starship Database` form (`FormExample2` component) from the [Example form](#example-form) section is used that only accepts the starship's classification and description. Data annotation validation is **not** triggered on form submission because the `DataAnnotationsValidator` component isn't included in the form.
* The `CustomValidation` component from the [Validator components](#validator-components) section of this article is used.
* The validation requires a value for the ship's description (`Description`) if the user selects the "`Defense`" ship classification (`Classification`).

When validation messages are set in the component, they're added to the validator's <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessageStore> and shown in the <xref:Microsoft.AspNetCore.Components.Forms.EditForm>'s validation summary.

`Pages/FormExample5.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample5.razor)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample5.razor)]

::: moniker-end

> [!NOTE]
> As an alternative to using [validation components](#validator-components), data annotation validation attributes can be used. Custom attributes applied to the form's model activate with the use of the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component. When used with server-side validation, the attributes must be executable on the server. For more information, see <xref:mvc/models/validation#alternatives-to-built-in-attributes>.

## Server validation with a validator component

Server validation is supported in addition to client-side validation:

* Process client-side validation in the form with the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component.
* When the form passes client-side validation (<xref:Microsoft.AspNetCore.Components.Forms.EditForm.OnValidSubmit> is called), send the <xref:Microsoft.AspNetCore.Components.Forms.EditContext.Model?displayProperty=nameWithType> to a backend server API for form processing.
* Process model validation on the server.
* The server API includes both the built-in framework data annotations validation and custom validation logic supplied by the developer. If validation passes on the server, process the form and send back a success status code ([`200 - OK`](https://developer.mozilla.org/docs/Web/HTTP/Status/200)). If validation fails, return a failure status code ([`400 - Bad Request`](https://developer.mozilla.org/docs/Web/HTTP/Status/400)) and the field validation errors.
* Either disable the form on success or display the errors.

[Basic validation](#basic-validation) is useful in cases where the form's model is defined within the component hosting the form, either as members directly on the component or in a subclass. Use of a validator component is recommended where an independent model class is used across several components.

The following example is based on:

* A hosted Blazor WebAssembly solution created from the [Blazor WebAssembly project template](xref:blazor/project-structure). The approach is supported for any of the secure hosted Blazor solutions described in the [hosted Blazor WebAssembly security documentation](xref:blazor/security/webassembly/index#implementation-guidance).
* The `Starship` model  (`Starship.cs`) from the [Example form](#example-form) section.
* The `CustomValidation` component shown in the [Validator components](#validator-components) section.

Place the `Starship` model (`Starship.cs`) into the solution's **`Shared`** project so that both the client and server apps can use the model. Add or update the namespace to match the namespace of the shared app (for example, `namespace BlazorSample.Shared`). Since the model requires data annotations, add a package reference for [`System.ComponentModel.Annotations`](https://www.nuget.org/packages/System.ComponentModel.Annotations) to the **`Shared`** project's project file:

```xml
<ItemGroup>
  <PackageReference Include="System.ComponentModel.Annotations" Version="{VERSION}" />
</ItemGroup>
```

To determine the latest non-preview version of the package for the `{VERSION}` placeholder, see the package **Version History** at [NuGet.org for `System.ComponentModel.Annotations`](https://www.nuget.org/packages/System.ComponentModel.Annotations).

In the **`Server`** project, add a controller to process starship validation requests and return failed validation messages. Update the namespaces in the last `using` statement for the **`Shared`** project and the `namespace` for the controller class. In addition to data annotations validation (client-side and server-side), the controller validates that a value is provided for the ship's description (`Description`) if the user selects the `Defense` ship classification (`Classification`).

The validation for the `Defense` ship classification only occurs server-side in the controller because the upcoming form doesn't perform the same validation client-side when the form is submitted to the server. Server-side validation without client-side validation is common in apps that require private business logic validation of user input on the server. For example, private information from data stored for a user might be required to validate user input. Private data obviously can't be sent to the client for client-side validation.

> [!NOTE]
> The `StarshipValidation` controller in this section uses Microsoft Identity 2.0. The Web API only accepts tokens for users that have the "`API.Access`" scope for this API. Additional customization is required if the API's scope name is different from `API.Access`. For a version of the controller that works with Microsoft Identity 1.0 and ASP.NET Core prior to version 5.0, see an earlier version of this article.
>
> For more information on security, see:
>
> * <xref:blazor/security/index> (and the other articles in the Blazor *Security and Identity* node)
> * [Microsoft identity platform documentation](/azure/active-directory/develop/)

`Controllers/StarshipValidation.cs`:

::: moniker range=">= aspnetcore-5.0"

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using BlazorSample.Shared;

namespace BlazorSample.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StarshipValidationController : ControllerBase
    {
        private readonly ILogger<StarshipValidationController> logger;

        public StarshipValidationController(
            ILogger<StarshipValidationController> logger)
        {
            this.logger = logger;
        }

        static readonly string[] scopeRequiredByApi = new[] { "API.Access" };

        [HttpPost]
        public async Task<IActionResult> Post(Starship starship)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            try
            {
                if (starship.Classification == "Defense" && 
                    string.IsNullOrEmpty(starship.Description))
                {
                    ModelState.AddModelError(nameof(starship.Description),
                        "For a 'Defense' ship " +
                        "classification, 'Description' is required.");
                }
                else
                {
                    logger.LogInformation("Processing the form asynchronously");

                    // Process the valid form
                    // async ...

                    return Ok(ModelState);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Validation Error: {Message}", ex.Message);
            }

            return BadRequest(ModelState);
        }
    }
}
```

::: moniker-end

::: moniker range="< aspnetcore-5.0"

> [!NOTE]
> The `StarshipValidation` controller in this section is appropriate for use with Microsoft Identity 1.0. Additional configuration is required for use with Microsoft Identity 2.0 and ASP.NET Core 5.0 or later. To see a version of the controller for updated versions of these technologies, see a later version of this article.
>
> For more information on security, see:
>
> * <xref:blazor/security/index> (and the other articles in the *Blazor Security and Identity* node)
> * [Microsoft identity platform documentation](/azure/active-directory/develop/)

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using BlazorSample.Shared;

namespace BlazorSample.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StarshipValidationController : ControllerBase
    {
        private readonly ILogger<StarshipValidationController> logger;

        public StarshipValidationController(
            ILogger<StarshipValidationController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Starship starship)
        {
            try
            {
                if (starship.Classification == "Defense" && 
                    string.IsNullOrEmpty(starship.Description))
                {
                    ModelState.AddModelError(nameof(starship.Description),
                        "For a 'Defense' ship " +
                        "classification, 'Description' is required.");
                }
                else
                {
                    logger.LogInformation("Processing the form asynchronously");

                    // Process the valid form
                    // async ...

                    return Ok(ModelState);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Validation Error: {Message}", ex.Message);
            }

            return BadRequest(ModelState);
        }
    }
}
```

::: moniker-end

If using the preceding controller in a hosted Blazor WebAssembly app, update the namespace (`BlazorSample.Server.Controllers`) to match the app's controllers namespace.

When a model binding validation error occurs on the server, an [`ApiController`](xref:web-api/index) (<xref:Microsoft.AspNetCore.Mvc.ApiControllerAttribute>) normally returns a [default bad request response](xref:web-api/index#default-badrequest-response) with a <xref:Microsoft.AspNetCore.Mvc.ValidationProblemDetails>. The response contains more data than just the validation errors, as shown in the following example when all of the fields of the `Starfleet Starship Database` form aren't submitted and the form fails validation:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Identifier": ["The Identifier field is required."],
    "Classification": ["The Classification field is required."],
    "IsValidatedDesign": ["This form disallows unapproved ships."],
    "MaximumAccommodation": ["Accommodation invalid (1-100000)."]
  }
}
```

> [!NOTE]
> To demonstrate the preceding JSON response, you must either disable the form's client-side validation to permit empty field form submission or use a tool to send a request directly to the server API, such as [Fiddler](https://www.telerik.com/fiddler), [Firefox Browser Developer](https://www.mozilla.org/firefox/developer/), or [Postman](https://www.postman.com).

If the server API returns the preceding default JSON response, it's possible for the client to parse the response in developer code to obtain the children of the `errors` node for forms validation error processing. It's inconvenient to write developer code to parse the file. Parsing the JSON manually requires producing a [`Dictionary<string, List<string>>`](xref:System.Collections.Generic.Dictionary`2) of errors after calling <xref:System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync%2A>. Ideally, the server API should only return the validation errors:

```json
{
  "Identifier": ["The Identifier field is required."],
  "Classification": ["The Classification field is required."],
  "IsValidatedDesign": ["This form disallows unapproved ships."],
  "MaximumAccommodation": ["Accommodation invalid (1-100000)."]
}
```

To modify the server API's response to make it only return the validation errors, change the delegate that's invoked on actions that are annotated with <xref:Microsoft.AspNetCore.Mvc.ApiControllerAttribute> in `Startup.ConfigureServices`. For the API endpoint (`/StarshipValidation`), return a <xref:Microsoft.AspNetCore.Mvc.BadRequestObjectResult> with the <xref:Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary>. For any other API endpoints, preserve the default behavior by returning the object result with a new <xref:Microsoft.AspNetCore.Mvc.ValidationProblemDetails>.

Add the <xref:Microsoft.AspNetCore.Mvc?displayProperty=fullName> namespace to the top of the `Startup.cs` file in the **`Server`** app:

```csharp
using Microsoft.AspNetCore.Mvc;
```

In `Startup.ConfigureServices`, locate the <xref:Microsoft.Extensions.DependencyInjection.MvcServiceCollectionExtensions.AddControllersWithViews%2A> extension method and add the following call to <xref:Microsoft.Extensions.DependencyInjection.MvcCoreMvcBuilderExtensions.ConfigureApiBehaviorOptions%2A>:

```csharp
services.AddControllersWithViews()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            if (context.HttpContext.Request.Path == "/StarshipValidation")
            {
                return new BadRequestObjectResult(context.ModelState);
            }
            else
            {
                return new BadRequestObjectResult(
                    new ValidationProblemDetails(context.ModelState));
            }
        };
    });
```

For more information, see <xref:web-api/handle-errors#validation-failure-error-response>.

In the **`Client`** project, add the `CustomValidation` component shown in the [Validator components](#validator-components) section. Update the namespace to match the app (for example, `namespace BlazorSample.Client`).

In the **`Client`** project, the `Starfleet Starship Database` form is updated to show server validation errors with help of the `CustomValidation` component. When the server API returns validation messages, they're added to the `CustomValidation` component's <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessageStore>. The errors are available in the form's <xref:Microsoft.AspNetCore.Components.Forms.EditContext> for display by the form's validation summary.

In the following `FormExample6` component, update the namespace of the **`Shared`** project (`@using BlazorSample.Shared`) to the shared project's namespace. Note that the form requires authorization, so the user must be signed into the app to navigate to the form.

`Pages/FormExample6.razor`:

```razor
@page "/form-example-6"
@using System.Net
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.WebAssembly.Authentication
@using Microsoft.Extensions.Logging
@using BlazorSample.Shared
@attribute [Authorize]
@inject HttpClient Http
@inject ILogger<FormExample6> Logger

<h1>Starfleet Starship Database</h1>

<h2>New Ship Entry Form</h2>

<EditForm Model="@starship" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />
    <CustomValidation @ref="customValidation" />
    <ValidationSummary />

    <p>
        <label>
            Identifier:
            <InputText @bind-Value="starship.Identifier" disabled="@disabled" />
        </label>
    </p>
    <p>
        <label>
            Description (optional):
            <InputTextArea @bind-Value="starship.Description" 
                disabled="@disabled" />
        </label>
    </p>
    <p>
        <label>
            Primary Classification:
            <InputSelect @bind-Value="starship.Classification" disabled="@disabled">
                <option value="">Select classification ...</option>
                <option value="Exploration">Exploration</option>
                <option value="Diplomacy">Diplomacy</option>
                <option value="Defense">Defense</option>
            </InputSelect>
        </label>
    </p>
    <p>
        <label>
            Maximum Accommodation:
            <InputNumber @bind-Value="starship.MaximumAccommodation" 
                disabled="@disabled" />
        </label>
    </p>
    <p>
        <label>
            Engineering Approval:
            <InputCheckbox @bind-Value="starship.IsValidatedDesign" 
                disabled="@disabled" />
        </label>
    </p>
    <p>
        <label>
            Production Date:
            <InputDate @bind-Value="starship.ProductionDate" disabled="@disabled" />
        </label>
    </p>

    <button type="submit" disabled="@disabled">Submit</button>

    <p style="@messageStyles">
        @message
    </p>

    <p>
        <a href="http://www.startrek.com/">Star Trek</a>,
        &copy;1966-2019 CBS Studios, Inc. and
        <a href="https://www.paramount.com">Paramount Pictures</a>
    </p>
</EditForm>

@code {
    private bool disabled;
    private string message;
    private string messageStyles = "visibility:hidden";
    private CustomValidation customValidation;
    private Starship starship = new() { ProductionDate = DateTime.UtcNow };

    private async Task HandleValidSubmit(EditContext editContext)
    {
        customValidation.ClearErrors();

        try
        {
            var response = await Http.PostAsJsonAsync<Starship>(
                "StarshipValidation", (Starship)editContext.Model);

            var errors = await response.Content
                .ReadFromJsonAsync<Dictionary<string, List<string>>>();

            if (response.StatusCode == HttpStatusCode.BadRequest && 
                errors.Any())
            {
                customValidation.DisplayErrors(errors);
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Validation failed. Status Code: {response.StatusCode}");
            }
            else
            {
                disabled = true;
                messageStyles = "color:green";
                message = "The form has been processed.";
            }
        }
        catch (AccessTokenNotAvailableException ex)
        {
            ex.Redirect();
        }
        catch (Exception ex)
        {
            Logger.LogError("Form processing error: {Message}", ex.Message);
            disabled = true;
            messageStyles = "color:red";
            message = "There was an error processing the form.";
        }
    }
}
```

> [!NOTE]
> As an alternative to the use of a [validation component](#validator-components), data annotation validation attributes can be used. Custom attributes applied to the form's model activate with the use of the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component. When used with server-side validation, the attributes must be executable on the server. For more information, see <xref:mvc/models/validation#alternatives-to-built-in-attributes>.

> [!NOTE]
> The server-side validation approach in this section is suitable for any of the Blazor WebAssembly hosted solution examples in this documentation set:
>
> * [Azure Active Directory (AAD)](xref:blazor/security/webassembly/hosted-with-azure-active-directory)
> * [Azure Active Directory (AAD) B2C](xref:blazor/security/webassembly/hosted-with-azure-active-directory-b2c)
> * [Identity Server](xref:blazor/security/webassembly/hosted-with-identity-server)

## `InputText` based on the input event

Use the <xref:Microsoft.AspNetCore.Components.Forms.InputText> component to create a custom component that uses the [`input`](https://developer.mozilla.org/docs/Web/API/HTMLElement/input_event) event instead of the [`change`](https://developer.mozilla.org/docs/Web/API/HTMLElement/change_event) event. Use of the `input` event triggers field validation on each keystroke.

The following example uses the `ExampleModel` class.

`ExampleModel.cs`:

::: moniker range=">= aspnetcore-5.0"

[!code-csharp[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/ExampleModel.cs)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-csharp[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/ExampleModel.cs)]

::: moniker-end

The following `CustomInputText` component inherits the framework's `InputText` component and sets event binding to the [`oninput`](https://developer.mozilla.org/docs/Web/API/GlobalEventHandlers/oninput) event.

`Shared/CustomInputText.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Shared/forms-and-validation/CustomInputText.razor)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Shared/forms-and-validation/CustomInputText.razor)]

::: moniker-end

The `CustomInputText` component can be used anywhere <xref:Microsoft.AspNetCore.Components.Forms.InputText> is used. The following `FormExample7` component uses the shared `CustomInputText` component.

`Pages/FormExample7.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample7.razor?highlight=9)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample7.razor?highlight=9)]

::: moniker-end

## Radio buttons

::: moniker range=">= aspnetcore-5.0"

The example in this section is based on the `Starfleet Starship Database` form of the [Example form](#example-form) section of this article.

Add the following [`enum` types](/dotnet/csharp/language-reference/language-specification/enums) to the app. Create a new file to hold them or add them to the `Starship.cs` file.

```csharp
public class ComponentEnums
{
    public enum Manufacturer { SpaceX, NASA, ULA, VirginGalactic, Unknown }
    public enum Color { ImperialRed, SpacecruiserGreen, StarshipBlue, VoyagerOrange }
    public enum Engine { Ion, Plasma, Fusion, Warp }
}
```

Make the `enums` accessible to the:

* `Starship` model in `Starship.cs` (for example, `using static ComponentEnums;` if the `enums` class is named `ComponentEnums`).
* `Starfleet Starship Database` form (for example, `@using static ComponentEnums` if the enums class is named `ComponentEnums`).

Use <xref:Microsoft.AspNetCore.Components.Forms.InputRadio%601> components with the <xref:Microsoft.AspNetCore.Components.Forms.InputRadioGroup%601> component to create a radio button group. In the following example, properties are added to the `Starship` model described in the [Example form](#example-form) section:

```csharp
[Required]
[Range(typeof(Manufacturer), nameof(Manufacturer.SpaceX), 
    nameof(Manufacturer.VirginGalactic), ErrorMessage = "Pick a manufacturer.")]
public Manufacturer Manufacturer { get; set; } = Manufacturer.Unknown;

[Required, EnumDataType(typeof(Color))]
public Color? Color { get; set; } = null;

[Required, EnumDataType(typeof(Engine))]
public Engine? Engine { get; set; } = null;
```

Update the `Starfleet Starship Database` form (`FormExample2` component) from the [Example form](#example-form) section. Add the components to produce:

* A radio button group for the ship manufacturer.
* A nested radio button group for engine and ship color.

> [!NOTE]
> Nested radio button groups aren't often used in forms because they can result in a disorganized layout of form controls that may confuse users. However, there are cases when they make sense in UI design, such as in the following example that pairs recommendations for two user inputs, ship engine and ship color. One engine and one color are required by the form's validation. The form's layout uses nested <xref:Microsoft.AspNetCore.Components.Forms.InputRadioGroup%601>s to pair engine and color recommendations. However, the user can combine any engine with any color to submit the form.

```razor
<p>
    <InputRadioGroup @bind-Value="starship.Manufacturer">
        Manufacturer:
        <br>
        @foreach (var manufacturer in (Manufacturer[])Enum
            .GetValues(typeof(Manufacturer)))
        {
            <InputRadio Value="manufacturer" />
            <text>&nbsp;</text>@manufacturer<br>
        }
    </InputRadioGroup>
</p>
<p>
    Select one engine and one color. Recommendations are paired but any 
    combination of engine and color is allowed:<br>
    <InputRadioGroup Name="engine" @bind-Value="starship.Engine">
        <InputRadioGroup Name="color" @bind-Value="starship.Color">
            <InputRadio Name="engine" Value="Engine.Ion" />
            Engine: Ion<br>
            <InputRadio Name="color" Value="Color.ImperialRed" />
            Color: Imperial Red<br><br>
            <InputRadio Name="engine" Value="Engine.Plasma" />
            Engine: Plasma<br>
            <InputRadio Name="color" Value="Color.SpacecruiserGreen" />
            Color: Spacecruiser Green<br><br>
            <InputRadio Name="engine" Value="Engine.Fusion" />
            Engine: Fusion<br>
            <InputRadio Name="color" Value="Color.StarshipBlue" />
            Color: Starship Blue<br><br>
            <InputRadio Name="engine" Value="Engine.Warp" />
            Engine: Warp<br>
            <InputRadio Name="color" Value="Color.VoyagerOrange" />
            Color: Voyager Orange
        </InputRadioGroup>
    </InputRadioGroup>
</p>
```

> [!NOTE]
> If `Name` is omitted, <xref:Microsoft.AspNetCore.Components.Forms.InputRadio%601> components are grouped by their most recent ancestor.

::: moniker-end

::: moniker range="< aspnetcore-5.0"

When working with radio buttons in a form, data binding is handled differently than other elements because radio buttons are evaluated as a group. The value of each radio button is fixed, but the value of the radio button group is the value of the selected radio button. The following example shows how to:

* Handle data binding for a radio button group.
* Support validation using a custom <xref:Microsoft.AspNetCore.Components.Forms.InputRadio%601> component.

`Shared/InputRadio.razor`:

```razor
@using System.Globalization
@inherits InputBase<TValue>
@typeparam TValue

<input @attributes="AdditionalAttributes" type="radio" value="@SelectedValue" 
       checked="@(SelectedValue.Equals(Value))" @onchange="OnChange" />

@code {
    [Parameter]
    public TValue SelectedValue { get; set; }

    private void OnChange(ChangeEventArgs args)
    {
        CurrentValueAsString = args.Value.ToString();
    }

    protected override bool TryParseValueFromString(string value, 
        out TValue result, out string errorMessage)
    {
        var success = BindConverter.TryConvertTo<TValue>(
            value, CultureInfo.CurrentCulture, out var parsedValue);
        if (success)
        {
            result = parsedValue;
            errorMessage = null;

            return true;
        }
        else
        {
            result = default;
            errorMessage = $"{FieldIdentifier.FieldName} field isn't valid.";

            return false;
        }
    }
}
```

For more information on generic type parameters (`@typeparam`), see the following articles:

* <xref:mvc/views/razor#typeparam>
* <xref:blazor/components/index#generic-type-parameter-support>
* <xref:blazor/components/templated-components>

The following `RadioButtonExample` component uses the preceding `InputRadio` component to obtain and validate a rating from the user:

`Pages/RadioButtonExample.razor`:

```razor
@page "/radio-button-example"
@using System.ComponentModel.DataAnnotations
@using Microsoft.Extensions.Logging
@inject ILogger<RadioButtonExample> Logger

<h1>Radio Button Example</h1>

<EditForm Model="@model" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    @for (int i = 1; i <= 5; i++)
    {
        <label>
            <InputRadio name="rate" SelectedValue="@i" @bind-Value="model.Rating" />
            @i
        </label>
    }

    <button type="submit">Submit</button>
</EditForm>

<p>You chose: @model.Rating</p>

@code {
    private Model model = new();

    private void HandleValidSubmit()
    {
        Logger.LogInformation("HandleValidSubmit called");

        // Process the valid form
    }

    public class Model
    {
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
```

::: moniker-end

## Validation Summary and Validation Message components

The <xref:Microsoft.AspNetCore.Components.Forms.ValidationSummary> component summarizes all validation messages, which is similar to the [Validation Summary Tag Helper](xref:mvc/views/working-with-forms#the-validation-summary-tag-helper):

```razor
<ValidationSummary />
```

Output validation messages for a specific model with the `Model` parameter:
  
```razor
<ValidationSummary Model="@starship" />
```

The <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessage%601> component displays validation messages for a specific field, which is similar to the [Validation Message Tag Helper](xref:mvc/views/working-with-forms#the-validation-message-tag-helper). Specify the field for validation with the <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessage%601.For%2A> attribute and a lambda expression naming the model property:

```razor
<ValidationMessage For="@(() => starship.MaximumAccommodation)" />
```

The <xref:Microsoft.AspNetCore.Components.Forms.ValidationMessage%601> and <xref:Microsoft.AspNetCore.Components.Forms.ValidationSummary> components support arbitrary attributes. Any attribute that doesn't match a component parameter is added to the generated `<div>` or `<ul>` element.

Control the style of validation messages in the app's stylesheet (`wwwroot/css/app.css` or `wwwroot/css/site.css`). The default `validation-message` class sets the text color of validation messages to red:

```css
.validation-message {
    color: red;
}
```

## Custom validation attributes

To ensure that a validation result is correctly associated with a field when using a [custom validation attribute](xref:mvc/models/validation#custom-attributes), pass the validation context's <xref:System.ComponentModel.DataAnnotations.ValidationContext.MemberName> when creating the <xref:System.ComponentModel.DataAnnotations.ValidationResult>.

`CustomValidator.cs`:

```csharp
using System;
using System.ComponentModel.DataAnnotations;

public class CustomValidator : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, 
        ValidationContext validationContext)
    {
        ...

        return new ValidationResult("Validation message to user.",
            new[] { validationContext.MemberName });
    }
}
```

> [!NOTE]
> <xref:System.ComponentModel.DataAnnotations.ValidationContext.GetService%2A?displayProperty=nameWithType> is `null`. Injecting services for validation in the `IsValid` method isn't supported.

::: moniker range=">= aspnetcore-5.0"

## Custom validation CSS class attributes

Custom validation CSS class attributes are useful when integrating with CSS frameworks, such as [Bootstrap](https://getbootstrap.com/).

The following example uses the `ExampleModel` class.

`ExampleModel.cs`:

[!code-csharp[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/ExampleModel.cs)]

To specify custom validation CSS class attributes, start by providing CSS styles for custom validation. In the following example, valid (`validField`) and invalid (`invalidField`) styles are specified.

`wwwroot/css/app.css` (Blazor WebAssembly) or `wwwroot/css/site.css` (Blazor Server):

```css
.validField {
    border-color: lawngreen;
}

.invalidField {
    background-color: tomato;
}
```

Create a class derived from <xref:Microsoft.AspNetCore.Components.Forms.FieldCssClassProvider> that checks for field validation messages and applies the appropriate valid or invalid style.

`CustomFieldClassProvider.cs`:

[!code-csharp[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/CustomFieldClassProvider.cs?highlight=11)]

Set the `CustomFieldClassProvider` class as the Field CSS Class Provider on the form's <xref:Microsoft.AspNetCore.Components.Forms.EditContext> instance with <xref:Microsoft.AspNetCore.Components.Forms.EditContextFieldClassExtensions.SetFieldCssClassProvider%2A>.

`Pages/FormExample8.razor`:

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample8.razor?highlight=21)]

The preceding example checks the validity of all form fields and applies a style to each field. If the form should only apply custom styles to a subset of the fields, make `CustomFieldClassProvider` apply styles conditionally. The following `CustomFieldClassProvider2` example only applies a style to the `Name` field. For any fields with names not matching `Name`, `string.Empty` is returned, and no style is applied.

`CustomFieldClassProvider2.cs`:

[!code-csharp[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/CustomFieldClassProvider2.cs?highlight=9,16)]

Add an additional property to `ExampleModel`, for example:

```csharp
[StringLength(10, ErrorMessage = "Description is too long.")]
public string Description { get; set; } 
```

Add the `Description` to the `ExampleForm7` component's form:

```razor
<InputText id="description" @bind-Value="exampleModel.Description" />
```

Update the `EditContext` instance in the component's `OnInitialized` method to use the new Field CSS Class Provider:

```csharp
editContext.SetFieldCssClassProvider(new CustomFieldClassProvider2());
```

Because a CSS validation class isn't applied to the `Description` field (`id="description"`), it isn't styled. However, field validation runs normally. If more than 10 characters are provided, the validation summary indicates the error:

> Description is too long.

In the following example:

* The custom CSS style is applied to the `Name` field.
* Any other fields apply logic similar to Blazor's default logic and using Blazor's default field CSS validation styles, `modified` with `valid` or `invalid`. Note that for the default styles, you don't need to add them to the app's stylesheet if the app is based on a Blazor project template. For apps not based on a Blazor project template, the default styles can be added to the app's stylesheet:

  ```css
  .valid.modified:not([type=checkbox]) {
      outline: 1px solid #26b050;
  }

  .invalid {
      outline: 1px solid red;
  }
  ```

`CustomFieldClassProvider3.cs`:

[!code-csharp[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/CustomFieldClassProvider3.cs?highlight=17-24)]

Update the `EditContext` instance in the component's `OnInitialized` method to use the preceding Field CSS Class Provider:

```csharp
editContext.SetFieldCssClassProvider(new CustomFieldClassProvider3());
```

Using `CustomFieldClassProvider3`:

* The `Name` field uses the app's custom validation CSS styles.
* The `Description` field uses logic similar to Blazor's logic and Blazor's default field CSS validation styles.

::: moniker-end

## Blazor data annotations validation package

The [`Microsoft.AspNetCore.Components.DataAnnotations.Validation`](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.DataAnnotations.Validation) is a package that fills validation experience gaps using the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component. The package is currently *experimental*.

> [!NOTE]
> The [`Microsoft.AspNetCore.Components.DataAnnotations.Validation`](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.DataAnnotations.Validation) package has a latest version of *release candidate* at [Nuget.org](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.DataAnnotations.Validation). Continue to use the *experimental* release candidate package at this time. The package's assembly might be moved to either the framework or the runtime in a future release. Watch the [Announcements GitHub repository](https://github.com/aspnet/Announcements), the [dotnet/aspnetcore GitHub repository](https://github.com/dotnet/aspnetcore), or this topic section for further updates.

::: moniker range="< aspnetcore-5.0"

## `[CompareProperty]` attribute

The <xref:System.ComponentModel.DataAnnotations.CompareAttribute> doesn't work well with the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> component because it doesn't associate the validation result with a specific member. This can result in inconsistent behavior between field-level validation and when the entire model is validated on a submit. The [`Microsoft.AspNetCore.Components.DataAnnotations.Validation`](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.DataAnnotations.Validation) *experimental* package introduces an additional validation attribute, `ComparePropertyAttribute`, that works around these limitations. In a Blazor app, `[CompareProperty]` is a direct replacement for the [`[Compare]` attribute](xref:System.ComponentModel.DataAnnotations.CompareAttribute).

::: moniker-end

## Nested models, collection types, and complex types

Blazor provides support for validating form input using data annotations with the built-in <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator>. However, the <xref:Microsoft.AspNetCore.Components.Forms.DataAnnotationsValidator> only validates top-level properties of the model bound to the form that aren't collection- or complex-type properties.

To validate the bound model's entire object graph, including collection- and complex-type properties, use the `ObjectGraphDataAnnotationsValidator` provided by the *experimental* [`Microsoft.AspNetCore.Components.DataAnnotations.Validation`](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.DataAnnotations.Validation) package:

```razor
<EditForm Model="@model" OnValidSubmit="@HandleValidSubmit">
    <ObjectGraphDataAnnotationsValidator />
    ...
</EditForm>
```

Annotate model properties with `[ValidateComplexType]`. In the following model classes, the `ShipDescription` class contains additional data annotations to validate when the model is bound to the form:

`Starship.cs`:

::: moniker range=">= aspnetcore-5.0"

```csharp
using System;
using System.ComponentModel.DataAnnotations;

public class Starship
{
    ...

    [ValidateComplexType]
    public ShipDescription ShipDescription { get; set; } = new();

    ...
}
```

::: moniker-end

::: moniker range="< aspnetcore-5.0"

```csharp
using System;
using System.ComponentModel.DataAnnotations;

public class Starship
{
    ...

    [ValidateComplexType]
    public ShipDescription ShipDescription { get; set; } = 
        new ShipDescription();

    ...
}
```

::: moniker-end

`ShipDescription.cs`:

```csharp
using System;
using System.ComponentModel.DataAnnotations;

public class ShipDescription
{
    [Required]
    [StringLength(40, ErrorMessage = "Description too long (40 char).")]
    public string ShortDescription { get; set; }

    [Required]
    [StringLength(240, ErrorMessage = "Description too long (240 char).")]
    public string LongDescription { get; set; }
}
```

## Enable the submit button based on form validation

To enable and disable the submit button based on form validation, the following example:

* Uses a shortened version of the preceding `Starfleet Starship Database` form (`FormExample2` component) that only accepts a value for the ship's identifier. The other `Starship` properties receive valid default values when an instance of the `Starship` type is created.
* Uses the form's <xref:Microsoft.AspNetCore.Components.Forms.EditContext> to assign the model when the component is initialized.
* Validates the form in the context's <xref:Microsoft.AspNetCore.Components.Forms.EditContext.OnFieldChanged> callback to enable and disable the submit button.
* Implements <xref:System.IDisposable> and unsubscribes the event handler in the `Dispose` method. For more information, see <xref:blazor/components/lifecycle#component-disposal-with-idisposable-and-iasyncdisposable>.

> [!NOTE]
> When assigning to the <xref:Microsoft.AspNetCore.Components.Forms.EditForm.EditContext?displayProperty=nameWithType>, don't also assign an <xref:Microsoft.AspNetCore.Components.Forms.EditForm.Model?displayProperty=nameWithType> to the <xref:Microsoft.AspNetCore.Components.Forms.EditForm>.

`Pages/FormExample9.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample9.razor)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/forms-and-validation/FormExample9.razor)]

::: moniker-end

If a form isn't preloaded with valid values and you wish to disable the **`Submit`** button on form load, set `formInvalid` to `true`.

A side effect of the preceding approach is that a validation summary (<xref:Microsoft.AspNetCore.Components.Forms.ValidationSummary> component) is populated with invalid fields after the user interacts with any one field. Address this scenario in either of the following ways:

* Don't use a <xref:Microsoft.AspNetCore.Components.Forms.ValidationSummary> component on the form.
* Make the <xref:Microsoft.AspNetCore.Components.Forms.ValidationSummary> component visible when the submit button is selected (for example, in a `HandleValidSubmit` method).

```razor
<EditForm EditContext="@editContext" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary style="@displaySummary" />

    ...

    <button type="submit" disabled="@formInvalid">Submit</button>
</EditForm>

@code {
    private string displaySummary = "display:none";

    ...

    private void HandleValidSubmit()
    {
        displaySummary = "display:block";
    }
}
```

## Troubleshoot

> InvalidOperationException: EditForm requires a Model parameter, or an EditContext parameter, but not both.

Confirm that the <xref:Microsoft.AspNetCore.Components.Forms.EditForm> assigns a <xref:Microsoft.AspNetCore.Components.Forms.EditForm.Model> **or** an <xref:Microsoft.AspNetCore.Components.Forms.EditForm.EditContext>. Don't use both for the same form.

When assigning to <xref:Microsoft.AspNetCore.Components.Forms.EditForm.Model>, confirm that the model type is instantiated, as the following example shows:

::: moniker range=">= aspnetcore-5.0"

```csharp
private ExampleModel exampleModel = new();
```

::: moniker-end

::: moniker range="< aspnetcore-5.0"

```csharp
private ExampleModel exampleModel = new ExampleModel();
```

::: moniker-end

## Additional resources

::: moniker range=">= aspnetcore-5.0"

* <xref:blazor/file-uploads>
* <xref:blazor/security/webassembly/hosted-with-azure-active-directory>
* <xref:blazor/security/webassembly/hosted-with-azure-active-directory-b2c>
* <xref:blazor/security/webassembly/hosted-with-identity-server>

::: moniker-end

::: moniker range="< aspnetcore-5.0"

* <xref:blazor/security/webassembly/hosted-with-azure-active-directory>
* <xref:blazor/security/webassembly/hosted-with-azure-active-directory-b2c>
* <xref:blazor/security/webassembly/hosted-with-identity-server>

::: moniker-end
