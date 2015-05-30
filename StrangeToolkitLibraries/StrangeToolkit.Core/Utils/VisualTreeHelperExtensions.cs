namespace StrangeToolkit.Utils
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    public static class VisualTreeHelperExtensions
    {
        public static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            var child = default(T);
            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < numVisuals; i++)
            {
                var v = VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static T GetVisualChild<T>(DependencyObject parent, string controlName) where T : FrameworkElement
        {
            var child = default(T);
            
            //TODO: rewrite
            //var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            //for (var i = 0; i < numVisuals; i++)
            //{
            //    var v = VisualTreeHelper.GetChild(parent, i);
            //    child = v as T ?? GetVisualChild<T>(v);
            //    if (child != null)
            //    {
            //        break;
            //    }
            //}

            return child;
        }
    }
}
