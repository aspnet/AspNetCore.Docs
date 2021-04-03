﻿using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AppModelSample.Conventions
{
    public class MustBeInRouteParameterModelConvention : Attribute, IParameterModelConvention
    {
        public void Apply(ParameterModel model)
        {
            model.BindingInfo ??= new BindingInfo();
            model.BindingInfo.BindingSource = BindingSource.Path;
        }
    }
}
