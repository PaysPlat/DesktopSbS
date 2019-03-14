using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace DesktopSbS.Converters
{
	/// <summary>
	/// A value converter which contains a list of IValueConverters and invokes their Convert or ConvertBack methods
	/// in the order that they exist in the list.  The output of one converter is piped into the next converter
	/// allowing for modular value converters to be chained together.  If the ConvertBack method is invoked, the
	/// value converters are executed in reverse order (highest to lowest index).  Do not leave an element in the
	/// Converters property collection null, every element must reference a valid IValueConverter instance. If a
	/// value converter's type is not decorated with the ValueConversionAttribute, an InvalidOperationException will be
	/// thrown when the converter is added to the Converters collection.
	/// </summary>
	[System.Windows.Markup.ContentProperty("Converters")]
	public class ValueConverterGroup : IValueConverter
	{
		#region Data

		private readonly ObservableCollection<IValueConverter> converters = new ObservableCollection<IValueConverter>();
		private readonly Dictionary<IValueConverter, ValueConversionAttribute> cachedAttributes = new Dictionary<IValueConverter, ValueConversionAttribute>();

		#endregion // Data

		#region Constructor

		public ValueConverterGroup()
		{
			this.converters.CollectionChanged += this.OnConvertersCollectionChanged;
		}

		#endregion // Constructor

		#region Converters

/// <summary>
/// Returns the list of IValueConverters contained in this converter.
/// </summary>
public ObservableCollection<IValueConverter> Converters
{
	get { return this.converters; }
}

		#endregion // Converters

		#region IValueConverter Members

		object IValueConverter.Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			object output = value;

            #region ModParametersLouis
            string[] convParams = new string[this.Converters.Count];

            string param = parameter as string;
            if (!string.IsNullOrEmpty(param))
            {
                string[] tab = param.Split('|');
                for (int i = 0; i < tab.Length; ++i)
                {
                    if (i < this.Converters.Count)
                    {
                        convParams[i] = tab[i];
                    }
                }
            }
            #endregion

            for ( int i = 0; i < this.Converters.Count; ++i )
			{
				IValueConverter converter = this.Converters[i];
				Type currentTargetType = this.GetTargetType( i, targetType, true );
				output = converter.Convert( output, currentTargetType, convParams[i], culture );

				// If the converter returns 'DoNothing' then the binding operation should terminate.
				if( output == Binding.DoNothing )
					break;
			}

			return output;
		}

		object IValueConverter.ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			object output = value;

            #region ModParametersLouis
            string[] convParams = new string[this.Converters.Count];

            string param = parameter as string;
            if (!string.IsNullOrEmpty(param))
            {
                string[] tab = param.Split('|');
                for (int i = 0; i < tab.Length; ++i)
                {
                    if (i < this.Converters.Count)
                    {
                        convParams[i] = tab[i];
                    }
                }
            }
            #endregion


			for( int i = this.Converters.Count - 1; i > -1; --i )
			{
				IValueConverter converter = this.Converters[i];
				Type currentTargetType = this.GetTargetType( i, targetType, false );
				output = converter.ConvertBack( output, currentTargetType, convParams[i], culture );

				// When a converter returns 'DoNothing' the binding operation should terminate.
				if( output == Binding.DoNothing )
					break;
			}

			return output;
		}

		#endregion // IValueConverter Members

		#region Private Helpers

			#region GetTargetType

		/// <summary>
		/// Returns the target type for a conversion operation.
		/// </summary>
		/// <param name="converterIndex">The index of the current converter about to be executed.</param>
		/// <param name="finalTargetType">The 'targetType' argument passed into the conversion method.</param>
		/// <param name="convert">Pass true if calling from the Convert method, or false if calling from ConvertBack.</param>
		protected virtual Type GetTargetType( int converterIndex, Type finalTargetType, bool convert )
		{
			// If the current converter is not the last/first in the list, 
			// get a reference to the next/previous converter.
			IValueConverter nextConverter = null;
			if( convert )
			{
				if( converterIndex < this.Converters.Count - 1 )
				{
					nextConverter = this.Converters[converterIndex + 1];
					if( nextConverter == null )
						throw new InvalidOperationException( "The Converters collection of the ValueConverterGroup contains a null reference at index: " + (converterIndex + 1) );
				}
			}
			else
			{
				if( converterIndex > 0 )
				{
					nextConverter = this.Converters[converterIndex - 1];
					if( nextConverter == null )
						throw new InvalidOperationException( "The Converters collection of the ValueConverterGroup contains a null reference at index: " + (converterIndex - 1) );
				}
			}

			if( nextConverter != null )
			{
				ValueConversionAttribute conversionAttribute = cachedAttributes[nextConverter];

				// If the Convert method is going to be called, we need to use the SourceType of the next 
				// converter in the list.  If ConvertBack is called, use the TargetType.
				return convert ? conversionAttribute.SourceType : conversionAttribute.TargetType; 
			}

			// If the current converter is the last one to be executed return the target type passed into the conversion method.
			return finalTargetType;
		}

			#endregion // GetTargetType

			#region OnConvertersCollectionChanged

		void OnConvertersCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			// The 'Converters' collection has been modified, so validate that each value converter it now
			// contains is decorated with ValueConversionAttribute and then cache the attribute value.

			IList convertersToProcess = null;
			if( e.Action == NotifyCollectionChangedAction.Add ||
				e.Action == NotifyCollectionChangedAction.Replace )
			{
				convertersToProcess = e.NewItems;
			}
			else if( e.Action == NotifyCollectionChangedAction.Remove )
			{
				foreach( IValueConverter converter in e.OldItems )
					this.cachedAttributes.Remove( converter );
			}
			else if( e.Action == NotifyCollectionChangedAction.Reset )
			{
				this.cachedAttributes.Clear();
				convertersToProcess = this.converters;
			}

			if( convertersToProcess != null && convertersToProcess.Count > 0 )
			{
				foreach( IValueConverter converter in convertersToProcess )
				{
					object[] attributes = converter.GetType().GetCustomAttributes( typeof( ValueConversionAttribute ), false );

					if( attributes.Length != 1 )
						throw new InvalidOperationException( "All value converters added to a ValueConverterGroup must be decorated with the ValueConversionAttribute attribute exactly once." );

					this.cachedAttributes.Add( converter, attributes[0] as ValueConversionAttribute );
				}
			}
		}

			#endregion // OnConvertersCollectionChanged

		#endregion // Private Helpers
	}
}