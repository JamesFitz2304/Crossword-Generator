using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Generator;
using CrosswordGenerator.Generator.Interfaces;
using System;

namespace CrosswordWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IGenerationManager, GenerationManager>();
            services.AddScoped<IGenerator, Generator>();
            services.AddScoped<IPlacementFinder, PlacementFinder>();

            services.AddSingleton<MainWindow>();

        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }

    }


}
