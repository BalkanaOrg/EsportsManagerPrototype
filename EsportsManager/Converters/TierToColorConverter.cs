using EsportsManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Converters
{
    public class TierToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TournamentTier color)
            {
                return color switch
                {
                    TournamentTier.Online => Colors.LightGrey,
                    TournamentTier.Major => Colors.Gold,
                    TournamentTier.RMR => Colors.Goldenrod,
                    TournamentTier.S => Colors.Purple,
                    TournamentTier.A => Colors.DarkGreen,
                    TournamentTier.B => Colors.Blue,
                    TournamentTier.C => Colors.LightBlue,
                    _ => Colors.White

                };
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
