using EsportsManager.ViewModels;
using EsportsManager.Views;
using Microsoft.Extensions.Logging;

namespace EsportsManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<GameService>();

        // Register view models
        builder.Services.AddTransient<TeamViewModel>();
        builder.Services.AddTransient<MarketViewModel>();
        builder.Services.AddTransient<TournamentsViewModel>();
        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<PlayerProfileViewModel>();
        builder.Services.AddTransient<TeamProfileViewModel>();
        builder.Services.AddTransient<TournamentDetailViewModel>();

        // Register views
        builder.Services.AddTransient<TeamView>();
        builder.Services.AddTransient<MarketView>();
        builder.Services.AddTransient<TournamentsView>();
        builder.Services.AddTransient<CalendarView>();
        builder.Services.AddTransient<PlayerProfileView>();
        builder.Services.AddTransient<TeamProfileView>();
        builder.Services.AddTransient<TournamentDetailView>();

        // Register converters
        builder.Services.AddSingleton<NationalityToFlagConverter>();
        builder.Services.AddSingleton<RatingToProgressConverter>();
        builder.Services.AddSingleton<TeamToColorConverter>();

        return builder.Build();
    }
}
