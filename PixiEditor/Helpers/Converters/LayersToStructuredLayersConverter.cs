﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using PixiEditor.Models.Layers;

namespace PixiEditor.Helpers.Converters
{
    public class LayersToStructuredLayersConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is IEnumerable<Layer> layers && values[1] is LayerStructure structure)
            {
                return new StructuredLayerTree(layers, structure).RootDirectoryItems;
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new ArgumentException("Value is not a StructuredLayerTree");
        }
    }
}