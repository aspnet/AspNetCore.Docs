---
title: ASP.NET Core Blazor data binding
author: guardrex
description: Learn about data binding features for components and Document Object Model (DOM) elements in Blazor apps.
monikerRange: '>= aspnetcore-3.1'
ms.author: riande
ms.custom: mvc
ms.date: 03/15/2021
no-loc: [Home, Privacy, Kestrel, appsettings.json, "ASP.NET Core Identity", cookie, Cookie, Blazor, "Blazor Server", "Blazor WebAssembly", "Identity", "Let's Encrypt", Razor, SignalR]
uid: blazor/components/data-binding
---
# ASP.NET Core Blazor data binding

Razor components provide data binding features with the [`@bind`](xref:mvc/views/razor#bind) Razor directive attribute with a field, property, or Razor expression value.

The following example binds:

* An `<input>` element value to the C# `inputValue` field.
* A second `<input>` element value to the C# `InputValue` property.

When an `<input>` element loses focus, its bound field or property is updated.

`Pages/Bind.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/data-binding/Bind.razor?highlight=4,8)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/data-binding/Bind.razor?highlight=4,8)]

::: moniker-end

The text box is updated in the UI only when the component is rendered, not in response to changing the field's or property's value. Since components render themselves after event handler code executes, field and property updates are usually reflected in the UI immediately after an event handler is triggered.

As a demonstration of how data binding composes in HTML, the following example binds the `InputValue` property to the second `<input>` element's `value` and [`onchange`](https://developer.mozilla.org/docs/Web/API/GlobalEventHandlers/onchange) attributes. *The second `<input>` element in the following example is a concept demonstration and isn't meant to suggest how you should bind data in Razor components.*

`Pages/BindTheory.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/data-binding/BindTheory.razor?highlight=12-14)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/data-binding/BindTheory.razor?highlight=12-14)]

::: moniker-end

When the `BindTheory` component is rendered, the `value` of the HTML demonstration `<input>` element comes from the `InputValue` property. When the user enters a value in the text box and changes element focus, the `onchange` event is fired and the `InputValue` property is set to the changed value. In reality, code execution is more complex because [`@bind`](xref:mvc/views/razor#bind) handles cases where type conversions are performed. In general, [`@bind`](xref:mvc/views/razor#bind) associates the current value of an expression with a `value` attribute and handles changes using the registered handler.

Bind a property or field on other [Document Object Model (DOM)](https://developer.mozilla.org/docs/Web/API/Document_Object_Model/Introduction) events by including an `@bind:event="{EVENT}"` attribute with a DOM event for the `{EVENT}` placeholder. The following example binds the `InputValue` property to the `<input>` element's value when the element's [`oninput` event](https://developer.mozilla.org/docs/Web/API/GlobalEventHandlers/oninput) is triggered. Unlike the [`onchange` event](https://developer.mozilla.org/docs/Web/API/GlobalEventHandlers/onchange), which fires when the element loses focus, [`oninput`](https://developer.mozilla.org/docs/Web/API/GlobalEventHandlers/oninput) fires when the value of the text box changes.

`Page/BindEvent.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/data-binding/BindEvent.razor?highlight=4)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/data-binding/BindEvent.razor?highlight=4)]

::: moniker-end

Razor attribute binding is case sensitive:

* `@bind` and `@bind:event` are valid.
* `@Bind`/`@Bind:Event` (capital letters `B` and `E`) or `@BIND`/`@BIND:EVENT` (all capital letters) **are invalid**.

::: moniker range=">= aspnetcore-6.0"

## Multiple option selection with `<input>` elements

Binding supports [`multiple`](https://developer.mozilla.org/docs/Web/HTML/Attributes/multiple) option selection with `<input>` elements. The [`@onchange`](xref:mvc/views/razor#onevent) event provides an array of the selected elements via [event arguments (`ChangeEventArgs`)](xref:blazor/components/event-handling#event-arguments). The value must be bound to an array type.

`Pages/BindMultipleInput.razor`:

```razor
@page "/bind-multiple-input"

<h1>Bind Multiple <code>input</code>Example</h1>

<p>
    <label>
        Select one or more cars: 
        <select @onchange="SelectedCarsChanged" multiple>
            <option value="audi">Audi</option>
            <option value="jeep">Jeep</option>
            <option value="opel">Opel</option>
            <option value="saab">Saab</option>
            <option value="volvo">Volvo</option>
        </select>
    </label>
</p>

<p>
    Selected Cars: @string.Join(", ", SelectedCars)
</p>

<p>
    <label>
        Select one or more cities: 
        <select @bind="SelectedCities" multiple>
            <option value="@("\"bal\"")">Baltimore</option>
            <option value="@("\"la\"")">Los Angeles</option>
            <option value="@("\"pdx\"")">Portland</option>
            <option value="@("\"sf\"")">San Francisco</option>
            <option value="@("\"sea\"")">Seattle</option>
        </select>
    </label>
</p>

<span>
    Selected Cities: @string.Join(", ", SelectedCities)
</span>

@code {
    public string[] SelectedCars { get; set; } = new string[] { };
    public string[] SelectedCities { get; set; } = new[] { "\"bal\"", "\"sea\"" };

    void SelectedCarsChanged(ChangeEventArgs e)
    {
        SelectedCars = (string[])e.Value;
    }
}
```

For information on how empty strings and `null` values are handled in data binding, see the [Binding `<select>` element options to C# object `null` values](#binding-select-element-options-to-c-object-null-values) section.

::: moniker-end

## Binding `<select>` element options to C# object `null` values

There's no sensible way to represent a `<select>` element option value as a C# object `null` value, because:

* HTML attributes can't have `null` values. The closest equivalent to `null` in HTML is absence of the HTML `value` attribute from the `<option>` element.
* When selecting an `<option>` with no `value` attribute, the browser treats the value as the *text content* of that `<option>`'s element.

The Blazor framework doesn't attempt to suppress the default behavior because it would involve:

* Creating a chain of special-case workarounds in the framework.
* Breaking changes to current framework behavior.

::: moniker range=">= aspnetcore-5.0"

The most plausible `null` equivalent in HTML is an *empty string* `value`. The Blazor framework handles `null` to empty string conversions for two-way binding to a `<select>`'s value.

::: moniker-end

::: moniker range="< aspnetcore-5.0"

The Blazor framework doesn't automatically handle `null` to empty string conversions when attempting two-way binding to a `<select>`'s value. For more information, see [Fix binding `<select>` to a null value (dotnet/aspnetcore #23221)](https://github.com/dotnet/aspnetcore/pull/23221).

::: moniker-end

## Unparsable values

When a user provides an unparsable value to a databound element, the unparsable value is automatically reverted to its previous value when the bind event is triggered.

Consider the following component, where an `<input>` element is bound to an `int` type with an initial value of `123`.

`Pages/UnparsableValues.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/data-binding/UnparsableValues.razor?highlight=4,12)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/data-binding/UnparsableValues.razor?highlight=4,12)]

::: moniker-end

By default, binding applies to the element's `onchange` event. If the user updates the value of the text box's entry to `123.45` and changes the focus, the element's value is reverted to `123` when `onchange` fires. When the value `123.45` is rejected in favor of the original value of `123`, the user understands that their value wasn't accepted.

For the `oninput` event (`@bind:event="oninput"`), a value reversion occurs after any keystroke that introduces an unparsable value. When targeting the `oninput` event with an `int`-bound type, a user is prevented from typing a dot (`.`) character. A dot (`.`) character is immediately removed, so the user receives immediate feedback that only whole numbers are permitted. There are scenarios where reverting the value on the `oninput` event isn't ideal, such as when the user should be allowed to clear an unparsable `<input>` value. Alternatives include:

* Don't use the `oninput` event. Use the default `onchange` event, where an invalid value isn't reverted until the element loses focus.
* Bind to a nullable type, such as `int?` or `string` and provide [custom `get` and `set` accessor logic](#custom-binding-formats) to handle invalid entries.
* Use a [form validation component](xref:blazor/forms-validation), such as <xref:Microsoft.AspNetCore.Components.Forms.InputNumber%601> or <xref:Microsoft.AspNetCore.Components.Forms.InputDate%601>. Form validation components provide built-in support to manage invalid inputs. Form validation components:
  * Permit the user to provide invalid input and receive validation errors on the associated <xref:Microsoft.AspNetCore.Components.Forms.EditContext>.
  * Display validation errors in the UI without interfering with the user entering additional webform data.

## Format strings

Data binding works with a single <xref:System.DateTime> format string using `@bind:format="{FORMAT STRING}"`, where the `{FORMAT STRING}` placeholder is the format string. Other format expressions, such as currency or number formats, aren't available at this time but might be added in a future release.

`Pages/DateBinding.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/data-binding/DateBinding.razor?highlight=6)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/data-binding/DateBinding.razor?highlight=6)]

::: moniker-end

In the preceding code, the `<input>` element's field type (`type` attribute) defaults to `text`.

Nullable <xref:System.DateTime?displayProperty=fullName> and <xref:System.DateTimeOffset?displayProperty=fullName> are supported:

```csharp
private DateTime? date;
private DateTimeOffset? dateOffset;
```

Specifying a format for the `date` field type isn't recommended because Blazor has built-in support to format dates. In spite of the recommendation, only use the `yyyy-MM-dd` date format for binding to function correctly if a format is supplied with the `date` field type:

```razor
<input type="date" @bind="startDate" @bind:format="yyyy-MM-dd">
```

## Custom binding formats

[C# `get` and `set` accessors](/dotnet/csharp/programming-guide/classes-and-structs/using-properties) can be used to create custom binding format behavior, as the following `DecimalBinding` component demonstrates. The component binds a positive or negative decimal with up to three decimal places to an `<input>` element by way of a `string` property (`DecimalValue`).

`Pages/DecimalBinding.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/data-binding/DecimalBinding.razor?highlight=7,21-31)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/data-binding/DecimalBinding.razor?highlight=7,21-31)]

::: moniker-end

## Binding with component parameters

A common scenario is binding a property of a child component to a property in its parent component. This scenario is called a *chained bind* because multiple levels of binding occur simultaneously.

[Component parameters](xref:blazor/components/index#component-parameters) permit binding properties of a parent component with `@bind-{PROPERTY}` syntax, where the `{PROPERTY}` placeholder is the property to bind.

You can't implement chained binds with [`@bind`](xref:mvc/views/razor#bind) syntax in the child component. An event handler and value must be specified separately to support updating the property in the parent from the child component.

The parent component still leverages the [`@bind`](xref:mvc/views/razor#bind) syntax to set up the databinding with the child component.

The following `ChildBind` component has a `Year` component parameter and an <xref:Microsoft.AspNetCore.Components.EventCallback%601>. By convention, the <xref:Microsoft.AspNetCore.Components.EventCallback%601> for the parameter must be named as the component parameter name with a "`Changed`" suffix. The naming syntax is `{PARAMETER NAME}Changed`, where the `{PARAMETER NAME}` placeholder is the parameter name. In the following example, the <xref:Microsoft.AspNetCore.Components.EventCallback%601> is named `YearChanged`.

<xref:Microsoft.AspNetCore.Components.EventCallback.InvokeAsync%2A?displayProperty=nameWithType> invokes the delegate associated with the binding with the provided argument and dispatches an event notification for the changed property.

`Shared/ChildBind.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Shared/data-binding/ChildBind.razor?highlight=14-15,17-18,22)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Shared/data-binding/ChildBind.razor?highlight=14-15,17-18,22)]

::: moniker-end

For more information on events and <xref:Microsoft.AspNetCore.Components.EventCallback%601>, see the *EventCallback* section of the <xref:blazor/components/event-handling#eventcallback> article.

In the following `Parent` component, the `year` field is bound to the `Year` parameter of the child component. The `Year` parameter is bindable because it has a companion `YearChanged` event that matches the type of the `Year` parameter.

`Pages/Parent.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/data-binding/Parent1.razor?highlight=9)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/data-binding/Parent1.razor?highlight=9)]

::: moniker-end

By convention, a property can be bound to a corresponding event handler by including an `@bind-{PROPERTY}:event` attribute assigned to the handler, where the `{PROPERTY}` placeholder is the property. `<ChildBind @bind-Year="year" />` is equivalent to writing:

```razor
<ChildBind @bind-Year="year" @bind-Year:event="YearChanged" />
```

In a more sophisticated and real-world example, the following `PasswordEntry` component:

* Sets an `<input>` element's value to a `password` field.
* Exposes changes of a `Password` property to a parent component with an [`EventCallback`](xref:blazor/components/event-handling#eventcallback) that passes in the current value of the child's `password` field as its argument.
* Uses the `onclick` event to trigger the `ToggleShowPassword` method. For more information, see <xref:blazor/components/event-handling>.

`Shared/PasswordEntry.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Shared/data-binding/PasswordEntry.razor?highlight=7-10,13,23-24,26-27,36-39)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Shared/data-binding/PasswordEntry.razor?highlight=7-10,13,23-24,26-27,36-39)]

::: moniker-end

The `PasswordEntry` component is used in another component, such as the following `PasswordBinding` component example.

`Pages/PasswordBinding.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/data-binding/PasswordBinding.razor?highlight=5)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/data-binding/PasswordBinding.razor?highlight=5)]

::: moniker-end

When the `PasswordBinding` component is initially rendered, the `password` value of `Not set` is displayed in the UI. After initial rendering, the value of `password` reflects changes made to the `Password` component parameter value in the `PasswordEntry` component.

> [!NOTE]
> The preceding example binds the password one-way from the child `PasswordEntry` component to the parent `PasswordBinding` component. Two-way binding isn't a requirement in this scenario if the goal is for the app to have a shared password entry component for reuse around the app that merely passes the password to the parent. For an approach that permits two-way binding without [writing directly to the child component's parameter](xref:blazor/components/index#overwritten-parameters), see the `NestedChild` component example in the [Bind across more than two components](#bind-across-more-than-two-components) section of this article.

Perform checks or trap errors in the handler. The following revised `PasswordEntry` component provides immediate feedback to the user if a space is used in the password's value.

`Shared/PasswordEntry.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Shared/data-binding/PasswordEntry2.razor?highlight=35-46)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Shared/data-binding/PasswordEntry2.razor?highlight=35-46)]

::: moniker-end

## Bind across more than two components

You can bind parameters through any number of nested components, but you must respect the one-way flow of data:

* Change notifications *flow up the hierarchy*.
* New parameter values *flow down the hierarchy*.

A common and recommended approach is to only store the underlying data in the parent component to avoid any confusion about what state must be updated, as shown in the following example.

`Pages/Parent.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Pages/data-binding/Parent2.razor)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Pages/data-binding/Parent2.razor)]

::: moniker-end

`Shared/NestedChild.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Shared/data-binding/NestedChild.razor)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Shared/data-binding/NestedChild.razor)]

::: moniker-end

> [!WARNING]
> Generally, avoid creating components that write directly to their own component parameters. The preceding `NestedChild` component makes use of a `BoundValue` property instead of writing directly to its `ChildMessage` parameter. For more information, see <xref:blazor/components/index#overwritten-parameters>.

`Shared/NestedGrandchild.razor`:

::: moniker range=">= aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/5.x/BlazorSample_WebAssembly/Shared/data-binding/NestedGrandchild.razor)]

::: moniker-end

::: moniker range="< aspnetcore-5.0"

[!code-razor[](~/blazor/common/samples/3.x/BlazorSample_WebAssembly/Shared/data-binding/NestedGrandchild.razor)]

::: moniker-end

For an alternative approach suited to sharing data in memory and across components that aren't necessarily nested, see <xref:blazor/state-management>.

## Additional resources

* <xref:blazor/forms-validation>
* [Binding to radio buttons in a form](xref:blazor/forms-validation#radio-buttons)
* [Binding `InputSelect` options to C# object `null` values](xref:blazor/forms-validation#binding-inputselect-options-to-c-object-null-values)
* [ASP.NET Core Blazor event handling: `EventCallback` section](xref:blazor/components/event-handling#eventcallback)
