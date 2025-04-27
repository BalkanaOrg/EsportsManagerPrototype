using EsportsManager.Models;
using System.Globalization;

public class TeamToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Team team && parameter is int teamId)
        {
            return team.Id == teamId ? Colors.Green : Colors.Red;
        }
        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}