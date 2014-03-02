[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.LoquaciousConfig), "PreStart")]

namespace $rootnamespace$.App_Start
{
    /// <summary>
    /// Configures the data provider.
    /// </summary>
    public static class LoquaciousConfig
    {
        /// <summary>
        /// Activates the data provider.
        /// </summary>
        public static void PreStart()
        {
            Loquacious.DataProvider.LoquaciousDataProvider.Activate();
        }
    }
}