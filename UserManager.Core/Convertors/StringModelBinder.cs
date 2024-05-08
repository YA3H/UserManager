using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DNTPersianUtils.Core;

namespace UserManager.Core.Convertors
{
    public class StringModelBinder : IModelBinder
    {
        private readonly IModelBinder _fallbackBinder;

        public StringModelBinder(IModelBinder fallbackBinder)
        {
            if (fallbackBinder == null)
                throw new ArgumentNullException(nameof(fallbackBinder));

            _fallbackBinder = fallbackBinder;
        }
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var attemptedValue = value.FirstValue;



            ///var modifiedValue = value.ToString().Replace((char)1610, (char)1740).Replace((char)1603, (char)1705);

            var modifiedValue = value.ToString().ApplyCorrectYeKe().ToEnglishNumbers();

            bindingContext.Result = ModelBindingResult.Success(modifiedValue);
        }
    }

    public class StringEncodeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.Metadata.IsComplexType && context.Metadata.ModelType == typeof(string)) // only encode string types
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new StringModelBinder(new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory));
            }

            return null;
        }
    }

    public static class MvcOptionsExtensions
    {
        public static void UseStringEncodeModelBinding(this MvcOptions opts)
        {
            var binderToFind = opts.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(SimpleTypeModelBinderProvider));

            if (binderToFind == null) return;

            var index = opts.ModelBinderProviders.IndexOf(binderToFind);
            opts.ModelBinderProviders.Insert(index, new StringEncodeModelBinderProvider());
        }
    }
}
