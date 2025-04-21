using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace client
{
    public class AutoScrollBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += (s, e) =>
            {
                var collection = AssociatedObject.Items;
                ((INotifyCollectionChanged)collection).CollectionChanged += (s2, e2) =>
                {
                    if (collection.Count > 0)
                        AssociatedObject.ScrollIntoView(collection[collection.Count - 1]);
                };
            };
        }
    }
}
